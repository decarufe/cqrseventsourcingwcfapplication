using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NameChangedEvent : DomainEvent
  {
    public string NewName { get; set; }
  }
}