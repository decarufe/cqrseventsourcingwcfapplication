using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NameChangedEvent : DomainEvent
  {
    public string NewName { get; set; }

    public override string ToString()
    {
      return String.Format("{0}: {1}", this.GetType(), NewName);
    }
  }
}