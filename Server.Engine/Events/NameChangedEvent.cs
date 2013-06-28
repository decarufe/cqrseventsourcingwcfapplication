using SimpleCqrs.Eventing;

namespace Server.Engine.Events
{
  public class NameChangedEvent : DomainEvent
  {
    public string NewName { get; set; }
  }
}