using System;
using System.ServiceModel;
using JetBrains.Annotations;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Engine;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using Unity.Wcf;
using Utils;

namespace Server.Backend.Endpoint
{
  [UsedImplicitly]
  internal class Program
  {
    private static DomainModelRuntime _runtime;
    private static DefaultHost _host;

    private static void Main()
    {
      PrepareQueues.Prepare(Resource.MsmqEndpoint, QueueType.Standard);

      Console.Write(Resource.Starting_0_, Resource.service_bus);
      _host = new DefaultHost();
      _host.Start<BackendBootStrapper>();
      Console.WriteLine(Resource.Complete);

      Console.Write(Resource.Starting_0_, Resource.CQRS_Runtime);
      _runtime = new DomainModelRuntime();
      _runtime.Start();
      Console.WriteLine(Resource.Complete);

      var unityContainer = _runtime.ServiceLocator.Container;
      ConfigureContainer(unityContainer);

      ServiceHost serviceHost = null;
      try
      {
        Console.Write(Resource.Starting_0_, Resource.WCF_Service);
        var container = _runtime.ServiceLocator.Resolve<IUnityContainer>();
        serviceHost = new UnityServiceHost(container, typeof (ServiceImpl), new Uri("http://localhost:11998/"));
        serviceHost.Open();
        Console.WriteLine(Resource.Complete);

        Console.WriteLine(Resource.Waiting_for_messages);
        Console.ReadLine();
      }
      finally
      {
        Console.Write(Resource.Closing_0_, Resource.WCF_Service);
        if (serviceHost != null)
        {
          if (serviceHost.State == CommunicationState.Opened) serviceHost.Close();
        }
        Console.WriteLine(Resource.Complete);

        Console.Write(Resource.Closing_0_, Resource.CQRS_Runtime);
        _runtime.Shutdown();
        Console.WriteLine(Resource.Complete);

        Console.Write(Resource.Closing_0_, Resource.service_bus);
        _host.Dispose();
        Console.WriteLine(Resource.Complete);
      }
    }

    private static void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<ICqrsService, ServiceImpl>();
      container.RegisterInstance((IServiceBus) _host.Bus);
    }
  }
}