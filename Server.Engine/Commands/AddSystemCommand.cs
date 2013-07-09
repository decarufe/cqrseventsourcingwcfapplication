using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AddSystemCommand : ICommand
  {
    private readonly string _name;
    private readonly string _parentSystemName;
    private readonly Guid _id;

    public AddSystemCommand(Guid id, string name, string parentSystemName)
    {
      _id = id;
      _name = name;
      _parentSystemName = parentSystemName;
    }

    public string Name
    {
      get { return _name; }
    }

    public string ParentSystemName
    {
      get { return _parentSystemName; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}