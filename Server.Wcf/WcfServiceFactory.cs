using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Engine;
using Unity.Wcf;
using Utils;

namespace Server.Wcf
{
  public class WcfServiceFactory : UnityServiceHostFactory
  {
    private TrainingRuntime _runtime;
    private DefaultHost _host;
    private DefaultHost _backend;

    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
      PrepareQueues.Prepare("msmq://localhost/CQRS.Backend", QueueType.Standard);

      _backend = new DefaultHost();
      _backend.BusConfiguration(config =>
      {
        return config
          .Bus("msmq://localhost/CQRS.Backend")
          .Retries(5)
          .Threads(1);
      });
      _backend.Start<BackendBootStrapper>();

      PrepareQueues.Prepare("msmq://localhost/CQRS.Client", QueueType.Standard);

      _host = new DefaultHost();
      _host.Start<ClientBootStrapper>();

      _runtime = new TrainingRuntime(new TrainingEventBus((IServiceBus)_host.Bus));
      _runtime.Start();

      var unityContainer = _runtime.ServiceLocator.Container;
      ConfigureContainer(unityContainer);
      unityContainer.RegisterInstance(_backend, new ContainerControlledLifetimeManager());
      return new UnityServiceHost(unityContainer, serviceType, baseAddresses);
    }

    protected override void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<ICqrsService, ServiceImpl>();
    }
  }
}