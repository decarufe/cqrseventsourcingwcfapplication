using System;
using System.Windows.Input;
using Client.WpfApplication.CqrsServiceReference;

namespace Client.WpfApplication
{
  public class SetNameCommand : ICommand
  {
    private readonly IRefresh _refresher;
    private bool _executing;

    public SetNameCommand(IRefresh refresher)
    {
      _refresher = refresher;
    }

    public void Execute(object parameter)
    {
      try
      {
        _executing = true;
        OnCanExecuteChanged();

        var readModelEntity = (DomainModelDto) parameter;

        using (var client = new CqrsServiceClient())
        {
          client.SetName(Guid.Parse(readModelEntity.ReadModelId), readModelEntity.Name);
        }
        _refresher.Refresh();
      }
      finally
      {
        _executing = false;
        OnCanExecuteChanged();
      }
    }

    public bool CanExecute(object parameter)
    {
      return !_executing;
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
      var handler = CanExecuteChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}