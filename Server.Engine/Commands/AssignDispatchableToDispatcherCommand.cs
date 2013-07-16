using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignDispatchableToDispatcherCommand : ICommand
  {
    private readonly string _dispatchableName;
    private readonly string _dispatcherName;
    private readonly Guid _id;

    public AssignDispatchableToDispatcherCommand(Guid id, string dispatchableName, string dispatcherName)
    {
      _id = id;
      _dispatchableName = dispatchableName;
      _dispatcherName = dispatcherName;
    }

    public string DispatchableName
    {
      get { return _dispatchableName; }
    }

    public string DispatcherName
    {
      get { return _dispatcherName; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}