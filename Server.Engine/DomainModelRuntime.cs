using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Contracts;
using SimpleCqrs;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
using Utils;
using UnityServiceLocator = SimpleCqrs.Unity.UnityServiceLocator;

namespace Server.Engine
{
  public class DomainModelRuntime : SimpleCqrsRuntime<UnityServiceLocator>
  {
    private readonly IEventBus _bus;
    private readonly NEventStore _eventStore;
    private NSnapshotStore _snapshotStore;

    public DomainModelRuntime()
    {
      _eventStore = new NEventStore();
      _snapshotStore = new NSnapshotStore(_eventStore);
    }

    public DomainModelRuntime(IEventBus bus) : this()
    {
      _bus = bus;
    }

    protected override void OnStarted(UnityServiceLocator serviceLocator)
    {
      base.OnStarted(serviceLocator);
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ReadModelEntity).FullName);
      DateTime lastEvent = readModelInfo == null ? DateTime.MinValue : readModelInfo.LastEvent;

      if (readModelInfo != null)
      {
        IEnumerable<DomainEvent> missingEvents = GetMissingEvents(readModelInfo.LastEvent);
      }
    }

    private DomainEvent[] GetMissingEvents(DateTime lastEvent)
    {
      Assembly assembly = typeof(ICqrsService).Assembly;
      var types = from t in assembly.GetTypes()
                  where t.IsPublic
                        && typeof(DomainEvent).IsAssignableFrom(t)
                  select t;

      IEnumerable<DomainEvent> events = _eventStore.GetEventsByEventTypes(types, lastEvent, DateTime.MaxValue);

      DomainEvent[] messages = events.Where(e => e.EventDate > lastEvent).ToArray();
      return messages;
    }


    protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
    {
      return _eventStore;
    }

    protected override IEventBus GetEventBus(IServiceLocator serviceLocator)
    {
      var eventBus = base.GetEventBus(serviceLocator);
      if (_bus == null)
      {
        return eventBus;
      }
      return new PiplelineEventBus(eventBus, _bus);
    }

    protected override ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
    {
      return _snapshotStore;
    }
  }

  public class NSnapshotStore : ISnapshotStore
  {
    private readonly NEventStore _eventStore;

    public NSnapshotStore(NEventStore eventStore)
    {
      _eventStore = eventStore;
    }

    public Snapshot GetSnapshot(Guid aggregateRootId)
    {
      EventStore.Snapshot loadSnapshot = _eventStore.LoadSnapshot(aggregateRootId);
      if (loadSnapshot == null) return null;
      return (Snapshot) loadSnapshot.Payload;
    }

    public void SaveSnapshot<TSnapshot>(TSnapshot snapshot) where TSnapshot : Snapshot
    {
      var nSnapshot = new EventStore.Snapshot(snapshot.AggregateRootId, snapshot.LastEventSequence, snapshot);
      _eventStore.TakeSnapshot(nSnapshot);
    }
  }
}
