
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("Executable: {Name} (Parent: {ParentSystemName}), Node: {Node}")]
  public class Executable : SystemEntity
  {
    [DataMember(Order = 0)]
    public string Node { get; set; }
  }
}