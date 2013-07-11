using System;
using System.Threading;
using Server.Contracts;
using Server.Contracts.Events;
using SimpleCqrs.Eventing;
using Utils;

namespace Server.Engine.EventHandlers
{
  public class ReadModelEventHandler :
    IHandleDomainEvents<DomainModelCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>
  {
    public void Handle(DomainModelCreatedEvent domainEvent)
    {
      var entity = new ReadModelEntity
      {
        Id = domainEvent.AggregateRootId.ToString(),
        LastEventSequence = domainEvent.Sequence
      };
      Persistance<ReadModelEntity>.Instance.Add(entity);
      UpdateLastEvent(domainEvent);
    }

    public void Handle(NameChangedEvent domainEvent)
    {
      ReadModelEntity entity = Persistance<ReadModelEntity>.Instance.Get(domainEvent.AggregateRootId.ToString());
      if (domainEvent.Sequence <= entity.LastEventSequence) return;
      entity.LastEventSequence = domainEvent.Sequence;
      entity.Name = domainEvent.NewName;
      Persistance<ReadModelEntity>.Instance.Update(entity);
      UpdateLastEvent(domainEvent);
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