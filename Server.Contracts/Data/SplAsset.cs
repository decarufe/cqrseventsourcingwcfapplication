using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("Element: {ElementName} ({ElementId}), Type: {ElementType}, Asset: {AssetName}")]
  public class SplAsset
  {
    [DataMember(Order = 0)]
    public long ElementId { get; set; }

    [DataMember(Order = 1)]
    public string ElementName { get; set; }

    [DataMember(Order = 2)]
    public string ElementType { get; set; }

    [DataMember(Order = 3)]
    public string AssetName { get; set; }
  }
}