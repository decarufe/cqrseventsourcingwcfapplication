using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignDispatcherToNodeCommand : ICommand
  {
    private readonly string _nodeName;
    private readonly string _dispatcherName;
    private readonly Guid _id;

    public AssignDispatcherToNodeCommand(Guid id, string dispatcherName, string nodeName)
    {
      _id = id;
      _dispatcherName = dispatcherName;
      _nodeName = nodeName;
    }

    public string DispatcherName
    {
      get { return _dispatcherName; }
    }

    public string NodeName
    {
      get { return _nodeName; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}