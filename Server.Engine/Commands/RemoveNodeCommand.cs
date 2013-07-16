using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveNodeCommand : ICommand
  {
    private readonly long _nodeId;
    private readonly Guid _id;

    public RemoveNodeCommand(Guid id, long nodeId)
    {
      _id = id;
      _nodeId = nodeId;
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