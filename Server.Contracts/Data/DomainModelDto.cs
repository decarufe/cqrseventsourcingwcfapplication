using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("System: {Name} (Parent: {ParentSystemName})")]
  public class DomainModelDto
  {
    [DataMember(Order = 0)]
    public string Name { get; set; }

    [DataMember(Order = 1)]
    public string Version { get; set; }

    [DataMember(Order = 2)]
    public Guid DomainModelId { get; set; }

    [DataMember(Order = 3)]
    public string ReadModelId { get; set; }
  }
}