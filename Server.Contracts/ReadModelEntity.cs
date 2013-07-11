using System;
using System.Collections.Generic;
using MongoRepository;
using Server.Contracts.Data;

namespace Server.Contracts
{
  public class ReadModelEntity : IEntity
  {
    public string Id { get; set; }
    public Guid DomainModelId { get; set; }
    public int LastEventSequence { get; set; }
    public string Name { get; set; }
    public Version Version { get; set; }
    public List<SystemEntity> Systems { get; set; }
    public List<Node> Nodes { get; set; }
    public List<Executable> Executables { get; set; }
  }
}