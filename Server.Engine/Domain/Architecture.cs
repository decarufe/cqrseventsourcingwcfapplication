﻿using System;
using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace Server.Engine.Domain
{
  public class Architecture : AggregateRoot, ISnapshotOriginator
  {
    private string _name;
    private long _appliedEventsSize;
    private bool _takeSnapshot;
    private long _lastSnapshotSize;

    public Architecture()
    {
    }

    private Architecture(Guid id, string name)
    {
      CreateArchitecture(id);
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

    public static Architecture Create(Guid id, string name)
    {
      return new Architecture(id, name);
    }

    private void CreateArchitecture(Guid id)
    {
      Apply(new ArchitectureCreatedEvent() {AggregateRootId = id});
    }

    public void OnArchitectureCreated(ArchitectureCreatedEvent architectureCreatedEvent)
    {
      Id = architectureCreatedEvent.AggregateRootId;
      _appliedEventsSize += ComputeSize(architectureCreatedEvent);
      _takeSnapshot = _appliedEventsSize > ComputeSize(GetSnapshot());
    }

    public void OnNameChanged(NameChangedEvent nameChangedEvent)
    {
      _name = nameChangedEvent.NewName;
      _appliedEventsSize += ComputeSize(nameChangedEvent);
      _takeSnapshot = _appliedEventsSize > _lastSnapshotSize;
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

    public Snapshot GetSnapshot()
    {
      var architectureSnapshot = new ArchitectureSnapshot()
      {
        AggregateRootId = Id, LastEventSequence = LastEventSequence, Name = _name
      };
      return architectureSnapshot;
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
      _lastSnapshotSize = ComputeSize(snapshot);
      _name = ((ArchitectureSnapshot)snapshot).Name;
    }

    public bool ShouldTakeSnapshot(Snapshot previousSnapshot)
    {
      return _takeSnapshot;
    }

    private class ArchitectureSnapshot : Snapshot
    {
      public string Name { get; set; }
    }
  }
}