using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AddSystemCommand : ICommand
  {
    private readonly string _name;
    private readonly long _parentSystemId;
    private readonly Guid _id;

    public AddSystemCommand(Guid id, string name, long parentSystemId)
    {
      _id = id;
      _name = name;
      _parentSystemId = parentSystemId;
    }

    public string Name
    {
      get { return _name; }
    }

    public long ParentSystemId
    {
      get { return _parentSystemId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}