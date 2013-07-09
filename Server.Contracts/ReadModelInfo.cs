using System;
using JetBrains.Annotations;
using MongoRepository;

namespace Server.Contracts
{
  [UsedImplicitly]
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