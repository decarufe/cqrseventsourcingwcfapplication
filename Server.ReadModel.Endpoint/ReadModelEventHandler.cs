using System;
using System.Threading;
using JetBrains.Annotations;
using Rhino.ServiceBus;
using Server.Contracts.Events;

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
    }

    public void Consume(DomainModelCreatedEvent message)
    {
      Console.WriteLine("{0}: {1}", message.GetType().Name, message.AggregateRootId);
    }

    public void Consume(NameChangedEvent message)
    {
      Console.WriteLine("{0}: {1}, {2}", message.GetType().Name, message.AggregateRootId, message.NewName);
    }

    public void Consume(PingCalled message)
    {
      Console.WriteLine("Pong");
    }
  }
}