using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AddSystemElementCommand : ICommand
  {
    private readonly string _name;
    private readonly string _type;
    private readonly long _parentSystemId;
    private readonly Guid _id;

    public AddSystemElementCommand(Guid id, string name, string type, long parentSystemId)
    {
      _id = id;
      _type = type;
      _name = name;
      _parentSystemId = parentSystemId;
    }

    public string Name
    {
      get { return _name; }
    }

    public string Type
    {
      get { return _type; }
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