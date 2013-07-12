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
    public string Name { get; set; }

    [DataMember(Order = 1)]
    public string ParentSystemName { get; set; }
  }
}