using System;
using System.Collections.Generic;

namespace Server.Contracts
{
  public interface IPersistance
  {
    void Save(Guid id, ArchitectureView architectureView);
    ArchitectureView Get(Guid id);
    IEnumerable<ArchitectureView> GetAll();
  }
}