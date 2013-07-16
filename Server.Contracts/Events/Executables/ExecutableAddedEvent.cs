using SimpleCqrs.Eventing;

namespace Server.Contracts.Events.Executables
{
  public class ExecutableAddedEvent : DomainEvent
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public long ParentSystemId { get; set; }
  }
}