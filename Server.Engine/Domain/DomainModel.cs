using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace Server.Engine.Domain
{
  public class DomainModel : AggregateRoot, ISnapshotOriginator
  {
    private State _state = new State
    {
      SystemElements = new List<SystemElement>(),
      SystemElementsAddedSinceLastCommit = new List<SystemElement>(),
      LastCommitedVersion = new Version(0,0),
    };
    private long _appliedEventsSize;
    private bool _shouldTakeSnapshot;
    private long _lastSnapshotSize;
    private const long Ratio = 2;

    public class State : Snapshot
    {
      public string Name { get; set; }
      public List<SystemElement> SystemElements { get; set; }
      public List<SystemElement> SystemElementsAddedSinceLastCommit { get; set; }
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

    private void ComputeSnapshotRequirements(DomainEvent @event)
    {
      _appliedEventsSize += ComputeSize(@event);
      _shouldTakeSnapshot = _appliedEventsSize > _lastSnapshotSize * Ratio;
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
    public void OnDomainModelCreated(DomainModelCreatedEvent @event)
    {
      Id = @event.AggregateRootId;
      ComputeSnapshotRequirements(@event);
      //_appliedEventsSize += ComputeSize(@event);
      //_shouldTakeSnapshot = _appliedEventsSize > ComputeSize(GetSnapshot());
    }

    [UsedImplicitly]
    public void OnNameChanged(NameChangedEvent @event)
    {
      _state.Name = @event.NewName;
      ComputeSnapshotRequirements(@event);
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

    public void AddSystem(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(system => system.Name == name && system.GetType() == typeof(SystemGroup)))
      {
        throw new ArgumentException(String.Format("A System named {0} already exists.", name));
      }
      if (!string.IsNullOrEmpty(parentSystemName)
          && _state.SystemElements.OfType<SystemGroup>().All(system => system.Name != parentSystemName))
      {
        throw new ArgumentException(String.Format("Parent System named {0} does not exist.", parentSystemName));
      }

      Apply(new SystemAddedEvent
      {
        Name = name,
        ParentSystemName = parentSystemName
      });
    }

    [UsedImplicitly]
    private void OnSystemAdded(SystemAddedEvent @event)
    {
      var newSystem = new SystemGroup
      {
        Name = @event.Name,
        ParentSystemName = @event.ParentSystemName,
      };
      _state.SystemElements.Add(newSystem);
      _state.SystemElementsAddedSinceLastCommit.Add(newSystem);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveSystem(string name)
    {
      var systemToRemove = _state.SystemElements.OfType<SystemGroup>().FirstOrDefault(system => system.Name != name);
      if (systemToRemove == null)
      {
        throw new ArgumentException(String.Format("The System named {0} does not exists.", name));
      }

      IEnumerable<SystemElement> systemsToRemove = ListSystemAndChildren(systemToRemove);
      foreach (var systemElement in systemsToRemove)
      {
        // Todo implement visitors
        if (systemElement.GetType() == typeof (SystemGroup))
        {
          Apply(new SystemRemovedEvent
          {
            Name = systemElement.Name,
          });
        }
        if (systemElement.GetType() == typeof(Node))
        {
          Apply(new NodeRemovedEvent
          {
            Name = systemElement.Name,
          });
        }
        if (systemElement.GetType() == typeof(Executable))
        {
          Apply(new ExecutableRemovedEvent
          {
            Name = systemElement.Name,
          });
        }
      }

    }

    private IEnumerable<SystemElement> ListSystemAndChildren(SystemElement systemElement)
    {
      var list = new List<SystemElement>();
      foreach (var element in _state.SystemElements.Where(system => system.ParentSystemName == systemElement.Name))
      {
        list.AddRange(ListSystemAndChildren(element));
      }
      list.Add(systemElement);

      return list;
    }

    [UsedImplicitly]
    private void OnSystemRemoved(SystemRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<SystemGroup>().FirstOrDefault(system => system.Name == @event.Name);
      if (toRemove != null)
      {
        _state.SystemElements.Remove(toRemove);

        var newlyAddedSystemElement =
          _state.SystemElementsAddedSinceLastCommit.FirstOrDefault(system => system.Name == toRemove.Name);
        if (newlyAddedSystemElement != null)
        {
          _state.SystemElementsAddedSinceLastCommit.Remove(newlyAddedSystemElement);
        }
        else
        {
          _state.MajorChange = true;
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AddNode(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(system => system.Name == name && system.GetType() == typeof(Node)))
      {
        throw new ArgumentException(String.Format("A Node named {0} already exists.", name));
      }
      if (!string.IsNullOrEmpty(parentSystemName)
          && _state.SystemElements.OfType<SystemGroup>().All(system => system.Name != parentSystemName))
      {
        throw new ArgumentException(String.Format("Parent System named {0} does not exist.", parentSystemName));
      }

      Apply(new NodeAddedEvent
      {
        Name = name,
        ParentSystemName = parentSystemName
      });
    }

    [UsedImplicitly]
    private void OnNodeAdded(NodeAddedEvent @event)
    {
      var node = new Node
      {
        Name = @event.Name,
        ParentSystemName = @event.ParentSystemName,
      };
      _state.SystemElements.Add(node);
      _state.SystemElementsAddedSinceLastCommit.Add(node);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveNode(string name)
    {
      var toRemove = _state.SystemElements.OfType<Node>().FirstOrDefault(system => system.Name != name);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Node named {0} does not exists.", name));
      }

      Apply(new NodeRemovedEvent
      {
        Name = toRemove.Name
      });
    }

    [UsedImplicitly]
    private void OnNodeRemoved(NodeRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Node>().FirstOrDefault(system => system.Name == @event.Name);
      if (toRemove != null)
      {
        _state.SystemElements.Remove(toRemove);

        var newlyAddedSystemElement =
          _state.SystemElementsAddedSinceLastCommit.FirstOrDefault(system => system.Name == toRemove.Name);
        if (newlyAddedSystemElement != null)
        {
          _state.SystemElementsAddedSinceLastCommit.Remove(newlyAddedSystemElement);
        }
        else
        {
          _state.MajorChange = true;
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AddExecutable(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(system => system.Name == name && system.GetType() == typeof(Node)))
      {
        throw new ArgumentException(String.Format("A Executable named {0} already exists.", name));
      }
      if (!string.IsNullOrEmpty(parentSystemName)
          && _state.SystemElements.OfType<SystemGroup>().All(system => system.Name != parentSystemName))
      {
        throw new ArgumentException(String.Format("Parent System named {0} does not exist.", parentSystemName));
      }

      Apply(new ExecutableAddedEvent
      {
        Name = name,
        ParentSystemName = parentSystemName
      });
    }

    [UsedImplicitly]
    private void OnExecutableAdded(ExecutableAddedEvent @event)
    {
      var executable = new Executable
      {
        Name = @event.Name,
        ParentSystemName = @event.ParentSystemName,
      };
      _state.SystemElements.Add(executable);
      _state.SystemElementsAddedSinceLastCommit.Add(executable);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveExecutable(string name)
    {
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(system => system.Name != name);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Executable named {0} does not exists.", name));
      }

      Apply(new ExecutableRemovedEvent
      {
        Name = toRemove.Name
      });
    }

    [UsedImplicitly]
    private void OnExecutableRemoved(ExecutableRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(system => system.Name == @event.Name);
      if (toRemove != null)
      {
        _state.SystemElements.Remove(toRemove);

        var newlyAddedSystemElement =
          _state.SystemElementsAddedSinceLastCommit.FirstOrDefault(system => system.Name == toRemove.Name);
        if (newlyAddedSystemElement != null)
        {
          _state.SystemElementsAddedSinceLastCommit.Remove(newlyAddedSystemElement);
        }
        else
        {
          _state.MajorChange = true;
        }
      }

      ComputeSnapshotRequirements(@event);
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
    private void OnVersionCommited(VersionCommitedEvent @event)
    {
      _state.LastCommitedVersion = Version.Parse(@event.NewVersion);
      _state.SystemElementsAddedSinceLastCommit.Clear();
      ComputeSnapshotRequirements(@event);
    }
  }
}