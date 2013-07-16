using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Executables
{
  public class ExecutableAssignedEvent : DomainEvent
  {
    public long ExecutableId { get; set; }
    public long NodeId { get; set; }
  }
}