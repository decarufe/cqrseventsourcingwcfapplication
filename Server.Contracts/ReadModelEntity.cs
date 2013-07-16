using System;
using System.Runtime.Serialization;
using MongoRepository;

namespace Server.Contracts
{
  [DataContract]
  public class ReadModelEntity : IEntity
  {
    [DataMember]
    public string Id { get; set; }
    [IgnoreDataMember]
    public int LastEventSequence { get; set; }
    [DataMember]
    public string Name { get; set; }
  }
}