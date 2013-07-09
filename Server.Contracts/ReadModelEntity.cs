using System;
using System.Runtime.Serialization;
using MongoRepository;

namespace Server.Contracts
{
  [DataContract]
  public class ReadModelEntity : IEntity
  {
    public string Id { get; set; }
    public int LastEventSequence { get; set; }
    public string Name { get; set; }
  }
}