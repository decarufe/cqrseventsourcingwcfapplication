using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class SystemRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}