using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AddDispatcherCommand : ICommand
  {
    private readonly string _name;
    private readonly string _nodeName;
    private readonly Guid _id;

    public AddDispatcherCommand(Guid id, string name, string nodeName)
    {
      _id = id;
      _name = name;
      _nodeName = nodeName;
    }

    public string Name
    {
      get { return _name; }
    }

    public string NodeName
    {
      get { return _nodeName; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}