using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventStore;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using Server.Contracts;
using Server.DomainObjects;
using SimpleCqrs.Eventing;

namespace Server.Engine
{
  public class NEventStore : IEventStore
  {
    private readonly IStoreEvents _store;

    public NEventStore()
    {
      RegisterMapping();

      _store = Wireup
        .Init()
        .UsingMongoPersistence("EventStore", new DocumentObjectSerializer())
        .InitializeStorageEngine()
        .UsingBsonSerialization()
        .Build();
    }

    private static void RegisterMapping()
    {
      // The following code is equivalent to 
      //   BsonClassMap.RegisterClassMap<DomainModelCreatedEvent>();
      //   BsonClassMap.RegisterClassMap<NameChangedEvent>();
      //   ...
      Assembly eventsAssembly = typeof(ICqrsService).Assembly;
      var types = (from t in eventsAssembly.GetTypes()
                   where (t.IsPublic || t.IsNestedPublic)
                         && t.IsClass
                         && t != typeof(ReadModelInfo) // Remove ReadModelInfo since it seems to already be registred
                   select t).ToList();

      Assembly domainAssembly = typeof(DomainModel).Assembly;
      types.AddRange((from t in domainAssembly.GetTypes()
                      where (t.IsPublic || t.IsNestedPublic)
                            && t.IsClass
                      select t).ToList());

      var mapper = (from map in typeof(BsonClassMap)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod)
                    where map.Name == "RegisterClassMap"
                          && !map.GetParameters().Any()
                    select map).Single();

      foreach (var type in types)
      {
        MethodInfo map = mapper.MakeGenericMethod(new[] { type });
        try
        {
          map.Invoke(null, null);
        }
        catch (Exception e)
        {
          Console.WriteLine("Cannot map {0}: {1}",type.FullName , e);
        }
      }

      // End of mapping
    }

    public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
    {
      var commits = _store.Advanced.GetFrom(aggregateRootId, startSequence + 1, Int32.MaxValue);
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
                         select (DomainEvent) eventMessage.Body;

      return domainEvents;
    }

    public IEnumerable<DomainEvent> GetEventsByEventTypes(IEnumerable<Type> domainEventTypes, Guid aggregateRootId)
    {
      var commits = _store.Advanced.GetFrom(aggregateRootId, 0, int.MaxValue);
      var domainEvents = from commit in commits
                         from eventMessage in commit.Events
                         where domainEventTypes.Contains(eventMessage.Body.GetType())
                         select (DomainEvent) eventMessage.Body;

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
                         select (DomainEvent) eventMessage.Body;

      return domainEvents;
    }

    public Snapshot LoadSnapshot(Guid aggregateRootId)
    {
      Snapshot snapshot = _store.Advanced.GetSnapshot(aggregateRootId, Int32.MaxValue);
      return snapshot;
    }

    public void TakeSnapshot(Snapshot snapshot)
    {
      _store.Advanced.AddSnapshot(snapshot);
    }
  }
}