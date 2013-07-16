using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}