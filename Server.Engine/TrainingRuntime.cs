using System;
using System.Linq;
using System.Reflection;
using Rhino.ServiceBus;
using Server.Contracts;
using SimpleCqrs;
using SimpleCqrs.Eventing;
using UnityServiceLocator = SimpleCqrs.Unity.UnityServiceLocator;

namespace Server.Engine
{
  public class TrainingRuntime : SimpleCqrsRuntime<UnityServiceLocator>
  {
    private readonly IEventBus _bus;

    public TrainingRuntime()
    {
    }

    public TrainingRuntime(IEventBus bus)
    {
      _bus = bus;
    }

    protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
    {
      return new NEventStore();
    }

    protected override IEventBus GetEventBus(IServiceLocator serviceLocator)
    {
      return _bus;
    }

    protected override void OnStarted(UnityServiceLocator serviceLocator)
    {
      //base.OnStarted(serviceLocator);

      //var eventStore = serviceLocator.Resolve<IEventStore>();
      //var eventBus = serviceLocator.Resolve<IEventBus>();
      //Assembly assembly = typeof (ICqrsService).Assembly;
      //var types = from t in assembly.GetTypes()
      //            where t.IsPublic
      //                  && typeof (DomainEvent).IsAssignableFrom(t)
      //            select t;
      //var events = eventStore.GetEventsByEventTypes(types).ToArray();
      //if (events.Any())
      //  eventBus.PublishEvents(events);
    }
  }
}
