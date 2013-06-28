using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.ReadModels
{
  public class Persistance : IPersistance
  {
    private Dictionary<Guid, ArchitectureView> _architectureTable = new Dictionary<Guid, ArchitectureView>();

    private static IPersistance _persistance;
    public static IPersistance Instance
    {
      get { return _persistance ?? (_persistance = new Persistance()); }
    }

    public void Save(Guid id, ArchitectureView architectureView)
    {
      if (_architectureTable.ContainsKey(id))
      {
        _architectureTable[id] = architectureView;
      }
      else
      {
        _architectureTable.Add(id,architectureView);
      }
    }

    public ArchitectureView Get(Guid id)
    {
      return _architectureTable[id];
    }
  }
}
