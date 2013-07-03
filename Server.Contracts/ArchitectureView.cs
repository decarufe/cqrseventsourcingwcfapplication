using System;
using System.Runtime.Serialization;

namespace Server.Contracts
{
  [DataContract]
  public class ArchitectureView
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}