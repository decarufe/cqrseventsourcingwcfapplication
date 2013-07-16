using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MongoRepository;

namespace Server.Contracts
{
  public class ReadModelInfo : IEntity
  {
    public ReadModelInfo()
    {
    }

    public ReadModelInfo(Type type)
    {
      Id = type.FullName;
    }

    public string Id { get; set; }
    public DateTime LastEvent { get; set; }
  }
}