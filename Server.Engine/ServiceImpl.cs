using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Rhino.ServiceBus;
using Server.Contracts;
using Server.Contracts.Data;
using Server.Contracts.Events;
using Server.Engine.Commands;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using Utils;

namespace Server.Engine
{
  public class ServiceImpl : ICqrsService
  {
    private readonly IEventBus _eventBus;
    private readonly ICommandBus _commandBus;
    private readonly IEventStore _eventStore;
    private readonly IServiceBus _serviceBus;
    private static Semaphore _lock = new Semaphore(1, 1);
    private static List<string> _reservedTypes = new List<string> { "System", "Node", "Executable", "Dispatcher", "Dispatchable" }; 

    public ServiceImpl(ICommandBus commandBus, IEventStore eventStore, IServiceBus serviceBus, IEventBus eventBus)
    {
      _commandBus = commandBus;
      _eventStore = eventStore;
      _serviceBus = serviceBus;
      _eventBus = eventBus;

      _lock.WaitOne();

      var readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof (ReadModelEntity).FullName);
      DateTime lastEvent = readModelInfo == null ? DateTime.MinValue : readModelInfo.LastEvent;
      var missingEvents = GetMissingEvents(lastEvent);
      if (missingEvents.Any())
        _eventBus.PublishEvents(missingEvents);

      _lock.Release();
    }

    public void SetName(Guid id, string name)
    {
      _commandBus.Send(new SetNameCommand(id, name));
    }

    public long AddSystem(Guid id, string name, long parentSystemId)
    {
      _commandBus.Execute(new AddSystemElementCommand(id, name, "System", parentSystemId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var system = readModelEntity.Systems.FirstOrDefault(x => x.Name == name);
      if (system != null)
      {
        return system.Id;
      }
      return 0;
    }

    public long AddNode(Guid id, string name, long parentSystemId)
    {
      _commandBus.Send(new AddSystemElementCommand(id, name, "Node", parentSystemId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var node = readModelEntity.Nodes.FirstOrDefault(x => x.Name == name);
      if (node != null)
      {
        return node.Id;
      }
      return 0;
    }

    public void RemoveSystemElement(Guid id, long elementId)
    {
      _commandBus.Send(new RemoveSystemElementCommand(id, elementId));
    }

    public long AddExecutable(Guid id, string name, long parentSystemId)
    {
      _commandBus.Send(new AddSystemElementCommand(id, name, "Executable", parentSystemId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var executable = readModelEntity.Executables.FirstOrDefault(x => x.Name == name);
      if (executable != null)
      {
        return executable.Id;
      }
      return 0;
    }

    public void AssignExecutableToNode(Guid id, long executableId, long nodeId)
    {
      _commandBus.Send(new AssignExecutableToNodeCommand(id, executableId, nodeId));
    }

    public long AddDispatcher(Guid id, string name, long nodeId)
    {
      _commandBus.Send(new AddSystemElementCommand(id, name, "Dispatcher", nodeId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var dispatcher = readModelEntity.Dispatchers.FirstOrDefault(x => x.Name == name);
      if (dispatcher != null)
      {
        return dispatcher.Id;
      }
      return 0;
    }

    public void AssignDispatcherToNode(Guid id, long dispatcherId, long nodeId)
    {
      _commandBus.Send(new AssignDispatcherToNodeCommand(id, dispatcherId, nodeId));
    }

    public long AddDispatchable(Guid id, string name, long parentSystemId)
    {
      _commandBus.Send(new AddSystemElementCommand(id, name, "Dispatchable", parentSystemId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var dispatchable = readModelEntity.Dispatchables.FirstOrDefault(x => x.Name == name);
      if (dispatchable != null)
      {
        return dispatchable.Id;
      }
      return 0;
    }

    public long AddOtherSplElement(Guid id, string name, string type, long parentSystemId)
    {
      if (_reservedTypes.Any(x => x == type))
      {
        throw new ArgumentException(Resource.TypeReservedErrorText, type);
      }

      _commandBus.Send(new AddSystemElementCommand(id, name, type, parentSystemId));

      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());
      var splElement = readModelEntity.OtherSplElements.FirstOrDefault(x => x.Name == name);
      if (splElement != null)
      {
        return splElement.Id;
      }
      return 0;
    }

    public void AssignDispatchableToDispatcher(Guid id, long dispatchableId, long dispatcherId)
    {
      _commandBus.Send(new AssignDispatchableToDispatcherCommand(id, dispatchableId, dispatcherId));
    }

    public void CommitVersion(Guid id)
    {
      _commandBus.Send(new CommitVersionCommand(id));
    }

    public void AssignSplAsset(Guid id, long splElementId, string assetName)
    {
      _commandBus.Send(new AssignSplAssetCommand(id, splElementId, assetName));
    }

    public string GetName(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Name;
    }

    public IEnumerable<SystemEntity> GetSystems(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Systems;
    }

    public IEnumerable<Node> GetNodes(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Nodes;
    }

    public IEnumerable<Executable> GetExecutables(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Executables;
    }

    public IEnumerable<Dispatcher> GetDispatchers(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Dispatchers;
    }

    public IEnumerable<Dispatchable> GetDispatchables(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Dispatchables;
    }

    public IEnumerable<SplAsset> GetSplAssets(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.SplAssets;
    }

    public IEnumerable<DomainModelDto> GetList()
    {
      return from a in Persistance<ReadModelEntity>.Instance.GetAll().OrderBy(x => x.Name).ThenBy(x => x.Version)
             select
               new DomainModelDto
               {
                 Name = a.Name,
                 Version = a.Version != null ? a.Version.ToString() : string.Empty,
                 DomainModelId = a.DomainModelId,
                 ReadModelId = a.Id
               };
    }

    public IEnumerable<DomainModelDto> GetPublishedList()
    {
      return
        from a in
          Persistance<ReadModelEntity>.Instance.GetAll()
                                      .Where(y => y.DomainModelId.ToString() != y.Id)
                                      .OrderBy(x => x.Name)
                                      .ThenBy(x => x.Version)
        select
          new DomainModelDto
          {
            Name = a.Name,
            Version = a.Version != null ? a.Version.ToString() : string.Empty,
            DomainModelId = a.DomainModelId,
            ReadModelId = a.Id
          };
    }

    public Pong Ping(Uri uri)
    {
      _serviceBus.Send(new Endpoint { Uri = uri }, new PingCalled());
      return new Pong();
    }

    public void ReloadFromEvents(Uri uri, DateTime lastEvent)
    {
      var messages = GetMissingEvents(lastEvent);
      var endpoint = new Endpoint
      {
        Uri = uri
      };
// ReSharper disable CoVariantArrayConversion
      if (messages.Any()) _serviceBus.Send(endpoint, messages);
// ReSharper restore CoVariantArrayConversion
    }

    private DomainEvent[] GetMissingEvents(DateTime lastEvent)
    {
      Assembly assembly = typeof (ICqrsService).Assembly;
      var types = from t in assembly.GetTypes()
                  where t.IsPublic
                        && typeof (DomainEvent).IsAssignableFrom(t)
                  select t;

      IEnumerable<DomainEvent> events = _eventStore.GetEventsByEventTypes(types, lastEvent, DateTime.MaxValue);

      DomainEvent[] messages = events.Where(e => e.EventDate > lastEvent).ToArray();
      return messages;
    }
  }
}