using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveDispatcherCommand : ICommand
  {
    private readonly long _dispatcherId;
    private readonly Guid _id;

    public RemoveDispatcherCommand(Guid id, long dispatcherId)
    {
      _id = id;
      _dispatcherId = dispatcherId;
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