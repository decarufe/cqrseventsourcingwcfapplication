using System;
using SimpleCqrs.Eventing;

namespace Server.Engine.Events
{
  internal class ArchitectureCreatedEvent : DomainEvent
  {
    public ArchitectureCreatedEvent(Guid guid)
    {
      AggregateRootId = guid;
    }
  }
}