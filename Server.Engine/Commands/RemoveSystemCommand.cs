using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveSystemCommand : ICommand
  {
    private readonly long _systemId;
    private readonly Guid _id;

    public RemoveSystemCommand(Guid id, long systemId)
    {
      _id = id;
      _systemId = systemId;
    }

    public long SystemId
    {
      get { return _systemId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}