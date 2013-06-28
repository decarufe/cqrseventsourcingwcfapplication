using System;
using System.ServiceModel;

namespace Server.Contracts
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
  [ServiceContract]
  public interface IService1
  {
    [OperationContract]
    void SetName(Guid id, string name);
  }
}
