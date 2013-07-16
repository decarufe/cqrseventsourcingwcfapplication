using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Executables
{
  public class ExecutableRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}