using System;
using System.Collections.Generic;

namespace Server.Contracts
{
  public interface IPersistance
  {
    void Add(ArchitectureView architectureView);
    ArchitectureView Get(Guid id);
    void Update(ArchitectureView architecture);
    IEnumerable<ArchitectureView> GetAll();
  }
}