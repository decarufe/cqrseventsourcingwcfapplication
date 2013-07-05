using System;
using System.Runtime.Serialization;
using MongoRepository;

namespace Server.Contracts
{
  [DataContract]
  public class ArchitectureView : Entity
  {
    public Guid AggregateRootId { get; set; }
    public string Name { get; set; }
  }
}