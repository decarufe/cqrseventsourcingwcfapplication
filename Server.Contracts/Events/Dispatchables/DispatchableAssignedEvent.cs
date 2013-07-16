using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableAssignedEvent : DomainEvent
  {
    public string DispatchableName { get; set; }
    public string DispatcherName { get; set; }
  }
}