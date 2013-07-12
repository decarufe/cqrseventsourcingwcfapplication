using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class ExecutableAddedEvent : DomainEvent
  {
    public string Name { get; set; }
    public string ParentSystemName { get; set; }
  }
}