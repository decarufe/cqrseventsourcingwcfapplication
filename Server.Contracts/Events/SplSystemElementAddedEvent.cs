using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class SplSystemElementAddedEvent : DomainEvent
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public long ParentSystemId { get; set; }
  }
}