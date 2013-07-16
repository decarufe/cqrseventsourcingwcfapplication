using System;
using System.Collections.Generic;
using System.ServiceModel;
using Server.Contracts.Data;

namespace Server.Contracts
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICqrsService" in both code and config file together.
  [ServiceContract]
  public interface ICqrsService
  {
    [OperationContract]
    void SetName(Guid id, string name);

    [OperationContract]
    long AddSystem(Guid id, string name, long parentSystemId);

    [OperationContract]
    void RemoveSystem(Guid id, long systemId);

    [OperationContract]
    long AddNode(Guid id, string name, long parentSystemId);

    [OperationContract]
    void RemoveNode(Guid id, long nodeId);

    [OperationContract]
    long AddExecutable(Guid id, string name, long parentSystemId);

    [OperationContract]
    void RemoveExecutable(Guid id, long executableId);

    [OperationContract]
    void AssignExecutableToNode(Guid id, long executableId, long nodeId);

    [OperationContract]
    long AddDispatcher(Guid id, string name, long nodeId);

    [OperationContract]
    void RemoveDispatcher(Guid id, long dispatcherId);

    [OperationContract]
    void AssignDispatcherToNode(Guid id, long dispatcherId, long nodeId);

    [OperationContract]
    long AddDispatchable(Guid id, string name, long parentSystemId);

    [OperationContract]
    void RemoveDispatchable(Guid id, long dispatchableId);

    [OperationContract]
    void AssignDispatchableToDispatcher(Guid id, long dispatchableId, long dispatcherId);

    [OperationContract]
    void CommitVersion(Guid id);

    [OperationContract]
    string GetName(Guid id);

    [OperationContract]
    IEnumerable<SystemEntity> GetSystems(Guid id);

    [OperationContract]
    IEnumerable<Node> GetNodes(Guid id);

    [OperationContract]
    IEnumerable<Executable> GetExecutables(Guid id);

    [OperationContract]
    IEnumerable<Dispatcher> GetDispatchers(Guid id);

    [OperationContract]
    IEnumerable<Dispatchable> GetDispatchables(Guid id);

    [OperationContract]
    IEnumerable<DomainModelDto> GetList();

    [OperationContract]
    IEnumerable<DomainModelDto> GetPublishedList();

    [OperationContract]
    void ReloadFromEvents(Uri uri, DateTime lastEvent);

    [OperationContract]
    Pong Ping(Uri sender);
  }
}
