using System.Collections.Generic;
using System.Linq;
using MongoRepository;

namespace Utils
{
  public class Persistance<TEntity>
    where TEntity : IEntity
  {
    private readonly MongoRepository.MongoRepository<TEntity> _table;

    private static Persistance<TEntity> _persistance;

    private Persistance(MongoRepository<TEntity> table)
    {
      _table = table;
    }

    public static Persistance<TEntity> Instance
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

    public void Add(TEntity readModelEntity)
    {
      _table.Add(readModelEntity);
    }

    public void Update(TEntity readModelEntity)
    {
      _table.Update(readModelEntity);
    }

    public TEntity Get(string id)
    {
      return _table.GetById(id);
    }

    public IEnumerable<TEntity> GetAll()
    {
      return Enumerable.AsEnumerable<TEntity>(_table);
    }
  }
}