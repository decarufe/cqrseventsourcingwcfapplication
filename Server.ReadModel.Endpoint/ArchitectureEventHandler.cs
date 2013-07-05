using System;
using System.Diagnostics;
using Rhino.ServiceBus;
using Server.Contracts;
using Server.Contracts.Events;
using Server.Engine.Domain;
using Server.ReadModels;
using SimpleCqrs.Eventing;

namespace Server.ReadModel.Endpoint
{
  public class ArchitectureEventHandler :
    IHandleDomainEvents<ArchitectureCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>,
    ConsumerOf<ArchitectureCreatedEvent>,
    ConsumerOf<NameChangedEvent>
  {
    private readonly ArchitectureRepository _repository;

    public ArchitectureEventHandler(ArchitectureRepository repository)
    {
      _repository = repository;
    }

    public void Consume(ArchitectureCreatedEvent message)
    {
      Console.WriteLine("{0}: {1}", message.GetType().Name, message.AggregateRootId);
      var architectureView = new ArchitectureView
      {
        AggregateRootId = message.AggregateRootId
      };
      Persistance.Instance.Add(architectureView);
    }

    public void Consume(NameChangedEvent message)
    {
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.NewName);
      ArchitectureView architecture = Persistance.Instance.Get(message.AggregateRootId);
      architecture.Name = message.NewName;
      Persistance.Instance.Update(architecture);
    }

    public void Handle(ArchitectureCreatedEvent domainEvent)
    {
      // todo: Fix brigde between SimpleCqrs and Rhino.ServiceBus
      Consume(domainEvent);
    }

    public void Handle(NameChangedEvent domainEvent)
    {
      // todo: Fix brigde between SimpleCqrs and Rhino.ServiceBus
      Consume(domainEvent);
    }
  }
}