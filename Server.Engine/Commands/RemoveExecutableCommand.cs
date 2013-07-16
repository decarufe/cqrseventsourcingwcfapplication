using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveExecutableCommand : ICommand
  {
    private readonly long _executableId;
    private readonly Guid _id;

    public RemoveExecutableCommand(Guid id, long executableId)
    {
      _id = id;
      _executableId = executableId;
    }

    public long ExecutableId
    {
      get { return _executableId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}