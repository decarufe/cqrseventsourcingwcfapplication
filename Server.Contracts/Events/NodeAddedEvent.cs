using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NodeAddedEvent : DomainEvent
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long ParentSystemId { get; set; }
  }
}