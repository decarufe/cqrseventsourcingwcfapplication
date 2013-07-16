using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Server.Contracts.Events;
using Server.Contracts.Events.Dispatchables;
using Server.Contracts.Events.Executables;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace Server.DomainObjects
{
  public class DomainModel : AggregateRoot, ISnapshotOriginator
  {
    private State _state = new State
    {
      SystemElements = new List<SystemElement>(),
      SystemElementsAddedSinceLastCommit = new List<SystemElement>(),
      LastCommitedVersion = new Version(0,0),
      ExecutableAssignations = new List<ExecutableAssignation>(),
      DispatcherAssignations = new List<DispatcherAssignation>(),
      DispatchableAssignations = new List<DispatchableAssignation>(),
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
      public List<ExecutableAssignation> ExecutableAssignations { get; set; }
      public List<DispatcherAssignation> DispatcherAssignations { get; set; }
      public List<DispatchableAssignation> DispatchableAssignations { get; set; }
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

    #region DomainModel
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
    #endregion

    #region System Break Down
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
      var systemToRemove = _state.SystemElements.OfType<SystemGroup>().FirstOrDefault(system => system.Name == name);
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
        if (systemElement.GetType() == typeof(Dispatchable))
        {
          Apply(new DispatchableRemovedEvent
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
        RemoveSystemElement(toRemove);
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Node
    public void AddNode(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Node)))
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
      var toRemove = _state.SystemElements.OfType<Node>().FirstOrDefault(node => node.Name == name);
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
        RemoveSystemElement(toRemove);

        // Remove Exec Assignations on the Node Removed
        var assignations = _state.ExecutableAssignations.Where(x => x.NodeName == toRemove.Name).ToList();
        foreach (var executableAssignation in assignations)
        {
          _state.ExecutableAssignations.Remove(executableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Executable
    public void AddExecutable(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Executable)))
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
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(executable => executable.Name == name);
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
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(executable => executable.Name == @event.Name);
      if (toRemove != null)
      {
        RemoveSystemElement(toRemove);

        // Remove Node Assignation of the Executable being removed
        var assignations = _state.ExecutableAssignations.Where(x => x.ExecutableName == toRemove.Name).ToList();
        foreach (var executableAssignation in assignations)
        {
          _state.ExecutableAssignations.Remove(executableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignExecutableToNode(string executableName, string nodeName)
    {
      if (!_state.SystemElements.Any(element => element.Name == executableName && element.GetType() == typeof(Executable)))
      {
        throw new ArgumentException(String.Format("The Executable named {0} does not exist.", executableName));
      }
      if (!_state.SystemElements.Any(element => element.Name == nodeName && element.GetType() == typeof(Node)))
      {
        throw new ArgumentException(String.Format("The Node named {0} does not exist.", nodeName));
      }

      Apply(new ExecutableAssignedEvent
      {
        ExecutableName = executableName,
        NodeName = nodeName,
      });

    }

    [UsedImplicitly]
    private void OnExecutableAssigned(ExecutableAssignedEvent @event)
    {
      var assignation = _state.ExecutableAssignations.FirstOrDefault(x => x.ExecutableName == @event.ExecutableName);
      if (assignation == null)
      {
        assignation = new ExecutableAssignation
        {
          ExecutableName = @event.ExecutableName,
          NodeName = @event.NodeName,
        };
        _state.ExecutableAssignations.Add(assignation);
      }
      else
      {
        assignation.NodeName = @event.NodeName;
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Versionning
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
      _state.MajorChange = false;
      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Dispatcher
    public void AddDispatcher(string name, string nodeName)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Dispatcher)))
      {
        throw new ArgumentException(String.Format("A Dispatcher named {0} already exists.", name));
      }
      if (_state.SystemElements.OfType<Node>().All(system => system.Name != nodeName))
      {
        throw new ArgumentException(String.Format("Node named {0} does not exist.", nodeName));
      }

      Apply(new DispatcherAddedEvent
      {
        Name = name,
        NodeName = nodeName
      });
    }

    [UsedImplicitly]
    private void OnDispatcherAdded(DispatcherAddedEvent @event)
    {
      var dispatcher = new Dispatcher
      {
        Name = @event.Name,
      };
      _state.SystemElements.Add(dispatcher);
      _state.SystemElementsAddedSinceLastCommit.Add(dispatcher);
      _state.DispatcherAssignations.Add(new DispatcherAssignation
      {
        DispatcherName = @event.Name,
        NodeName = @event.NodeName,
      });
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveDispatcher(string name)
    {
      var toRemove = _state.SystemElements.OfType<Dispatcher>().FirstOrDefault(dispatchable => dispatchable.Name == name);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Dispatcher named {0} does not exists.", name));
      }

      Apply(new DispatcherRemovedEvent
      {
        Name = toRemove.Name
      });
    }

    [UsedImplicitly]
    private void OnDispatcherRemoved(DispatcherRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Dispatcher>().FirstOrDefault(dispatcher => dispatcher.Name == @event.Name);
      if (toRemove != null)
      {
        RemoveSystemElement(toRemove);

        // Remove all Dispatchable Assignations on the Dispatcher being removed
        var disaptchableAssignations = _state.DispatchableAssignations.Where(x => x.DispatcherName == toRemove.Name).ToList();
        foreach (var disaptchableAssignation in disaptchableAssignations)
        {
          _state.DispatchableAssignations.Remove(disaptchableAssignation);
        }

        // Remove Node Assignation of the Dispatcher being removed
        var assignations = _state.DispatcherAssignations.Where(x => x.DispatcherName == toRemove.Name).ToList();
        foreach (var dispatcherAssignation in assignations)
        {
          _state.DispatcherAssignations.Remove(dispatcherAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignDispatcherToNode(string name, string nodeName)
    {
      if (_state.SystemElements.OfType<Dispatcher>().All(element => element.Name != name))
      {
        throw new ArgumentException(String.Format("Dispatcher named {0} does not exist.", name));
      }
      if (_state.SystemElements.OfType<Node>().All(system => system.Name != nodeName))
      {
        throw new ArgumentException(String.Format("Node named {0} does not exist.", nodeName));
      }
      if (_state.DispatcherAssignations.All(dispatcher => dispatcher.DispatcherName != name))
      {
        throw new ArgumentException(String.Format("Dispatcher {0} Assignation does not exist.", name));
      }

      Apply(new DispatcherAssignedEvent
      {
        DispatcherName = name,
        NodeName = nodeName
      });
    }

    [UsedImplicitly]
    private void OnDispatcherAssigned(DispatcherAssignedEvent @event)
    {
      var assignation = _state.DispatcherAssignations.First(x => x.DispatcherName == @event.DispatcherName);
      if (assignation != null)
      {
        assignation.NodeName = @event.NodeName;
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Dispatchable
    public void AddDispatchable(string name, string parentSystemName)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Dispatchable)))
      {
        throw new ArgumentException(String.Format("A Dispatchable named {0} already exists.", name));
      }
      if (!string.IsNullOrEmpty(parentSystemName)
          && _state.SystemElements.OfType<SystemGroup>().All(system => system.Name != parentSystemName))
      {
        throw new ArgumentException(String.Format("Parent System named {0} does not exist.", parentSystemName));
      }

      Apply(new DispatchableAddedEvent
      {
        Name = name,
        ParentSystemName = parentSystemName
      });
    }

    [UsedImplicitly]
    private void OnDispatchableAdded(DispatchableAddedEvent @event)
    {
      var dispatchable = new Dispatchable
      {
        Name = @event.Name,
        ParentSystemName = @event.ParentSystemName,
      };
      _state.SystemElements.Add(dispatchable);
      _state.SystemElementsAddedSinceLastCommit.Add(dispatchable);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveDispatchable(string name)
    {
      var toRemove = _state.SystemElements.OfType<Dispatchable>().FirstOrDefault(dispatchable => dispatchable.Name == name);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Dispatchable named {0} does not exists.", name));
      }

      Apply(new DispatchableRemovedEvent
      {
        Name = toRemove.Name
      });
    }

    [UsedImplicitly]
    private void OnDispatchableRemoved(DispatchableRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Dispatchable>().FirstOrDefault(dispatchable => dispatchable.Name == @event.Name);
      if (toRemove != null)
      {
        RemoveSystemElement(toRemove);

        // Remove Dispatchable Assignation of the Dispatchable being removed
        var assignations = _state.DispatchableAssignations.Where(x => x.DispatchableName == toRemove.Name).ToList();
        foreach (var dispatchableAssignation in assignations)
        {
          _state.DispatchableAssignations.Remove(dispatchableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignDispatchableToDispatcher(string name, string dispatcherName)
    {
      if (_state.SystemElements.OfType<Dispatchable>().All(element => element.Name != name))
      {
        throw new ArgumentException(String.Format("Dispatchable named {0} does not exist.", name));
      }
      if (_state.SystemElements.OfType<Dispatcher>().All(dispatcher => dispatcher.Name != dispatcherName))
      {
        throw new ArgumentException(String.Format("Dispatcher named {0} does not exist.", dispatcherName));
      }

      Apply(new DispatchableAssignedEvent
      {
        DispatchableName = name,
        DispatcherName = dispatcherName,
      });
    }

    [UsedImplicitly]
    private void OnDispatchableAssigned(DispatchableAssignedEvent @event)
    {
      var assignation =
        _state.DispatchableAssignations.FirstOrDefault(x => x.DispatchableName == @event.DispatchableName);
      if (assignation == null)
      {
        assignation = new DispatchableAssignation
        {
          DispatchableName = @event.DispatchableName,
        };
        _state.DispatchableAssignations.Add(assignation);
      }
      assignation.DispatcherName = @event.DispatcherName;
      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Utilities
    private void RemoveSystemElement(SystemElement elementToRemove)
    {
      _state.SystemElements.Remove(elementToRemove);

      var newlyAddedSystemElement =
        _state.SystemElementsAddedSinceLastCommit.FirstOrDefault(system => system.Name == elementToRemove.Name);
      if (newlyAddedSystemElement != null)
      {
        _state.SystemElementsAddedSinceLastCommit.Remove(newlyAddedSystemElement);
      }
      else
      {
        _state.MajorChange = true;
      }
    }
    #endregion
  }
}