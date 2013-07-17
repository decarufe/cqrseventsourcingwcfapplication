using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("SplElement: {Name} (Parent: {ParentSystemName}), Type: {Type}")]
  public class SplElement : SystemEntity
  {
    [DataMember(Order = 0)]
    public string Type { get; set; }
  }
}