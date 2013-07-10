using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace Client.WpfApplication
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, IRefresh
  {
    public static readonly DependencyProperty ItemsProperty =
      DependencyProperty.Register("Items", typeof (ObservableCollection<ReadModelEntity>), typeof (MainWindow), new PropertyMetadata(default(ObservableCollection<ReadModelEntity>)));

    public ObservableCollection<ReadModelEntity> Items
    {
      get { return (ObservableCollection<ReadModelEntity>) GetValue(ItemsProperty); }
      set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register("SelectedItem", typeof (ReadModelEntity), typeof (MainWindow), new PropertyMetadata(default(ReadModelEntity)));

    public ReadModelEntity SelectedItem
    {
      get { return (ReadModelEntity) GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    public static readonly DependencyProperty ButtonLabelProperty =
      DependencyProperty.Register("ButtonLabel", typeof (string), typeof (MainWindow), new PropertyMetadata(default(string)));

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
      ButtonLabel = SelectedItem == null || SelectedItem is NullReadModelEntity ? "Create" : "Save";
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Refresh();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SelectedItem = (ReadModelEntity) ((ListBox) e.Source).SelectedItem;
      UpdateLabel();
    }

    private class NullReadModelEntity : ReadModelEntity
    {
      public NullReadModelEntity()
      {
        Id = Guid.NewGuid().ToString();
        Name = "<new>";
      }
    }

    public void Refresh()
    {
      using (var client = new CqrsServiceClient())
      {
        var readModelEntities = client.GetList();
        var all = new[] { new NullReadModelEntity() }
          .Union(readModelEntities);
        Items = new ObservableCollection<ReadModelEntity>(all);
      }
      SelectedItem = Items.FirstOrDefault();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Refresh();
    }
  }
}
