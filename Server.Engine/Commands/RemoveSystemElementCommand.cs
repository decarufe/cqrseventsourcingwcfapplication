using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveSystemElementCommand : ICommand
  {
    private readonly long _elementId;
    private readonly Guid _id;

    public RemoveSystemElementCommand(Guid id, long elementId)
    {
      _id = id;
      _elementId = elementId;
    }

    public long ElementId
    {
      get { return _elementId; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}