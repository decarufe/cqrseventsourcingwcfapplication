
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts
{
  [DataContract(IsReference = true, Name = "SystemEntity")]
  [DebuggerDisplay("System: {Name} (Parent: {ParentSystemName})")]
  public class SystemEntity
  {
    [DataMember(Order = 0)]
    public string Name { get; set; }

    [DataMember(Order = 1)]
    public string ParentSystemName { get; set; }
  }
}