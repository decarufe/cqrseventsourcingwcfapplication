using System;
using System.Linq;
using SimpleCqrs;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
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

    protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
    {
      return _eventStore;
    }

    protected override IEventBus GetEventBus(IServiceLocator serviceLocator)
    {
      if (_bus == null) return base.GetEventBus(serviceLocator);
      return _bus;
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
