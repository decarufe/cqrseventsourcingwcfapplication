using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Server.Contracts
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICqrsService" in both code and config file together.
  [ServiceContract]
  public interface ICqrsService
  {
    [OperationContract]
    void SetName(Guid id, string name);

    [OperationContract]
    void AddSystem(Guid id, string name, string parentSystemName);

    [OperationContract]
    void RemoveSystem(Guid id, string name);

    [OperationContract]
    void CommitVersion(Guid id);

    [OperationContract]
    string GetName(Guid id);

    [OperationContract]
    IEnumerable<SystemEntity> GetSystems(Guid id);

    [OperationContract]
    IEnumerable<DomainModelDto> GetList();

    [OperationContract]
    void ReloadFromEvents(Uri uri, DateTime lastEvent);

    [OperationContract]
    Pong Ping(Uri sender);
  }
}
