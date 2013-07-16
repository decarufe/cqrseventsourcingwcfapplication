using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableAddedEvent : DomainEvent
  {
    public string Name { get; set; }
    public string ParentSystemName { get; set; }
  }
}