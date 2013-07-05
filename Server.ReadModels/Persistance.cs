using System;
using System.Collections.Generic;
using System.Linq;
using MongoRepository;
using Server.Contracts;

namespace Server.ReadModels
{
  public class Persistance : IPersistance
  {
    private readonly MongoRepository.MongoRepository<ArchitectureView> _architectureTable;

    private static IPersistance _persistance;

    public Persistance(MongoRepository<ArchitectureView> architectureTable)
    {
      _architectureTable = architectureTable;
    }

    public static IPersistance Instance
    {
      get
      {
        if (_persistance == null)
        {
          _persistance = new Persistance(new MongoRepository<ArchitectureView>());
        }

        return _persistance;
      }
    }

    public void Add(ArchitectureView architectureView)
    {
      _architectureTable.Add(architectureView);
    }

    public void Update(ArchitectureView architecture)
    {
      _architectureTable.Update(architecture);
    }

    public ArchitectureView Get(Guid id)
    {
      return _architectureTable.GetSingle(a => a.AggregateRootId == id);
    }

    public IEnumerable<ArchitectureView> GetAll()
    {
      return _architectureTable.AsEnumerable();
    }
  }
}