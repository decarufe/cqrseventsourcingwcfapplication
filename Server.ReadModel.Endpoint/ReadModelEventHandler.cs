using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Rhino.ServiceBus;
using Server.Contracts;
using Server.Contracts.Events;
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
    ConsumerOf<VersionCommitedEvent>
  {
    public ReadModelEventHandler()
    {
      UpdateLastEvent(null);
    }

    private static void UpdateLastEvent(DomainEvent message)
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ReadModelEntity).FullName);
      if (message == null)
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

    public void Consume(DomainModelCreatedEvent message)
    {
      Console.WriteLine("{0}: {1}", message.GetType().Name, message.AggregateRootId);
      var entity = new ReadModelEntity
      {
        Id = message.AggregateRootId.ToString(),
        DomainModelId = message.AggregateRootId,
        LastEventSequence = message.Sequence,
        Systems = new List<SystemEntity>(),
      };
      Persistance<ReadModelEntity>.Instance.Add(entity);
      UpdateLastEvent(message);
    }

    public void Consume(NameChangedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      entity.Name = message.NewName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.NewName);
    }

    public void Consume(PingCalled message)
    {
      Console.WriteLine("Pong");
    }

    public void Consume(SystemAddedEvent message)
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
      Console.WriteLine("{0}: {1}, {2}, {3}", message.GetType().Name, message.AggregateRootId, message.Name, message.ParentSystemName);
    }

    public void Consume(SystemRemovedEvent message)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());
      if (message.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = message.Sequence;
      var systemToRemove = entity.Systems.First(system => system.Name == message.Name);
      entity.Systems.Remove(systemToRemove);
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(message);
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.Name);
    }

    public void Consume(VersionCommitedEvent message)
    {
      ReadModelEntity masterEntity = Persistance<ReadModelEntity>.Instance.Get(message.AggregateRootId.ToString());

      var versionEntity = new ReadModelEntity
      {
        Id = Guid.NewGuid().ToString(),
        DomainModelId = masterEntity.DomainModelId,
        Name = masterEntity.Name,
        LastEventSequence = masterEntity.LastEventSequence,
        Systems = masterEntity.Systems,
        Version = Version.Parse(message.NewVersion),
      };
      Persistance<ReadModelEntity>.Instance.Add(versionEntity);
      UpdateLastEvent(message);
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.NewVersion);
    }
  }
}