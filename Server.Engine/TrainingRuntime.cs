using System;
using SimpleCqrs;
using SimpleCqrs.Eventing;

namespace Server.Engine
{
  public class TrainingRuntime : SimpleCqrsRuntime<SimpleCqrs.Unity.UnityServiceLocator>
  {
    protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
    {
      return new MemoryEventStore();
      //return new SimpleCqrs
      //  .EventStore
      //  .MongoDb
      //  .MongoEventStore("mongodb://localhost/",
      //                   serviceLocator.Resolve<ITypeCatalog>());
    }
  }
}