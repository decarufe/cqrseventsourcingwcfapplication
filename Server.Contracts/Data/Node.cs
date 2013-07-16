using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("Node: {Name} (Parent: {ParentSystemName})")]
  public class Node : SystemEntity
  {
    [DataMember(Order = 0)]
    public IEnumerable<long> Executables { get; set; }

    [DataMember(Order = 1)]
    public IEnumerable<long> Dispatchers { get; set; }
  }
}