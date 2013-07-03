using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using Server.Contracts;
using Server.Engine;
using Microsoft.Practices.Unity;
using SimpleCqrs.Eventing;

namespace Server.ReadModel.Endpoint
{
  class Program
  {
    static void Main(string[] args)
    {
      new MongoRepository.MongoRepositoryManager<Architecture>().Drop();
      var repository = new ArchitectureRepository();
      
      repository.Add(new Architecture()
      {
        Id = Guid.NewGuid().ToString(),
        Name = "Test"
      });

      var runtime = new TrainingRuntime();
      runtime.Start();

      var container = runtime.ServiceLocator.Container;
      container.RegisterInstance<ArchitectureRepository>(repository);

      var eventStore = container.Resolve<IEventStore>();
      var eventBus = container.Resolve<IEventBus>();

      Assembly assembly = typeof(ICqrsService).Assembly;
      var types = from t in assembly.GetTypes()
                  where t.IsPublic
                        && typeof(DomainEvent).IsAssignableFrom(t)
                  select t;
      IEnumerable<DomainEvent> events = eventStore.GetEventsByEventTypes(types);

      eventBus.PublishEvents(events);
    }
  }
}
