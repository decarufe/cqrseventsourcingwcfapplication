using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Executables
{
  public class ExecutableRemovedEvent : DomainEvent
  {
    public string Name { get; set; }
  }
}