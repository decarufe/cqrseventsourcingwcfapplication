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
      if (_bus == null) return base.GetEventBus(serviceLocator);
      return _bus;
    }
  }
}
