using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Server.Contracts;
using Server.Contracts.Data;
using Server.Contracts.Events;
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
    IHandleDomainEvents<VersionCommitedEvent>
  {
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
      };
      Persistance<ReadModelEntity>.Instance.Add(entity);
      UpdateLastEvent(message);
    }

    public void Handle(NameChangedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Name = message.NewName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId, message.NewName);
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
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId, message.Name, message.ParentSystemName);
    }

    public void Handle(SystemRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      var toRemove = entity.Systems.First(x => x.Name == message.Name);
      entity.Systems.Remove(toRemove);
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId, message.Name);
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
      });
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId, message.Name, message.ParentSystemName);
    }

    public void Handle(NodeRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      var toRemove = entity.Nodes.First(x => x.Name == message.Name);
      entity.Nodes.Remove(toRemove);
      foreach (var executable in entity.Executables.Where(x => x.Node == toRemove.Name))
      {
        executable.Node = string.Empty;
      }

      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId, message.Name);
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
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId, message.Name, message.ParentSystemName);
    }

    public void Handle(ExecutableRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      var toRemove = entity.Executables.First(x => x.Name == message.Name);
      entity.Executables.Remove(toRemove);
      foreach (var node in entity.Nodes.Where(x => x.Executables.Any(y => y == toRemove.Name)))
      {
        var executables = node.Executables.ToList();
        executables.Remove(executables.First(x => x == toRemove.Name));
        node.Executables = executables;
      }
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId, message.Name);
    }

    public void Handle(ExecutableAssignedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      var executable = entity.Executables.First(x => x.Name == message.ExecutableName);
      var node = entity.Nodes.First(x => x.Name == message.NodeName);
      // Clear previous assignations
      foreach (var nodeItem in entity.Nodes.Where(x => x.Executables.Any(y => y == executable.Name)))
      {
        var executableItems = nodeItem.Executables.ToList();
        executableItems.Remove(executableItems.First(x => x == executable.Name));
        nodeItem.Executables = executableItems;
      }
      executable.Node = message.NodeName;
      var executables = node.Executables.ToList();
      executables.Add(message.ExecutableName);
      node.Executables = executables;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2__Var3, message.GetType().Name, message.AggregateRootId, message.ExecutableName, message.NodeName);
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
      };
      Persistance<ReadModelEntity>.Instance.Add(versionEntity);
      UpdateLastEvent(message);
      Console.WriteLine(Resource.StringFormat_Var0__Var1__Var2, message.GetType().Name, message.AggregateRootId, message.NewVersion);
    }

    private void UpdateLastEvent(DomainEvent message)
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ReadModelEntity).FullName);
      if (message == null || readModelInfo == null)
      {
        if (readModelInfo == null)
        {
          Persistance<ReadModelInfo>.Instance.Add(new ReadModelInfo(typeof(ReadModelEntity))
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