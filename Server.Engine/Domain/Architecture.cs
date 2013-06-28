using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Engine.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  class Architecture : AggregateRoot
  {
    private string _name;

    public Architecture()
    {
    }

    public Architecture(string name)
    {
      CreateArchitecture();
      ChangeName(name);
    }

    private void CreateArchitecture()
    {
      Id = new Guid("bozo");
      Apply(new ArchitectureCreatedEvent());
    }

    public void OnArchitectureCreated(ArchitectureCreatedEvent architectureCreatedEvent)
    {
      Id = architectureCreatedEvent.AggregateRootId;
    }

    public void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      _name = nameChangedEvent.NewName;
    }

    public void ChangeName(string name)
    {
      Apply(new NameChangedEvent
      {
        NewName = name
      });
    }
  }
}
