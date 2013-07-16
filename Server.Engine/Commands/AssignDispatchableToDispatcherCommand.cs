using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignDispatchableToDispatcherCommand : ICommand
  {
    private readonly long _dispatchableId;
    private readonly long _dispatcherId;
    private readonly Guid _id;

    public AssignDispatchableToDispatcherCommand(Guid id, long dispatchableId, long dispatcherId)
    {
      _id = id;
      _dispatchableId = dispatchableId;
      _dispatcherId = dispatcherId;
    }

    public long DispatchableId
    {
      get { return _dispatchableId; }
    }

    public long DispatcherId
    {
      get { return _dispatcherId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}