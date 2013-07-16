using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Dispatchables
{
  public class DispatcherAddedEvent : DomainEvent
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long NodeId { get; set; }
  }
}