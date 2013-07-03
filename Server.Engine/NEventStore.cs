using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using SimpleCqrs.Eventing;

namespace Server.Engine
{
  public class NEventStore : IEventStore
  {
    private readonly IStoreEvents _store;

    public NEventStore()
    {
      BsonClassMap.RegisterClassMap(new BsonClassMap<ArchitectureCreatedEvent>());
      BsonClassMap.RegisterClassMap(new BsonClassMap<NameChangedEvent>());

      _store = Wireup
        .Init()
        .UsingMongoPersistence("EventStore", new DocumentObjectSerializer())
        .InitializeStorageEngine()
        .UsingBinarySerialization()
        .Compress()
        //.UsingAsynchronousDispatchScheduler()
        //.DispatchTo(new DelegateMessageDispatcher(Dispatch))
        .Build();
    }

    private void Dispatch(Commit commit)
    {
      foreach (var eventMessage in commit.Events)
      {
        Console.WriteLine("Async: " + eventMessage.Body);
      }
    }

    public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
    {
      var commits = _store.Advanced.GetFrom(aggregateRootId, startSequence, Int32.MaxValue);
      var domainEvents = from commit in commits
                         from eventMessage in commit.Events
                         select (DomainEvent) eventMessage.Body;
 
      return domainEvents;
    }

    public void Insert(IEnumerable<DomainEvent> domainEvents)
    {
      var eventGroups = from e in domainEvents
                        group e by e.AggregateRootId
                        into eg
                        select new {Id = eg.Key, Events = eg};

      foreach (var eventGroup in eventGroups)
      {
        using (IEventStream eventStream = _store.OpenStream(eventGroup.Id, 0, int.MaxValue))
        {
          foreach (var domainEvent in eventGroup.Events)
          {
            eventStream.Add(new EventMessage {Body = domainEvent});
          }
          eventStream.CommitChanges(Guid.NewGuid());
        }
      }
    }

    public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes)
    {
      var commits = _store.Advanced.GetFrom(DateTime.MinValue);
      var domainEvents = from commit in commits
                         from eventMessage in commit.Events
                         where domainEventTypes.Contains(eventMessage.Body.GetType())
                         select (DomainEvent)eventMessage.Body;

      return domainEvents;
    }

    public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
    {
      var commits = _store.Advanced.GetFrom(aggregateRootId, 0, int.MaxValue);
      var domainEvents = from commit in commits
                         from eventMessage in commit.Events
                         where domainEventTypes.Contains(eventMessage.Body.GetType())
                         select (DomainEvent)eventMessage.Body;

      return domainEvents;
    }

    public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, DateTime startDate,
                                                          DateTime endDate)
    {
      var commits = _store.Advanced.GetFrom(startDate);
      var domainEvents = from commit in commits
                         where commit.CommitStamp <= endDate
                         from eventMessage in commit.Events
                         where domainEventTypes.Contains(eventMessage.Body.GetType())
                         select (DomainEvent)eventMessage.Body;

      return domainEvents;
    }
  }
}