using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoRepository;
using Server.Contracts;

namespace Server.ReadModels
{
  public class Persistance<TEntity> : IPersistance<TEntity>
    where TEntity : IEntity
  {
    private readonly MongoRepository.MongoRepository<TEntity> _table;

    private static IPersistance<TEntity> _persistance;

    private Persistance(MongoRepository<TEntity> table)
    {
      _table = table;
    }

    public static IPersistance<TEntity> Instance
    {
      get
      {
        if (_persistance == null)
        {
          _persistance = new Persistance<TEntity>(new MongoRepository<TEntity>());
        }

        return _persistance;
      }
    }

    public void Add(TEntity architectureView)
    {
      _table.Add(architectureView);
    }

    public void Update(TEntity architecture)
    {
      _table.Update(architecture);
    }

    public TEntity Get(string id)
    {
      return _table.GetById(id);
    }

    public IEnumerable<TEntity> GetAll()
    {
      return _table.AsEnumerable();
    }
  }
}