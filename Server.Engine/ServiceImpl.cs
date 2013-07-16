using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Rhino.ServiceBus;
using Server.Contracts;
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

    public string GetName(Guid id)
    {
      ReadModelEntity readModelEntity = Persistance<ReadModelEntity>.Instance.Get(id.ToString());

      return readModelEntity.Name;
    }

    public IEnumerable<ReadModelEntity> GetList()
    {
      return Persistance<ReadModelEntity>.Instance.GetAll();
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