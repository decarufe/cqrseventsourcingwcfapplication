using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("System: {Name} (Parent: {ParentSystemName})")]
  [KnownType(typeof(Node))]
  [KnownType(typeof(Executable))]
  public class SystemEntity
  {
    [DataMember(Order = 0)]
    public long Id { get; set; }

    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public long ParentSystemId { get; set; }
  }
}