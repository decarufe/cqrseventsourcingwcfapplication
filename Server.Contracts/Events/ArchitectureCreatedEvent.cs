using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class ArchitectureCreatedEvent : DomainEvent
  {
    public ArchitectureCreatedEvent(Guid id)
    {
      AggregateRootId = id;
    }

    public override string ToString()
    {
      return String.Format("{0}: {1}", this.GetType(), AggregateRootId);
    }
  }
}