using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class SetNameCommand : ICommand
  {
    private Guid _id;
    private string _name;

    public SetNameCommand(Guid id, string name)
    {
      _id = id;
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}