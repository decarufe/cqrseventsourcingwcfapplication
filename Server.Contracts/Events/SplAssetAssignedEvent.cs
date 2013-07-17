using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class SplAssetAssignedEvent : DomainEvent
  {
    public long SplElementId { get; set; }
    public string SplElementName { get; set; }
    public string ElementType { get; set; }
    public string AssetName { get; set; }
  }
}