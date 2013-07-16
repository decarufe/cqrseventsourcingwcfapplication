using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AddDispatcherCommand : ICommand
  {
    private readonly string _name;
    private readonly long _nodeId;
    private readonly Guid _id;

    public AddDispatcherCommand(Guid id, string name, long nodeId)
    {
      _id = id;
      _name = name;
      _nodeId = nodeId;
    }

    public string Name
    {
      get { return _name; }
    }

    public long NodeId
    {
      get { return _nodeId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}