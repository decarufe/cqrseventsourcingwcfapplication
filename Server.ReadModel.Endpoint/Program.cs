using System;
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
using SimpleCqrs.Rhino.ServiceBus;
using Utils;

namespace Server.ReadModel.Endpoint
{
  class Program
  {
    static void Main(string[] args)
    {
      new MongoRepository.MongoRepositoryManager<Architecture>().Drop();
      var repository = new ArchitectureRepository();

      Thread.Sleep(2000);

      PrepareQueues.Prepare("msmq://localhost/CQRS.ReadModel", QueueType.Standard);

      var host = new DefaultHost();
      host.Start<ReadModelBootStrapper>();

      var runtime = new TrainingRuntime(new RsbEventBus((IServiceBus)host.Bus));
      runtime.Start();

      var container = runtime.ServiceLocator.Container;
      container.RegisterInstance(repository);

      //var eventStore = container.Resolve<IEventStore>();
      //var eventBus = container.Resolve<IEventBus>();

      //Assembly assembly = typeof(ICqrsService).Assembly;
      //var types = from t in assembly.GetTypes()
      //            where t.IsPublic
      //                  && typeof(DomainEvent).IsAssignableFrom(t)
      //            select t;
      //IEnumerable<DomainEvent> events = eventStore.GetEventsByEventTypes(types);

      //eventBus.PublishEvents(events);

      Console.WriteLine("Waiting for messages");
      Console.ReadLine();
    }
  }
}
