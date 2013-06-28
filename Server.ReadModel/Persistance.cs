using System;
using System.Collections.Generic;
using Server.ReadModel.Architecture;

namespace Server.ReadModel
{
  public class Persistance : IPersistance
  {
    private readonly Dictionary<Guid, ArchitectureView> _architectureViews = new Dictionary<Guid, ArchitectureView>();

    private static IPersistance _persistance;
    public static IPersistance Instance
    {
      get { return _persistance ?? (_persistance = new Persistance()); }
    }
 
    public void Save(Guid id, ArchitectureView architectureView)
    {
      if (_architectureViews.ContainsKey(id))
      {
        _architectureViews[id] = architectureView;
      }
      else
      {
        _architectureViews.Add(id, architectureView);
      }
    }

    public ArchitectureView Get(Guid id)
    {
      if (_architectureViews.ContainsKey(id))
      {
        return _architectureViews[id];
      }

      throw new Exception("WTF!?");
    }
  }
}
