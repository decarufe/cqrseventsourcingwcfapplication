using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class CommitVersionCommand: ICommand
  {
    private readonly Guid _id;

    public CommitVersionCommand(Guid id)
    {
      _id = id;
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}