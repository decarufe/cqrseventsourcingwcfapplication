using System;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Engine;
using Server.ReadModel.Endpoint.CqrsServiceReference;
using Utils;

namespace Server.ReadModel.Endpoint
{
  [UsedImplicitly]
  class Program
  {
    static void Main([CanBeNull] string[] args)
    {
      PrepareQueues.Prepare(Resource.MsmqEndpoint, QueueType.Standard);

      try
      {
        var cqrsServiceClient = new CqrsServiceClient();
        cqrsServiceClient.Ping(new Uri(Resource.MsmqEndpoint));
      }
      catch (Exception e)
      {
        throw new InvalidOperationException("Server is not responding", e);
      }

      var host = new DefaultHost();
      host.Start<ReadModelBootStrapper>();

      var runtime = new TrainingRuntime();
      runtime.Start();

      RebuildFromEvents();

      Console.WriteLine("Waiting for messages");
      Console.ReadLine();
    }

    private static void RebuildFromEvents()
    {
      ReadModelInfo readModelInfo = Persistance<ReadModelInfo>.Instance.Get(typeof(ReadModelEntity).FullName);
      DateTime lastEvent = readModelInfo == null ? DateTime.MinValue : readModelInfo.LastEvent;

      Console.Write("Connecting to server...");
      using (var cqrsServiceClient = new CqrsServiceClient())
      {
        Console.WriteLine("Connnected.");
        Console.WriteLine("Reloading events...");
        cqrsServiceClient.ReloadFromEvents(new Uri(Resource.MsmqEndpoint), lastEvent.ToUniversalTime());
        Console.WriteLine("Reloading events completed.");
      }
    }
  }
}
