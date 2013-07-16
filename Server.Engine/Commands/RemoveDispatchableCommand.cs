using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveDispatchableCommand : ICommand
  {
    private readonly long _dispatchableId;
    private readonly Guid _id;

    public RemoveDispatchableCommand(Guid id, long dispatchableId)
    {
      _id = id;
      _dispatchableId = dispatchableId;
    }

    public long DispatchableId
    {
      get { return _dispatchableId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}