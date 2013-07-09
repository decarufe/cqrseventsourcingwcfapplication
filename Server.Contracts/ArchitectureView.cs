using System;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoRepository;

namespace Server.Contracts
{
  [DataContract]
  public class ArchitectureView : IEntity
  {
    public string Id { get; set; }
    public int LastEventSequence { get; set; }
    public string Name { get; set; }
  }
}