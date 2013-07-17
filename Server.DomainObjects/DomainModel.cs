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
      IdCounter = 1,
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
      public long IdCounter { get; set; }
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
    public void AddSystem(string name, long parentSystemId)
    {
      if (_state.SystemElements.Any(system => system.Name == name && system.GetType() == typeof(SystemGroup)))
      {
        throw new ArgumentException(String.Format("A System named {0} already exists.", name));
      }
      if (parentSystemId != 0
        && _state.SystemElements.OfType<SystemGroup>().All(system => system.Id != parentSystemId))
      {
        throw new ArgumentException(String.Format("Parent System {0} does not exist.", parentSystemId));
      }

      Apply(new SystemAddedEvent
      {
        Id = _state.IdCounter,
        Name = name,
        ParentSystemId = parentSystemId
      });
    }

    [UsedImplicitly]
    private void OnSystemAdded(SystemAddedEvent @event)
    {
      var newSystem = new SystemGroup
      {
        Id = @event.Id,
        Name = @event.Name,
        ParentSystemId = @event.ParentSystemId,
      };
      _state.IdCounter++;
      _state.SystemElements.Add(newSystem);
      _state.SystemElementsAddedSinceLastCommit.Add(newSystem);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveSystem(long id)
    {
      var systemToRemove = _state.SystemElements.OfType<SystemGroup>().FirstOrDefault(system => system.Id == id);
      if (systemToRemove == null)
      {
        throw new ArgumentException(String.Format("The System {0} does not exists.", id));
      }

      IEnumerable<SystemElement> systemsToRemove = ListSystemAndChildren(systemToRemove);
      foreach (var systemElement in systemsToRemove)
      {
        // Todo implement visitors
        if (systemElement.GetType() == typeof (SystemGroup))
        {
          Apply(new SystemRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
        else if (systemElement.GetType() == typeof(Node))
        {
          Apply(new NodeRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
        else if (systemElement.GetType() == typeof(Executable))
        {
          Apply(new ExecutableRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
        else if (systemElement.GetType() == typeof(Dispatchable))
        {
          Apply(new DispatchableRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
        else if (systemElement.GetType() == typeof(Dispatcher))
        {
          Apply(new DispatcherRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
        else if (systemElement as SplSystemElement != null)
        {
          Apply(new SplSystemElementRemovedEvent
          {
            Id = systemElement.Id,
          });
        }
      }

    }

    private IEnumerable<SystemElement> ListSystemAndChildren(SystemElement systemElement)
    {
      var list = new List<SystemElement>();
      foreach (var element in _state.SystemElements.Where(system => system.ParentSystemId == systemElement.Id))
      {
        list.AddRange(ListSystemAndChildren(element));
      }
      list.Add(systemElement);

      return list;
    }

    [UsedImplicitly]
    private void OnSystemRemoved(SystemRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<SystemGroup>().FirstOrDefault(system => system.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Node
    public void AddNode(string name, long parentSystemId)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Node)))
      {
        throw new ArgumentException(String.Format("A Node named {0} already exists.", name));
      }
      if (_state.SystemElements.OfType<SystemGroup>().All(system => system.Id != parentSystemId))
      {
        throw new ArgumentException(String.Format("Parent System {0} does not exist.", parentSystemId));
      }

      Apply(new NodeAddedEvent
      {
        Id = _state.IdCounter,
        Name = name,
        ParentSystemId = parentSystemId
      });
    }

    [UsedImplicitly]
    private void OnNodeAdded(NodeAddedEvent @event)
    {
      var node = new Node
      {
        Id = @event.Id,
        Name = @event.Name,
        ParentSystemId = @event.ParentSystemId,
      };
      _state.IdCounter++;
      _state.SystemElements.Add(node);
      _state.SystemElementsAddedSinceLastCommit.Add(node);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveNode(long id)
    {
      var toRemove = _state.SystemElements.OfType<Node>().FirstOrDefault(node => node.Id == id);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Node {0} does not exists.", id));
      }

      Apply(new NodeRemovedEvent
      {
        Id = id
      });
    }

    [UsedImplicitly]
    private void OnNodeRemoved(NodeRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Node>().FirstOrDefault(system => system.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);

        // Remove Exec Assignations on the Node Removed
        var assignations = _state.ExecutableAssignations.Where(x => x.NodeId == toRemove.Id).ToList();
        foreach (var executableAssignation in assignations)
        {
          _state.ExecutableAssignations.Remove(executableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Executable
    public void AddExecutable(string name, long parentSystemId)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Executable)))
      {
        throw new ArgumentException(String.Format("A Executable named {0} already exists.", name));
      }
      if (_state.SystemElements.OfType<SystemGroup>().All(system => system.Id != parentSystemId))
      {
        throw new ArgumentException(String.Format("Parent System {0} does not exist.", parentSystemId));
      }

      Apply(new ExecutableAddedEvent
      {
        Id = _state.IdCounter,
        Name = name,
        ParentSystemId = parentSystemId
      });
    }

    [UsedImplicitly]
    private void OnExecutableAdded(ExecutableAddedEvent @event)
    {
      var executable = new Executable
      {
        Id = @event.Id,
        Name = @event.Name,
        ParentSystemId = @event.ParentSystemId,
        Type = "Executable",
      };
      _state.IdCounter++;
      _state.SystemElements.Add(executable);
      _state.SystemElementsAddedSinceLastCommit.Add(executable);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveExecutable(long id)
    {
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(executable => executable.Id == id);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Executable {0} does not exists.", id));
      }

      Apply(new ExecutableRemovedEvent
      {
        Id = id
      });
    }

    [UsedImplicitly]
    private void OnExecutableRemoved(ExecutableRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Executable>().FirstOrDefault(executable => executable.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);

        // Remove Node Assignation of the Executable being removed
        var assignations = _state.ExecutableAssignations.Where(x => x.ExecutableId == @event.Id).ToList();
        foreach (var executableAssignation in assignations)
        {
          _state.ExecutableAssignations.Remove(executableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignExecutableToNode(long executableId, long nodeId)
    {
      if (!_state.SystemElements.Any(element => element.Id == executableId && element.GetType() == typeof(Executable)))
      {
        throw new ArgumentException(String.Format("The Executable {0} does not exist.", executableId));
      }
      if (!_state.SystemElements.Any(element => element.Id == nodeId && element.GetType() == typeof(Node)))
      {
        throw new ArgumentException(String.Format("The Node {0} does not exist.", nodeId));
      }

      Apply(new ExecutableAssignedEvent
      {
        ExecutableId = executableId,
        NodeId = nodeId,
      });

    }

    [UsedImplicitly]
    private void OnExecutableAssigned(ExecutableAssignedEvent @event)
    {
      var assignation = _state.ExecutableAssignations.FirstOrDefault(x => x.ExecutableId == @event.ExecutableId);
      if (assignation == null)
      {
        assignation = new ExecutableAssignation
        {
          ExecutableId = @event.ExecutableId,
          NodeId = @event.NodeId,
        };
        _state.ExecutableAssignations.Add(assignation);
      }
      else
      {
        assignation.NodeId = @event.NodeId;
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
    public void AddDispatcher(string name, long nodeId)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Dispatcher)))
      {
        throw new ArgumentException(String.Format("A Dispatcher named {0} already exists.", name));
      }
      if (_state.SystemElements.OfType<Node>().All(system => system.Id != nodeId))
      {
        throw new ArgumentException(String.Format("Node {0} does not exist.", nodeId));
      }

      Apply(new DispatcherAddedEvent
      {
        Id = _state.IdCounter,
        Name = name,
        NodeId = nodeId
      });
    }

    [UsedImplicitly]
    private void OnDispatcherAdded(DispatcherAddedEvent @event)
    {
      var dispatcher = new Dispatcher
      {
        Id = @event.Id,
        Name = @event.Name,
      };
      _state.IdCounter++;
      _state.SystemElements.Add(dispatcher);
      _state.SystemElementsAddedSinceLastCommit.Add(dispatcher);
      _state.DispatcherAssignations.Add(new DispatcherAssignation
      {
        DispatcherId = @event.Id,
        NodeId = @event.NodeId,
      });
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveDispatcher(long id)
    {
      var toRemove = _state.SystemElements.OfType<Dispatcher>().FirstOrDefault(dispatchable => dispatchable.Id == id);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Dispatcher {0} does not exists.", id));
      }

      Apply(new DispatcherRemovedEvent
      {
        Id = toRemove.Id
      });
    }

    [UsedImplicitly]
    private void OnDispatcherRemoved(DispatcherRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Dispatcher>().FirstOrDefault(dispatcher => dispatcher.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);

        // Remove all Dispatchable Assignations on the Dispatcher being removed
        var disaptchableAssignations = _state.DispatchableAssignations.Where(x => x.DispatcherId == @event.Id).ToList();
        foreach (var disaptchableAssignation in disaptchableAssignations)
        {
          _state.DispatchableAssignations.Remove(disaptchableAssignation);
        }

        // Remove Node Assignation of the Dispatcher being removed
        var assignations = _state.DispatcherAssignations.Where(x => x.DispatcherId == @event.Id).ToList();
        foreach (var dispatcherAssignation in assignations)
        {
          _state.DispatcherAssignations.Remove(dispatcherAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignDispatcherToNode(long dispatcherId, long nodeId)
    {
      if (_state.SystemElements.OfType<Dispatcher>().All(element => element.Id != dispatcherId))
      {
        throw new ArgumentException(String.Format("Dispatcher {0} does not exist.", dispatcherId));
      }
      if (_state.SystemElements.OfType<Node>().All(system => system.Id != nodeId))
      {
        throw new ArgumentException(String.Format("Node {0} does not exist.", nodeId));
      }
      if (_state.DispatcherAssignations.All(dispatcher => dispatcher.DispatcherId != dispatcherId))
      {
        throw new ArgumentException(String.Format("Dispatcher {0} Assignation does not exist.", dispatcherId));
      }

      Apply(new DispatcherAssignedEvent
      {
        DispatcherId = dispatcherId,
        NodeId = nodeId
      });
    }

    [UsedImplicitly]
    private void OnDispatcherAssigned(DispatcherAssignedEvent @event)
    {
      var assignation = _state.DispatcherAssignations.First(x => x.DispatcherId == @event.DispatcherId);
      if (assignation != null)
      {
        assignation.NodeId = @event.NodeId;
      }

      ComputeSnapshotRequirements(@event);
    }
    #endregion

    #region Dispatchable
    public void AddDispatchable(string name, long parentSystemId)
    {
      if (_state.SystemElements.Any(element => element.Name == name && element.GetType() == typeof(Dispatchable)))
      {
        throw new ArgumentException(String.Format("A Dispatchable named {0} already exists.", name));
      }
      if (_state.SystemElements.OfType<SystemGroup>().All(system => system.Id != parentSystemId))
      {
        throw new ArgumentException(String.Format("Parent System {0} does not exist.", parentSystemId));
      }

      Apply(new DispatchableAddedEvent
      {
        Id = _state.IdCounter,
        Name = name,
        ParentSystemId = parentSystemId
      });
    }

    [UsedImplicitly]
    private void OnDispatchableAdded(DispatchableAddedEvent @event)
    {
      var dispatchable = new Dispatchable
      {
        Id = @event.Id,
        Name = @event.Name,
        ParentSystemId = @event.ParentSystemId,
        Type = "Dispatchable",
      };
      _state.IdCounter++;
      _state.SystemElements.Add(dispatchable);
      _state.SystemElementsAddedSinceLastCommit.Add(dispatchable);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveDispatchable(long id)
    {
      var toRemove = _state.SystemElements.OfType<Dispatchable>().FirstOrDefault(dispatchable => dispatchable.Id == id);
      if (toRemove == null)
      {
        throw new ArgumentException(String.Format("The Dispatchable {0} does not exists.", id));
      }

      Apply(new DispatchableRemovedEvent
      {
        Id = id
      });
    }

    [UsedImplicitly]
    private void OnDispatchableRemoved(DispatchableRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<Dispatchable>().FirstOrDefault(dispatchable => dispatchable.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);

        // Remove Dispatchable Assignation of the Dispatchable being removed
        var assignations = _state.DispatchableAssignations.Where(x => x.DispatchableId == toRemove.Id).ToList();
        foreach (var dispatchableAssignation in assignations)
        {
          _state.DispatchableAssignations.Remove(dispatchableAssignation);
        }
      }

      ComputeSnapshotRequirements(@event);
    }

    public void AssignDispatchableToDispatcher(long dispatchableId, long dispatcherId)
    {
      if (_state.SystemElements.OfType<Dispatchable>().All(element => element.Id != dispatchableId))
      {
        throw new ArgumentException(String.Format("Dispatchable {0} does not exist.", dispatchableId));
      }
      if (_state.SystemElements.OfType<Dispatcher>().All(dispatcher => dispatcher.Id != dispatcherId))
      {
        throw new ArgumentException(String.Format("Dispatcher {0} does not exist.", dispatcherId));
      }

      Apply(new DispatchableAssignedEvent
      {
        DispatchableId = dispatchableId,
        DispatcherId = dispatcherId,
      });
    }

    [UsedImplicitly]
    private void OnDispatchableAssigned(DispatchableAssignedEvent @event)
    {
      var assignation =
        _state.DispatchableAssignations.FirstOrDefault(x => x.DispatchableId == @event.DispatchableId);
      if (assignation == null)
      {
        assignation = new DispatchableAssignation
        {
          DispatchableId = @event.DispatchableId,
        };
        _state.DispatchableAssignations.Add(assignation);
      }
      assignation.DispatcherId = @event.DispatcherId;
      ComputeSnapshotRequirements(@event);
    }
    #endregion

    public void AssignSplAsset(long splElementId, string assetName)
    {
      var splElement =
        _state.SystemElements.OfType<SplSystemElement>().FirstOrDefault(element => element.Id == splElementId);
      if (splElement == null)
      {
        throw new ArgumentException(String.Format("Spl Element {0} does not exist.", splElementId));
      }

      Apply(new SplAssetAssignedEvent
      {
        SplElementId = splElementId,
        SplElementName = splElement.Name,
        ElementType = splElement.Type,
        AssetName = assetName,
      });
    }

    [UsedImplicitly]
    private void OnSplAssetAssigned(SplAssetAssignedEvent @event)
    {
      var splAsset = _state.SystemElements.OfType<SplSystemElement>().First(x => x.Id == @event.SplElementId);
      splAsset.AssetName = @event.AssetName;
      ComputeSnapshotRequirements(@event);
    }

    public void AddSystemElement(string name, string type, long parentSystemId)
    {
      switch (type)
      {
        case "System":
          AddSystem(name, parentSystemId);
          break;
        case "Node":
          AddNode(name, parentSystemId);
          break;
        case "Executable":
          AddExecutable(name, parentSystemId);
          break;
        case "Dispatchable":
          AddDispatchable(name, parentSystemId);
          break;
        case "Dispatcher":
          AddDispatcher(name, parentSystemId);
          break;
        default:
          if (_state.SystemElements.OfType<SplSystemElement>().Any(element => element.Name == name))
          {
            throw new ArgumentException(String.Format("A System Element named {0} already exists.", name));
          }
          if (_state.SystemElements.OfType<SystemGroup>().All(system => system.Id != parentSystemId))
          {
            throw new ArgumentException(String.Format("Parent System {0} does not exist.", parentSystemId));
          }

          Apply(new SplSystemElementAddedEvent
          {
            Id = _state.IdCounter,
            Name = name,
            ParentSystemId = parentSystemId,
            Type = type,
          });

          break;
      }
    }

    [UsedImplicitly]
    private void OnSplSystemElementAdded(SplSystemElementAddedEvent @event)
    {
      var splSystemElement = new SplSystemElement
      {
        Id = @event.Id,
        Name = @event.Name,
        ParentSystemId = @event.ParentSystemId,
        Type = @event.Type
      };
      _state.IdCounter++;
      _state.SystemElements.Add(splSystemElement);
      _state.SystemElementsAddedSinceLastCommit.Add(splSystemElement);
      ComputeSnapshotRequirements(@event);
    }

    public void RemoveSystemElement(long elementId)
    {
      var systemElement =
        _state.SystemElements.FirstOrDefault(element => element.Id == elementId);
      if (systemElement == null)
      {
        throw new ArgumentException(String.Format("A System Element {0} does not exist.", elementId));
      }

      if (systemElement.GetType() == typeof (SystemGroup))
      {
        RemoveSystem(elementId);
      }
      else if (systemElement.GetType() == typeof(Node))
      {
        RemoveNode(elementId);
      }
      else if (systemElement.GetType() == typeof (Executable))
      {
        RemoveExecutable(elementId);
      }
      else if (systemElement.GetType() == typeof (Dispatchable))
      {
        RemoveDispatchable(elementId);
      }
      else if (systemElement.GetType() == typeof (Dispatcher))
      {
        RemoveDispatcher(elementId);
      }
      else if (systemElement as SplSystemElement != null)
      {
        Apply(new SplSystemElementRemovedEvent
        {
          Id = elementId,
        });
      }
      else
      {
        throw new ArgumentException(String.Format("A System Element {0} is not supported.", elementId));
      }
    }

    [UsedImplicitly]
    private void OnSplSystemElementRemoved(SplSystemElementRemovedEvent @event)
    {
      var toRemove = _state.SystemElements.OfType<SplSystemElement>().FirstOrDefault(splSystemElement => splSystemElement.Id == @event.Id);
      if (toRemove != null)
      {
        RemoveBaseSystemElement(toRemove);
      }

      ComputeSnapshotRequirements(@event);
    }

    #region Utilities

    private void RemoveBaseSystemElement(SystemElement elementToRemove)
    {
      _state.SystemElements.Remove(elementToRemove);

      var newlyAddedSystemElement =
        _state.SystemElementsAddedSinceLastCommit.FirstOrDefault(system => system.Id == elementToRemove.Id);
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