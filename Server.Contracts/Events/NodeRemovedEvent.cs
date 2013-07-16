using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class NodeRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}