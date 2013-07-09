using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Engine;
using Microsoft.Practices.Unity;
using Server.ReadModels;
using SimpleCqrs.Eventing;
using SimpleCqrs.Rhino.ServiceBus;
using Utils;

namespace Server.ReadModel.Endpoint
{
  class Program
  {
    static void Main(string[] args)
    {
      Thread.Sleep(100);

      PrepareQueues.Prepare("msmq://localhost/CQRS.ReadModel", QueueType.Standard);

      var host = new DefaultHost();
      host.Start<ReadModelBootStrapper>();

      var runtime = new TrainingRuntime();
      runtime.Start();

      var container = runtime.ServiceLocator.Container;

      RebuildFromEvents(container);

      Console.WriteLine("Waiting for messages");
      Console.ReadLine();
    }

    private static void RebuildFromEvents(IUnityContainer container)
    {
      var eventStore = container.Resolve<IEventStore>();
      var eventBus = container.Resolve<IEventBus>();

      Assembly assembly = typeof (ICqrsService).Assembly;
      var types = from t in assembly.GetTypes()
                  where t.IsPublic
                        && typeof (DomainEvent).IsAssignableFrom(t)
                  select t;

      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ArchitectureView).FullName);

      IEnumerable<DomainEvent> events;
      DateTime lastEvent;
      if (readModelInfo == null)
      {
        events = eventStore.GetEventsByEventTypes(types);
        lastEvent = DateTime.MinValue;
      }
      else
      {
        events = eventStore.GetEventsByEventTypes(types, readModelInfo.LastEvent, DateTime.MaxValue);
        lastEvent = readModelInfo.LastEvent;
      }

      eventBus.PublishEvents(events.Where(e => e.EventDate > lastEvent));
    }
  }
}
