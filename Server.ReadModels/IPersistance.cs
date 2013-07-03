using System;
using System.Collections.Generic;

namespace Server.ReadModels
{
  public interface IPersistance
  {
    void Save(Guid id, ArchitectureView architectureView);
    ArchitectureView Get(Guid id);
    IEnumerable<ArchitectureView> GetAll();
  }
}