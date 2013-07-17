using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class SplSystemElementRemovedEvent : DomainEvent
  {
    public long Id { get; set; }
  }
}