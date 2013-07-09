using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoRepository;

namespace Server.Contracts
{
  [DataContract]
  public class ReadModelEntity : IEntity
  {
    public string Id { get; set; }
    public Guid DomainModelId { get; set; }
    public int LastEventSequence { get; set; }
    public string Name { get; set; }
    public Version Version { get; set; }
    public List<SystemEntity> Systems { get; set; }
  }
}