using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NodeRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
  }
}