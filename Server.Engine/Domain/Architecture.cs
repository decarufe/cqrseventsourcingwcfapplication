using System;
using Server.Engine.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class Architecture : AggregateRoot
  {
    public string _name;

    public Architecture()
    {
    }

    public Architecture(Guid id, string name)
    {
      CreateArchitecture(id);
      ChangeName(name);
    }

    private void CreateArchitecture(Guid id)
    {
      Apply(new ArchitectureCreatedEvent(id));
    }

    public void OnArchitectureCreated(ArchitectureCreatedEvent architectureCreatedEvent)
    {
      Id = architectureCreatedEvent.AggregateRootId;
    }

    public void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      _name = nameChangedEvent.NewName;
    }

    public void ChangeName(string newName)
    {
      Apply(new NameChangedEvent()
      {
        NewName = newName
      });
    }
  }
}
