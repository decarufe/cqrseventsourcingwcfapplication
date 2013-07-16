using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableAssignedEvent : DomainEvent
  {
    public long DispatchableId { get; set; }
    public long DispatcherId { get; set; }
  }
}