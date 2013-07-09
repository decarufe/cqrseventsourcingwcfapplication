using System;
using System.IO;
using JetBrains.Annotations;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class DomainModel : AggregateRoot, ISnapshotOriginator
  {
    private State _state = new State();
    private long _appliedEventsSize;
    private bool _shouldTakeSnapshot;
    private long _lastSnapshotSize;
    private const long Ratio = 2;

    private class State : Snapshot
    {
      public string Name { get; set; }
    }

    public DomainModel()
    {
    }

    private DomainModel(Guid id, string name)
    {
      CreateDomainModel(id);
      ChangeName(name);
    }

    private long ComputeSize(object obj)
    {
      long computeSize;
      using (var mem = new MemoryStream())
      {
        using (BsonWriter writer = BsonWriter.Create(mem))
        {
          BsonSerializer.Serialize(writer, obj.GetType(), obj);
        }
        mem.Flush();
        computeSize = mem.Position;
      }

      return computeSize;
    }

    public static DomainModel Create(Guid id, string name)
    {
      return new DomainModel(id, name);
    }

    private void CreateDomainModel(Guid id)
    {
      Apply(new DomainModelCreatedEvent {AggregateRootId = id});
    }

    [UsedImplicitly]
    public void OnDomainModelCreated(DomainModelCreatedEvent domainModelCreatedEvent)
    {
      Id = domainModelCreatedEvent.AggregateRootId;
      _appliedEventsSize += ComputeSize(domainModelCreatedEvent);
      _shouldTakeSnapshot = _appliedEventsSize > ComputeSize(GetSnapshot());
    }

    [UsedImplicitly]
    public void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      _state.Name = nameChangedEvent.NewName;
      _appliedEventsSize += ComputeSize(nameChangedEvent);
      _shouldTakeSnapshot = _appliedEventsSize > _lastSnapshotSize * Ratio;
    }

    public void ChangeName(string newName)
    {
      if (newName != _state.Name)
      {
        Apply(new NameChangedEvent
        {
          NewName = newName
        });
      }
    }

    public Snapshot GetSnapshot()
    {
      _state.AggregateRootId = Id;
      _state.LastEventSequence = LastEventSequence;
      return _state;
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
      _lastSnapshotSize = ComputeSize(snapshot);
      _state = ((State)snapshot);
    }

    public bool ShouldTakeSnapshot(Snapshot previousSnapshot)
    {
      return _shouldTakeSnapshot;
    }
  }
}