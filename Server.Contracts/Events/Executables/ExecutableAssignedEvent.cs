using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Executables
{
  public class ExecutableAssignedEvent : DomainEvent
  {
    public string ExecutableName { get; set; }
    public string NodeName { get; set; }
  }
}