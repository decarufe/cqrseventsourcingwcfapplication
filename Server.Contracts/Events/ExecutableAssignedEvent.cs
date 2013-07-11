using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class ExecutableAssignedEvent : DomainEvent
  {
    public string ExecutableName { get; set; }
    public string NodeName { get; set; }
  }
}