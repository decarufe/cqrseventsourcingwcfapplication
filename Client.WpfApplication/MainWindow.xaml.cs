using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.WpfApplication.CqrsServiceReference;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Utils;

namespace Client.WpfApplication
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IRefresh
  {
    private DefaultHost _host;

    public static readonly DependencyProperty ItemsProperty =
      DependencyProperty.Register("Items", typeof(ObservableCollection<DomainModelDto>), typeof(MainWindow),
                                  new PropertyMetadata(default(ObservableCollection<DomainModelDto>)));

    public ObservableCollection<DomainModelDto> Items
    {
      get { return (ObservableCollection<DomainModelDto>)GetValue(ItemsProperty); }
      set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof(DomainModelDto), typeof(MainWindow),
                                  new PropertyMetadata(default(DomainModelDto)));

    public DomainModelDto SelectedItem
    {
      get { return (DomainModelDto) GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty ButtonLabelProperty =
      DependencyProperty.Register("ButtonLabel", typeof (string), typeof (MainWindow),
                                  new PropertyMetadata(default(string)));

    public string ButtonLabel
    {
      get { return (string) GetValue(ButtonLabelProperty); }
      set { SetValue(ButtonLabelProperty, value); }
    }

    public MainWindow()
    {
      InitializeComponent();
      UpdateLabel();
      SetNameCommand = new SetNameCommand(this);
      BatchInsertCommand = new BatchInsertCommand(this);
      DataContext = this;
      Loaded += OnLoaded;
    }

    public SetNameCommand SetNameCommand { get; set; }
    public BatchInsertCommand BatchInsertCommand { get; set; }

    private void UpdateLabel()
    {
      ButtonLabel = SelectedItem == null || SelectedItem is NullDomainModelDto ? "Create" : "Save";
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      PrepareQueues.Prepare(WpfApplication.Properties.Resources.MsmqEndpoint, QueueType.Standard);

      try
      {
        var cqrsServiceClient = new CqrsServiceClient();
        cqrsServiceClient.Ping(new Uri(WpfApplication.Properties.Resources.MsmqEndpoint));
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Server is not responding", ex);
      }

      _host = new DefaultHost();
      _host.Start<WpfBootStrapper>();

      Refresh();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SelectedItem = (DomainModelDto) ((ListBox) e.Source).SelectedItem;
      UpdateLabel();
    }

    private class NullDomainModelDto : DomainModelDto
    {
      public NullDomainModelDto()
      {
        ReadModelId = Guid.NewGuid().ToString();
        Name = "<new>";
      }
    }

    public void Refresh()
    {
      var client = new CqrsServiceClient();
      Task<IEnumerable<DomainModelDto>>
        .Factory
        .FromAsync(client.BeginGetList, client.EndGetList, client)
        .ContinueWith(t =>
        {
          var all = new[] {new NullDomainModelDto()}
            .Union(t.Result);
          Items = new ObservableCollection<DomainModelDto>(all);
          SelectedItem = Items.FirstOrDefault();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Refresh();
    }
  }
}