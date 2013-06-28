using System;
using Server.Engine.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class Architecture : AggregateRoot
  {
    public string Name;

    public Architecture(Guid id, string name)
    {
      CreateArchitecture(id);
      ChangeName(name);
    }

    private void CreateArchitecture(Guid id)
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

    public void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      Name = nameChangedEvent.NewName;
    }
  }
}