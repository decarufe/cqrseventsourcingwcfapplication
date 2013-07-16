using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
  }
}