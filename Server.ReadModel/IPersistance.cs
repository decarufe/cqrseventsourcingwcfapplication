using System;
using Server.ReadModel.Architecture;

namespace Server.ReadModel
{
  public interface IPersistance
  {
    void Save(Guid id, ArchitectureView architectureView);
    ArchitectureView Get(Guid id);
  }
}