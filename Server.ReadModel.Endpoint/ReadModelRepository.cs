using JetBrains.Annotations;
using MongoRepository;
using Server.Contracts;

namespace Server.ReadModel.Endpoint
{
  [UsedImplicitly]
  public class ReadModelRepository : MongoRepository<ReadModelEntity>
  {
    
  }
}