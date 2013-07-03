using MongoRepository;

namespace Server.ReadModel.Endpoint
{
  public class Architecture : IEntity
  {
    public string Id { get; set; }

    public string Name { get; set; }
  }
}