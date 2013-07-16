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

    public void AddSystem(Guid id, string name, string parentSystemName)
    {
      _commandBus.Send(new AddSystemCommand(id, name, parentSystemName));
    }

    public void RemoveSystem(Guid id, string name)
    {
      _commandBus.Send(new RemoveSystemCommand(id, name));
    }

    public void AddNode(Guid id, string name, string parentSystemName)
    {
      _commandBus.Send(new AddNodeCommand(id, name, parentSystemName));
    }

    public void RemoveNode(Guid id, string name)
    {
      _commandBus.Send(new RemoveNodeCommand(id, name));
    }

    public void AddExecutable(Guid id, string name, string parentSystemName)
    {
      _commandBus.Send(new AddExecutableCommand(id, name, parentSystemName));
    }

    public void RemoveExecutable(Guid id, string name)
    {
      _commandBus.Send(new RemoveExecutableCommand(id, name));
    }

    public void AssignExecutableToNode(Guid id, string executableName, string nodeName)
    {
      _commandBus.Send(new AssignExecutableToNodeCommand(id, executableName, nodeName));
    }

    public void AddDispatcher(Guid id, string name, string nodeName)
    {
      _commandBus.Send(new AddDispatcherCommand(id, name, nodeName));
    }

    public void RemoveDispatcher(Guid id, string name)
    {
      _commandBus.Send(new RemoveDispatcherCommand(id, name));
    }

    public void AssignDispatcherToNode(Guid id, string dispatcherName, string nodeName)
    {
      _commandBus.Send(new AssignDispatcherToNodeCommand(id, dispatcherName, nodeName));
    }

    public void AddDispatchable(Guid id, string name, string parentSystemName)
    {
      _commandBus.Send(new AddDispatchableCommand(id, name, parentSystemName));
    }

    public void RemoveDispatchable(Guid id, string name)
    {
      _commandBus.Send(new RemoveDispatchableCommand(id, name));
    }

    public void AssignDispatchableToDispatcher(Guid id, string dispatchableName, string dispatcherName)
    {
      _commandBus.Send(new AssignDispatchableToDispatcherCommand(id, dispatchableName, dispatcherName));
    }

    public void CommitVersion(Guid id)
    {
      _commandBus.Send(new CommitVersionCommand(id));
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