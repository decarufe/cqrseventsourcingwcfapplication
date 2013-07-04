using System.Collections.Generic;
using System.Linq;
using Rhino.ServiceBus;
using SimpleCqrs.Eventing;

namespace Server.Engine
{
  public class TrainingEventBus : IEventBus
  {
    private readonly IServiceBus _serviceBus;

    public TrainingEventBus(IServiceBus serviceBus)
    {
      _serviceBus = serviceBus;
    }

    public void PublishEvent(DomainEvent domainEvent)
    {
      _serviceBus.Send(new object[1]
      {
        domainEvent
      });
    }

    public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
    {
      _serviceBus.Send((object[])Enumerable.ToArray<DomainEvent>(domainEvents));
    }
  }
}