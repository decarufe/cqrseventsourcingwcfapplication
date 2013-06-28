using System;

namespace Server.ReadModels
{
  public interface IPersistance
  {
    void Save(Guid id, ArchitectureView architectureView);
    ArchitectureView Get(Guid id);
  }
}