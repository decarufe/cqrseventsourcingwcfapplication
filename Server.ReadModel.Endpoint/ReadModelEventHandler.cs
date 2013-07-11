using System;
using System.Threading;
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
    ConsumerOf<PingCalled>
  {
    private readonly IServiceBus _serviceBus;

    public ReadModelEventHandler(IServiceBus serviceBus)
    {
      _serviceBus = serviceBus;
      UpdateLastEvent(null);
    }

    public void Consume(DomainModelCreatedEvent message)
    {
      Console.WriteLine("{0}: {1}", message.GetType().Name, message.AggregateRootId);
      var entity = new ReadModelEntity
      {
        Id = message.AggregateRootId.ToString(),
        LastEventSequence = message.Sequence
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

    private void UpdateLastEvent(DomainEvent message)
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ReadModelEntity).FullName);
      if (message == null)
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
        _serviceBus.Send(new EntityChangedMessage() { Id = message.AggregateRootId });
      }
    }

    public void Consume(PingCalled message)
    {
      Console.WriteLine("Pong");
    }
  }
}