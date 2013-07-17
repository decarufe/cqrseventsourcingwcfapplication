using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Rhino.ServiceBus;
using Server.Contracts;
using Server.Contracts.Data;
using Server.Contracts.Events;
using Server.Contracts.Events.Dispatchables;
using Server.Contracts.Events.Executables;
using SimpleCqrs.Eventing;
using Utils;

namespace Server.ReadModel.Endpoint
{
  [UsedImplicitly]
  public class ReadModelEventHandler :
    ConsumerOf<DomainModelCreatedEvent>,
    ConsumerOf<NameChangedEvent>,
    ConsumerOf<PingCalled>,
    ConsumerOf<SystemAddedEvent>,
    ConsumerOf<SystemRemovedEvent>,
    ConsumerOf<NodeAddedEvent>,
    ConsumerOf<NodeRemovedEvent>,
    ConsumerOf<ExecutableAddedEvent>,
    ConsumerOf<ExecutableRemovedEvent>,
    ConsumerOf<ExecutableAssignedEvent>,
    ConsumerOf<VersionCommitedEvent>,
    ConsumerOf<DispatchableAddedEvent>,
    ConsumerOf<DispatchableAssignedEvent>,
    ConsumerOf<DispatchableRemovedEvent>,
    ConsumerOf<DispatcherAddedEvent>,
    ConsumerOf<DispatcherAssignedEvent>,
    ConsumerOf<DispatcherRemovedEvent>,
    ConsumerOf<SplAssetAssignedEvent>,
    ConsumerOf<SplSystemElementAddedEvent>,
    ConsumerOf<SplSystemElementRemovedEvent>
  {
    public ReadModelEventHandler()
    {
      UpdateLastEvent(null);
    }

    public void Consume(DispatchableAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Dispatchables.Add(new Dispatchable
      {
        Id = message.Id,
        Name = message.Name,
        ParentSystemId = message.ParentSystemId,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.ParentSystemId);
    }

    public void Consume(DispatchableAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatchable dispatchable = entity.Dispatchables.First(x => x.Id == message.DispatchableId);
      Dispatcher dispatcher = entity.Dispatchers.First(x => x.Id == message.DispatcherId);
      // Clear previous assignations
      foreach (
        Dispatcher dispatcherItem in entity.Dispatchers.Where(x => x.Dispatchables.Any(y => y == dispatchable.Id)))
      {
        List<long> dispatchableItems = dispatcherItem.Dispatchables.ToList();
        dispatchableItems.Remove(dispatchableItems.First(x => x == dispatchable.Id));
        dispatcherItem.Dispatchables = dispatchableItems;
      }
      dispatchable.Dispatcher = message.DispatcherId;
      List<long> dispatchables = dispatcher.Dispatchables.ToList();
      dispatchables.Add(message.DispatchableId);
      dispatcher.Dispatchables = dispatchables;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.DispatchableId, message.DispatcherId);
    }

    public void Consume(DispatchableRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatchable toRemove = entity.Dispatchables.First(x => x.Id == message.Id);
      entity.Dispatchables.Remove(toRemove);
      foreach (Dispatcher dispatcher in entity.Dispatchers.Where(x => x.Dispatchables.Any(y => y == message.Id)))
      {
        List<long> dispatchers = dispatcher.Dispatchables.ToList();
        dispatchers.Remove(dispatchers.First(x => x == message.Id));
        dispatcher.Dispatchables = dispatchers;
      }
      var splAsset = entity.SplAssets.FirstOrDefault(x => x.ElementId == message.Id);
      if (splAsset != null)
      {
        entity.SplAssets.Remove(splAsset);
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }

    public void Consume(DispatcherAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Dispatchers.Add(new Dispatcher
      {
        Id = message.Id,
        Name = message.Name,
        Node = message.NodeId,
        Dispatchables = new List<long>(),
      });
      var node = entity.Nodes.First(x => x.Id == message.NodeId);
      var dispatchers = node.Dispatchers.ToList();
      dispatchers.Add(message.Id);
      node.Dispatchers = dispatchers;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.NodeId);
    }

    public void Consume(DispatcherAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatcher dispatcher = entity.Dispatchers.First(x => x.Id == message.DispatcherId);
      Node node = entity.Nodes.First(x => x.Id == message.NodeId);
      // Clear previous assignations
      foreach (Node nodeItem in entity.Nodes.Where(x => x.Dispatchers.Any(y => y == dispatcher.Id)))
      {
        List<long> dispatcherItems = nodeItem.Dispatchers.ToList();
        dispatcherItems.Remove(dispatcherItems.First(x => x == dispatcher.Id));
        nodeItem.Dispatchers = dispatcherItems;
      }
      dispatcher.Node = message.NodeId;
      List<long> dispatchers = node.Dispatchers.ToList();
      dispatchers.Add(message.DispatcherId);
      node.Dispatchers = dispatchers;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.DispatcherId, message.NodeId);
    }

    public void Consume(DispatcherRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Dispatcher toRemove = entity.Dispatchers.First(x => x.Id == message.Id);
      entity.Dispatchers.Remove(toRemove);
      foreach (Node node in entity.Nodes.Where(x => x.Dispatchers.Any(y => y == message.Id)))
      {
        List<long> dispatchers = node.Dispatchers.ToList();
        dispatchers.Remove(dispatchers.First(x => x == message.Id));
        node.Dispatchers = dispatchers;
      }
      foreach (Dispatchable dispatchable in entity.Dispatchables.Where(x => x.Dispatcher == message.Id))
      {
        dispatchable.Dispatcher = 0;
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }

    public void Consume(DomainModelCreatedEvent message)
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
        OtherSplElements = new List<SplElement>(),
        SplAssets = new List<SplAsset>(),
      };
      Persistance<ReadModelEntity>.Instance.Add(entity);
      UpdateLastEvent(message);
    }

    public void Consume(ExecutableAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Executables.Add(new Executable
      {
        Id = message.Id,
        Name = message.Name,
        ParentSystemId = message.ParentSystemId,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.ParentSystemId);
    }

    public void Consume(ExecutableAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Executable executable = entity.Executables.First(x => x.Id == message.ExecutableId);
      Node node = entity.Nodes.First(x => x.Id == message.NodeId);
      // Clear previous assignations
      foreach (Node nodeItem in entity.Nodes.Where(x => x.Executables.Any(y => y == message.ExecutableId)))
      {
        List<long> executableItems = nodeItem.Executables.ToList();
        executableItems.Remove(executableItems.First(x => x == message.ExecutableId));
        nodeItem.Executables = executableItems;
      }
      executable.Node = message.NodeId;
      List<long> executables = node.Executables.ToList();
      executables.Add(message.ExecutableId);
      node.Executables = executables;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId,
                        message.ExecutableId, message.NodeId);
    }

    public void Consume(ExecutableRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Executable toRemove = entity.Executables.First(x => x.Id == message.Id);
      entity.Executables.Remove(toRemove);
      foreach (Node node in entity.Nodes.Where(x => x.Executables.Any(y => y == message.Id)))
      {
        List<long> executables = node.Executables.ToList();
        executables.Remove(executables.First(x => x == message.Id));
        node.Executables = executables;
      }
      var splAsset = entity.SplAssets.FirstOrDefault(x => x.ElementId == message.Id);
      if (splAsset != null)
      {
        entity.SplAssets.Remove(splAsset);
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }

    public void Consume(NameChangedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Name = message.NewName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      var versionnedEntities = Persistance<ReadModelEntity>.Instance.GetAll().Where(x => x.DomainModelId == message.AggregateRootId && x.Id != message.AggregateRootId.ToString());
      foreach (var versionnedEntity in versionnedEntities)
      {
        versionnedEntity.Name = message.NewName;
        Persistance<ReadModelEntity>.Instance.Update(versionnedEntity);
      }

      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.NewName);
    }

    public void Consume(NodeAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Nodes.Add(new Node
      {
        Id = message.Id,
        Name = message.Name,
        ParentSystemId = message.ParentSystemId,
        Executables = new List<long>(),
        Dispatchers = new List<long>(),
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.ParentSystemId);
    }

    public void Consume(NodeRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      Node toRemove = entity.Nodes.First(x => x.Id == message.Id);
      entity.Nodes.Remove(toRemove);
      foreach (Executable executable in entity.Executables.Where(x => x.Id == message.Id))
      {
        executable.Node = 0;
      }

      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }

    public void Consume(PingCalled message)
    {
      Console.WriteLine(Resource.Pong);
    }

    public void Consume(SystemAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Systems.Add(new SystemEntity
      {
        Id = message.Id,
        Name = message.Name,
        ParentSystemId = message.ParentSystemId,
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.ParentSystemId);
    }

    public void Consume(SystemRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      SystemEntity toRemove = entity.Systems.First(x => x.Id == message.Id);
      entity.Systems.Remove(toRemove);
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }

    public void Consume(VersionCommitedEvent message)
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
        OtherSplElements = masterEntity.OtherSplElements,
        SplAssets = masterEntity.SplAssets,
      };
      Persistance<ReadModelEntity>.Instance.Add(versionEntity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.NewVersion);
    }

    private static void UpdateLastEvent(DomainEvent message)
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

    public void Consume(SplAssetAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      SplAsset splAsset = entity.SplAssets.FirstOrDefault(x => x.ElementId == message.SplElementId);
      if (splAsset == null)
      {
        splAsset = new SplAsset
        {
          ElementId =  message.SplElementId,
          ElementName = message.SplElementName,
          ElementType = message.ElementType,
        };
        entity.SplAssets.Add(splAsset);
      }
      splAsset.AssetName = message.AssetName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4__Var5, message.GetType().Name, message.AggregateRootId,
                        message.SplElementId,message.SplElementName, message.ElementType, message.AssetName);
    }

    public void Consume(SplSystemElementAddedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.OtherSplElements.Add(new SplElement
      {
        Id = message.Id,
        Name = message.Name,
        ParentSystemId = message.ParentSystemId,
        Type = message.Type
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3__Var4, message.GetType().Name, message.AggregateRootId,
                        message.Id, message.Name, message.ParentSystemId);
    }

    public void Consume(SplSystemElementRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      SplElement toRemove = entity.OtherSplElements.First(x => x.Id == message.Id);
      entity.OtherSplElements.Remove(toRemove);
      var splAsset = entity.SplAssets.FirstOrDefault(x => x.ElementId == message.Id);
      if (splAsset != null)
      {
        entity.SplAssets.Remove(splAsset);
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId,
                        message.Id);
    }
  }
}