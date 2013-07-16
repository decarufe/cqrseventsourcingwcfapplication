using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignExecutableToNodeCommand : ICommand
  {
    private readonly long _nodeId;
    private readonly long _executableId;
    private readonly Guid _id;

    public AssignExecutableToNodeCommand(Guid id, long executableId, long nodeId)
    {
      _id = id;
      _executableId = executableId;
      _nodeId = nodeId;
    }

    public long ExecutableId
    {
      get { return _executableId; }
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
