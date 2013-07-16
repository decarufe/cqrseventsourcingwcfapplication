using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatchableAddedEvent : DomainEvent
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long ParentSystemId { get; set; }
  }
}