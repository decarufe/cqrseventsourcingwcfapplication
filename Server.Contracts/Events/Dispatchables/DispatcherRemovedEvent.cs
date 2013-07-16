using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
    public string NodeName { get; set; }
  }
}