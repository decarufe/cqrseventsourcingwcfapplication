using System;
using System.Windows.Input;
using Client.WpfApplication.CqrsServiceReference;

namespace Client.WpfApplication
{
  public class SetNameCommand : ICommand
  {
    private readonly IRefresh _refresher;

    public SetNameCommand(IRefresh refresher)
    {
      _refresher = refresher;
    }

    public void Execute(object parameter)
    {
      var readModelEntity = (ReadModelEntity) parameter;

      using (var client = new CqrsServiceClient())
      {
        client.SetName(Guid.Parse(readModelEntity.Id), readModelEntity.Name);
      }
      _refresher.Refresh();
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public event EventHandler CanExecuteChanged;
  }
}