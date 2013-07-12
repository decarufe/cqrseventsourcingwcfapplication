using System;
using System.Collections.Generic;
using MongoRepository;
using Server.Contracts.Data;
using System.Runtime.Serialization;

namespace Server.Contracts
{
  [DataContract]
  public class ReadModelEntity : IEntity
  {
    [DataMember]
    public string Id { get; set; }
    [DataMember]
    public Guid DomainModelId { get; set; }
    [IgnoreDataMember]
    public int LastEventSequence { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public Version Version { get; set; }
    [DataMember]
    public List<SystemEntity> Systems { get; set; }
    [DataMember]
    public List<Node> Nodes { get; set; }
    [DataMember]
    public List<Executable> Executables { get; set; }
  }
}