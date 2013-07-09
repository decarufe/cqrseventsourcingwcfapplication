using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using SimpleCqrs.Domain;

namespace Server.Engine.Domain
{
  public class DomainModel : AggregateRoot, ISnapshotOriginator
  {
    private State _state = new State
    {
      Systems = new List<SystemGroup>(),
      SystemsAddedSinceLastCommit = new List<SystemGroup>(),
      LastCommitedVersion = new Version(0,0),
    };
    private long _appliedEventsSize;
    private bool _shouldTakeSnapshot;
    private long _lastSnapshotSize;
    private const long Ratio = 2;

    public class State : Snapshot
    {
      public string Name { get; set; }
      public List<SystemGroup> Systems { get; set; }
      public List<SystemGroup> SystemsAddedSinceLastCommit { get; set; }
      public Version LastCommitedVersion { get; set; }
      public bool MajorChange { get; set; }
    }

    public DomainModel()
    {
    }

    private DomainModel(Guid id, string name)
    {
      CreateDomainModel(id);
      ChangeName(name);
    }

    #region Snapshot
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
    #endregion

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

    public void AddSystem(string systemName, string parentSystemName)
    {
      if (_state.Systems.Any(system => system.Name == systemName))
      {
        throw new ArgumentException(String.Format("A System with the named {0} already exists.", systemName));
      }
      if (!string.IsNullOrEmpty(parentSystemName)
          && _state.Systems.All(system => system.Name != parentSystemName))
      {
        throw new ArgumentException(String.Format("Parent System named {0} does not exist.", parentSystemName));
      }

      Apply(new SystemAddedEvent
      {
        Name = systemName,
        ParentSystemName = parentSystemName
      });
    }

    [UsedImplicitly]
    private void OnSystemAdded(SystemAddedEvent systemAddedEvent)
    {
      var newSystem = new SystemGroup
      {
        Name = systemAddedEvent.Name,
        ParentSystemName = systemAddedEvent.ParentSystemName,
      };
      _state.Systems.Add(newSystem);
      _state.SystemsAddedSinceLastCommit.Add(newSystem);
      _appliedEventsSize += ComputeSize(systemAddedEvent);
      _shouldTakeSnapshot = _appliedEventsSize > _lastSnapshotSize * Ratio;
    }

    public void RemoveSystem(string systemName)
    {
      if (_state.Systems.All(system => system.Name != systemName))
      {
        throw new ArgumentException(String.Format("The System named {0} does not exists.", systemName));
      }

      IEnumerable<string> systemsToRemove = ListSystemAndChildren(systemName);
      foreach (var systemNameToRemove in systemsToRemove)
      {
        Apply(new SystemRemovedEvent
        {
          Name = systemNameToRemove,
        });
      }

    }

    private IEnumerable<string> ListSystemAndChildren(string systemName)
    {
      var list = new List<string>();
      foreach (var systemGroup in _state.Systems.Where(system => system.ParentSystemName == systemName))
      {
        list.AddRange(ListSystemAndChildren(systemGroup.Name));
      }
      list.Add(systemName);

      return list;
    }

    [UsedImplicitly]
    private void OnSystemRemoved(SystemRemovedEvent systemRemovedEvent)
    {
      var systemToRemove = _state.Systems.FirstOrDefault(system => system.Name == systemRemovedEvent.Name);
      if (systemToRemove != null)
      {
        _state.Systems.Remove(systemToRemove);

        var newlyAddedSystemGroup =
          _state.SystemsAddedSinceLastCommit.FirstOrDefault(system => system.Name == systemToRemove.Name);
        if (newlyAddedSystemGroup != null)
        {
          _state.SystemsAddedSinceLastCommit.Remove(newlyAddedSystemGroup);
        }
        else
        {
          _state.MajorChange = true;
        }
      }

      _appliedEventsSize += ComputeSize(systemRemovedEvent);
      _shouldTakeSnapshot = _appliedEventsSize > _lastSnapshotSize * Ratio;
    }

    public void CommitVersion()
    {
      var newVersion = new VersionCommitedEvent
      {
        NewVersion = _state.MajorChange
                       ? new Version(_state.LastCommitedVersion.Major + 1, 0).ToString()
                       : new Version(_state.LastCommitedVersion.Major,
                                     _state.LastCommitedVersion.Minor + 1).ToString()
      };
      Apply(newVersion);
    }

    [UsedImplicitly]
    private void OnVersionCommited(VersionCommitedEvent versionCommitedEvent)
    {
      _state.LastCommitedVersion = Version.Parse(versionCommitedEvent.NewVersion);
      _state.SystemsAddedSinceLastCommit.Clear();
      _appliedEventsSize += ComputeSize(versionCommitedEvent);
      _shouldTakeSnapshot = _appliedEventsSize > _lastSnapshotSize * Ratio;
    }
  }
}