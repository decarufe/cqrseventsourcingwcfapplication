using System;
using System.Collections.Generic;
using MongoRepository;

namespace Server.Contracts
{
  public interface IPersistance<TEntity>
    where TEntity : IEntity
  {
    void Add(TEntity architectureView);
    TEntity Get(string id);
    void Update(TEntity architecture);
    IEnumerable<TEntity> GetAll();
  }
}