using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}