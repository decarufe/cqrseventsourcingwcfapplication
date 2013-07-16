using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignDispatcherToNodeCommand : ICommand
  {
    private readonly long _nodeId;
    private readonly long _dispatcherId;
    private readonly Guid _id;

    public AssignDispatcherToNodeCommand(Guid id, long dispatcherId, long nodeId)
    {
      _id = id;
      _dispatcherId = dispatcherId;
      _nodeId = nodeId;
    }

    public long DispatcherId
    {
      get { return _dispatcherId; }
    }

    public long NodeId
    {
      get { return _nodeId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}