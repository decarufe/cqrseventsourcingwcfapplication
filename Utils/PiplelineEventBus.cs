using System.Collections.Generic;
using SimpleCqrs.Eventing;

namespace Utils
{
  public class PiplelineEventBus : IEventBus
  {
    private readonly IEventBus[] _pipeline;

    public PiplelineEventBus(params IEventBus[] pipeline)
    {
      _pipeline = pipeline;
    }

    public void PublishEvent(DomainEvent domainEvent)
    {
      foreach (var eventBus in _pipeline)
      {
        eventBus.PublishEvent(domainEvent);
      }
    }

    public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
    {
      foreach (var eventBus in _pipeline)
      {
         eventBus.PublishEvents(domainEvents);
      }
    }
  }
}