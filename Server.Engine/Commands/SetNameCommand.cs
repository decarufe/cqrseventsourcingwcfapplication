using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class SetNameCommand : ICommand
  {
    public string Name { get; private set; }
    public Guid Id { get; private set; }

    public SetNameCommand(Guid id, string name)
    {
      Id = id;
      Name = name;
    }
  }
}