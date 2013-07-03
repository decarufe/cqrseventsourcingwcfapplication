using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Contracts;
using Server.Contracts.Events;
using SimpleCqrs;
using SimpleCqrs.Eventing;
using SimpleCqrs.Unity;

namespace Server.Engine
{
  public class TrainingRuntime : SimpleCqrsRuntime<SimpleCqrs.Unity.UnityServiceLocator>
  {
    protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
    {
      //return new MemoryEventStore();

      return new NEventStore();

      //return new SimpleCqrs
      //  .EventStore
      //  .MongoDb
      //  .MongoEventStore(
      //    "mongodb://localhost/",
      //    serviceLocator.Resolve<ITypeCatalog>());
    }

    //protected override ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
    //{
    //  return new SimpleCqrs
    //    .EventStore
    //    .MongoDb
    //    .MongoSnapshotStore(
    //      "mongodb://localhost/",
    //      serviceLocator.Resolve<ITypeCatalog>());
    //}

    protected override void OnStarted(UnityServiceLocator serviceLocator)
    {
      base.OnStarted(serviceLocator);

      var eventStore = serviceLocator.Resolve<IEventStore>();
      var eventBus = serviceLocator.Resolve<IEventBus>();
      Assembly assembly = typeof(IService1).Assembly;
      var types = from t in assembly.GetTypes()
                  where t.IsPublic
                        && typeof(DomainEvent).IsAssignableFrom(t)
                  select t;
      IEnumerable<DomainEvent> events = eventStore.GetEventsByEventTypes(types);
      eventBus.PublishEvents(events);
    }
  }
}