using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class ExecutableRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
  }
}