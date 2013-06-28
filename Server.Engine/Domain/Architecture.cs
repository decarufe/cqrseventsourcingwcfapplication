using System;
using Server.Engine.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class Architecture : AggregateRoot
  {
    private string _name;

    public Architecture()
    {
      
    }

    public Architecture(Guid id, string name)
    {
      CreateArchitecture(id);
      ChangeName(name);
    }

    public void CreateArchitecture(Guid id)
    {
      Apply(new ArchitectureCreatedEvent(id));
    }

    private void OnArchitectureCreated(ArchitectureCreatedEvent archArchitectureCreatedEvent)
    {
      Id = archArchitectureCreatedEvent.AggregateRootId;
    }

    public void ChangeName(string newName)
    {
      Apply(new NameChangedEvent { NewName = newName });
    }

    private void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      _name = nameChangedEvent.NewName;
    }
  }
}