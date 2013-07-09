﻿using System.Collections.Generic;
using Rhino.ServiceBus;
using SimpleCqrs.Eventing;

namespace Server.ReadModel.Endpoint
{
  public class ReadModelEventBus : IEventBus
  {
    private readonly IServiceBus _serviceBus;

    public ReadModelEventBus(IServiceBus serviceBus)
    {
      _serviceBus = serviceBus;
    }

    public void PublishEvent(DomainEvent domainEvent)
    {
      _serviceBus.Publish(new object[]
      {
        domainEvent
      });
    }

    public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
    {
      _serviceBus.Publish(domainEvents);
    }
  }
}