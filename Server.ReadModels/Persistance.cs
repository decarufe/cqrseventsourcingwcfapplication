using System;
using System.Collections.Generic;
using Server.Contracts;

namespace Server.ReadModels
{
  public class Persistance : IPersistance
  {
    private readonly Dictionary<Guid, ArchitectureView> _architectureTable = new Dictionary<Guid, ArchitectureView>();

    private static IPersistance _persistance;

    public static IPersistance Instance
    {
      get
      {
        if (_persistance == null)
        {
          _persistance = new Persistance();
        }

        return _persistance;
      }
    }

    public void Save(Guid id, ArchitectureView architectureView)
    {
      if (_architectureTable.ContainsKey(id))
        _architectureTable[id] = architectureView;
      else
        _architectureTable.Add(id, architectureView);
    }

    public ArchitectureView Get(Guid id)
    {
      return _architectureTable[id];
    }

    public IEnumerable<ArchitectureView> GetAll()
    {
      return _architectureTable.Values;
    }
  }
}