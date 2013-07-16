using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherAddedEvent : DomainEvent
  {
    public string Name { get; set; }
    public string NodeName { get; set; }
  }
}