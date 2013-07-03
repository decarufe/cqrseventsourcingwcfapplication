using System;
using System.Linq;
using SimpleCqrs;
using SimpleCqrs.Eventing;

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
  }
}