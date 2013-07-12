using JetBrains.Annotations;
using MongoRepository;
using Server.Contracts;

namespace Server.Engine
{
  [UsedImplicitly]
  public class ReadModelRepository : MongoRepository<ReadModelEntity>
  {
    
  }
}