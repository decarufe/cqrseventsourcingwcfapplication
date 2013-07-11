using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NodeAddedEvent : DomainEvent
  {
    public string Name { get; set; }
    public string ParentSystemName { get; set; }
  }
}