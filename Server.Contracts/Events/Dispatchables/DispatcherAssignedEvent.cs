using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherAssignedEvent : DomainEvent
  {
    public string DispatcherName { get; set; }
    public string NodeName { get; set; }
  }
}