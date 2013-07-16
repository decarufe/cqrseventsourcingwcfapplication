using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherAssignedEvent : DomainEvent
  {
    public long DispatcherId { get; set; }
    public long NodeId { get; set; }
  }
}