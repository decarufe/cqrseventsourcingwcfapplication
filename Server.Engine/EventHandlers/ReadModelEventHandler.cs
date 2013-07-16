using System;
using System.Collections.Generic;
using System.Linq;
using Server.Contracts;
using Server.Contracts.Data;
using Server.Contracts.Events;
using Server.Contracts.Events.Dispatchables;
using Server.Contracts.Events.Executables;
using SimpleCqrs.Eventing;
using Utils;

namespace Server.Engine.EventHandlers
{
  public class ReadModelEventHandler :
    IHandleDomainEvents<DomainModelCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>,
    IHandleDomainEvents<SystemAddedEvent>,
    IHandleDomainEvents<SystemRemovedEvent>,
    IHandleDomainEvents<NodeAddedEvent>,
    IHandleDomainEvents<NodeRemovedEvent>,
    IHandleDomainEvents<ExecutableAddedEvent>,
    IHandleDomainEvents<ExecutableRemovedEvent>,
    IHandleDomainEvents<ExecutableAssignedEvent>,
    IHandleDomainEvents<VersionCommitedEvent>,
    IHandleDomainEvents<DispatchableAddedEvent>,
    IHandleDomainEvents<DispatchableAssignedEvent>,
    IHandleDomainEvents<DispatchableRemovedEvent>,
    IHandleDomainEvents<DispatcherAddedEvent>,
    IHandleDomainEvents<DispatcherAssignedEvent>,
    IHandleDomainEvents<DispatcherRemovedEvent>
  {
    public void Handle(DispatchableAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Dispatchables.Add(new Dispatchable
      {
        Name = message.Name,
        ParentSystemName = message.ParentSystemName,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.Name, message.ParentSystemName);
    }

    public void Handle(DispatchableAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatchable dispatchable = entity.Dispatchables.First(x => x.Name == message.DispatchableName);
      Dispatcher dispatcher = entity.Dispatchers.First(x => x.Name == message.DispatcherName);
      // Clear previous assignations
      foreach (
        Dispatcher dispatcherItem in entity.Dispatchers.Where(x => x.Dispatchables.Any(y => y == dispatchable.Name)))
      {
        List<string> dispatchableItems = dispatcherItem.Dispatchables.ToList();
        dispatchableItems.Remove(dispatchableItems.First(x => x == dispatchable.Name));
        dispatcherItem.Dispatchables = dispatchableItems;
      }
      dispatchable.Dispatcher = message.DispatcherName;
      List<string> dispatchables = dispatcher.Dispatchables.ToList();
      dispatchables.Add(message.DispatchableName);
      dispatcher.Dispatchables = dispatchables;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.DispatchableName, message.DispatcherName);
    }

    public void Handle(DispatchableRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatchable toRemove = entity.Dispatchables.First(x => x.Name == message.Name);
      entity.Dispatchables.Remove(toRemove);
      foreach (Dispatcher dispatcher in entity.Dispatchers.Where(x => x.Dispatchables.Any(y => y == toRemove.Name)))
      {
        List<string> dispatchers = dispatcher.Dispatchables.ToList();
        dispatchers.Remove(dispatchers.First(x => x == toRemove.Name));
        dispatcher.Dispatchables = dispatchers;
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Name);
    }

    public void Handle(DispatcherAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Dispatchers.Add(new Dispatcher
      {
        Name = message.Name,
        Node = message.NodeName,
        Dispatchables = new List<string>(),
      });
      var node = entity.Nodes.First(x => x.Name == message.NodeName);
      var dispatchers = node.Dispatchers.ToList();
      dispatchers.Add(message.Name);
      node.Dispatchers = dispatchers;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.Name, message.NodeName);
    }

    public void Handle(DispatcherAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatcher dispatcher = entity.Dispatchers.First(x => x.Name == message.DispatcherName);
      Node node = entity.Nodes.First(x => x.Name == message.NodeName);
      // Clear previous assignations
      foreach (Node nodeItem in entity.Nodes.Where(x => x.Dispatchers.Any(y => y == dispatcher.Name)))
      {
        List<string> dispatcherItems = nodeItem.Dispatchers.ToList();
        dispatcherItems.Remove(dispatcherItems.First(x => x == dispatcher.Name));
        nodeItem.Dispatchers = dispatcherItems;
      }
      dispatcher.Node = message.NodeName;
      List<string> dispatchers = node.Dispatchers.ToList();
      dispatchers.Add(message.DispatcherName);
      node.Dispatchers = dispatchers;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.DispatcherName, message.NodeName);
    }

    public void Handle(DispatcherRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatcher toRemove = entity.Dispatchers.First(x => x.Name == message.Name);
      entity.Dispatchers.Remove(toRemove);
      foreach (Node node in entity.Nodes.Where(x => x.Dispatchers.Any(y => y == toRemove.Name)))
      {
        List<string> dispatchers = node.Dispatchers.ToList();
        dispatchers.Remove(dispatchers.First(x => x == toRemove.Name));
        node.Dispatchers = dispatchers;
      }
      foreach (Dispatchable dispatchable in entity.Dispatchables.Where(x => x.Dispatcher == toRemove.Name))
      {
        dispatchable.Dispatcher = string.Empty;
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Name);
    }

    public void Handle(DomainModelCreatedEvent message)
    {
      Console.WriteLine(Resource.StringFormat_Var0__Var1, message.GetType().Name, message.AggregateRootId);
      var entity = new ReadModelEntity
      {
        Id = message.AggregateRootId.ToString(),
        DomainModelId = message.AggregateRootId,
        LastEventSequence = message.Sequence,
        Systems = new List<SystemEntity>(),
        Nodes = new List<Node>(),
        Executables = new List<Executable>(),
        Dispatchers = new List<Dispatcher>(),
        Dispatchables = new List<Dispatchable>(),
      };
      Persistance<ReadModelEntity>.Instance.Add(entity);
      UpdateLastEvent(message);
    }

    public void Handle(ExecutableAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Executables.Add(new Executable
      {
        Name = message.Name,
        ParentSystemName = message.ParentSystemName,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.Name, message.ParentSystemName);
    }

    public void Handle(ExecutableAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Executable executable = entity.Executables.First(x => x.Name == message.ExecutableName);
      Node node = entity.Nodes.First(x => x.Name == message.NodeName);
      // Clear previous assignations
      foreach (Node nodeItem in entity.Nodes.Where(x => x.Executables.Any(y => y == executable.Name)))
      {
        List<string> executableItems = nodeItem.Executables.ToList();
        executableItems.Remove(executableItems.First(x => x == executable.Name));
        nodeItem.Executables = executableItems;
      }
      executable.Node = message.NodeName;
      List<string> executables = node.Executables.ToList();
      executables.Add(message.ExecutableName);
      node.Executables = executables;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.ExecutableName, message.NodeName);
    }

    public void Handle(ExecutableRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Executable toRemove = entity.Executables.First(x => x.Name == message.Name);
      entity.Executables.Remove(toRemove);
      foreach (Node node in entity.Nodes.Where(x => x.Executables.Any(y => y == toRemove.Name)))
      {
        List<string> executables = node.Executables.ToList();
        executables.Remove(executables.First(x => x == toRemove.Name));
        node.Executables = executables;
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Name);
    }

    public void Handle(NameChangedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Name = message.NewName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.NewName);
    }

    public void Handle(NodeAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Nodes.Add(new Node
      {
        Name = message.Name,
        ParentSystemName = message.ParentSystemName,
        Executables = new List<string>(),
        Dispatchers = new List<string>(),
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.Name, message.ParentSystemName);
    }

    public void Handle(NodeRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Node toRemove = entity.Nodes.First(x => x.Name == message.Name);
      entity.Nodes.Remove(toRemove);
      foreach (Executable executable in entity.Executables.Where(x => x.Node == toRemove.Name))
      {
        executable.Node = string.Empty;
      }

      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Name);
    }

    public void Handle(SystemAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Systems.Add(new SystemEntity
      {
        Name = message.Name,
        ParentSystemName = message.ParentSystemName,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.Name, message.ParentSystemName);
    }

    public void Handle(SystemRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      SystemEntity toRemove = entity.Systems.First(x => x.Name == message.Name);
      entity.Systems.Remove(toRemove);
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Name);
    }

    public void Handle(VersionCommitedEvent message)
    {
      ReadModelEntity masterEntity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());

      var versionEntity = new ReadModelEntity
      {
        Id = Guid.NewGuid().ToString(),
        DomainModelId = masterEntity.DomainModelId,
        LastEventSequence = masterEntity.LastEventSequence,
        Name = masterEntity.Name,
        Version = Version.Parse(message.NewVersion),
        Systems = masterEntity.Systems,
        Nodes = masterEntity.Nodes,
        Executables = masterEntity.Executables,
        Dispatchables = masterEntity.Dispatchables,
        Dispatchers = masterEntity.Dispatchers,
      };
      Persistance<ReadModelEntity>.Instance.Add(versionEntity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.NewVersion);
    }

    private void UpdateLastEvent(DomainEvent message)
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof (ReadModelEntity).FullName);
      if (message == null || readModelInfo == null)
      {
        if (readModelInfo == null)
        {
          Persistance<ReadModelInfo>.Instance.Add(new ReadModelInfo(typeof (ReadModelEntity))
          {
            LastEvent = DateTime.MinValue
          });
        }
        return;
      }
      if (message.EventDate.ToUniversalTime() > readModelInfo.LastEvent)
      {
        readModelInfo.LastEvent = message.EventDate.ToUniversalTime();
        Persistance<ReadModelInfo>.Instance.Update(readModelInfo);
      }
    }
  }
}