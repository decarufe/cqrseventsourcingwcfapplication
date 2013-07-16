using System;
using System.Windows.Input;
using Client.WpfApplication.CqrsServiceReference;

namespace Client.WpfApplication
{
  public class BatchInsertCommand : ICommand
  {
    private readonly IRefresh _refresher;

    public BatchInsertCommand(IRefresh refresher)
    {
      _refresher = refresher;
    }

    public void Execute(object parameter)
    {
      var readModelEntity = (ReadModelEntity) parameter;

      using (var client = new CqrsServiceClient())
      {
        for (int i = 0; i < 100; i++)
        {
          client.SetName(Guid.NewGuid(), "Insert " + i);
        }
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