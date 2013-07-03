using System;
using Server.Contracts.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class Architecture : AggregateRoot
  {
    private string _name;

    // af2b57e3-27bd-4750-85ba-0e456ad497bc

    public Architecture()
    {
    }

    private Architecture(Guid id, string name)
    {
      CreateArchitecture(id);
      ChangeName(name);
    }

    public static Architecture Create(Guid id, string name)
    {
      return new Architecture(id, name);
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
      if (newName != _name)
      {
        Apply(new NameChangedEvent
        {
          NewName = newName
        });
      }
    }
  }
}
