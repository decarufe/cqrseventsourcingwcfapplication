using System;
using System.Diagnostics;
using MongoDB.Bson;
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
      UpdateLastEvent(null);
    }

    public void Consume(ArchitectureCreatedEvent message)
    {
      Console.WriteLine("{0}: {1}", message.GetType().Name, message.AggregateRootId);
      var architectureView = new ArchitectureView
      {
        Id = message.AggregateRootId.ToString(),
        LastEventSequence = message.Sequence
      };
      Persistance<ArchitectureView>.Instance.Add(architectureView);
      UpdateLastEvent(message);
    }

    public void Consume(NameChangedEvent message)
    {
      ArchitectureView architecture = Persistance<ArchitectureView>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= architecture.LastEventSequence) return;
      architecture.LastEventSequence = message.Sequence;
      architecture.Name = message.NewName;
      Persistance<ArchitectureView>.Instance.Update(architecture);
      UpdateLastEvent(message);
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.NewName);
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

    private static void UpdateLastEvent(DomainEvent message)
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ArchitectureView).FullName);
      if (message == null)
      {
        if (readModelInfo == null)
        {
          Persistance<ReadModelInfo>.Instance.Add(new ReadModelInfo(typeof (ArchitectureView))
          {
            LastEvent = DateTime.MinValue
          });
        }
        return;
      }
      if (message.EventDate.ToUniversalTime() > readModelInfo.LastEvent)
      {
        readModelInfo.LastEvent = message.EventDate.ToUniversalTime();
        Persistance<ReadModelInfo>.Instance.Update(readModelInfo);
      }
    }
  }
}