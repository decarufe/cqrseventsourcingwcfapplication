using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class SystemRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
  }
}