using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Contracts.Events;
using Server.Engine;
using SimpleCqrs.Rhino.ServiceBus;
using Unity.Wcf;
using Utils;

namespace Server.Wcf
{
  public class WcfServiceFactory : UnityServiceHostFactory
  {
    private TrainingRuntime _runtime;
    private DefaultHost _host;

    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
      PrepareQueues.Prepare("msmq://localhost/CQRS.Backend", QueueType.Standard);

      _host = new DefaultHost();
      _host.Start<BackendBootStrapper>();

      _runtime = new TrainingRuntime(new RsbEventBus((IServiceBus)_host.Bus));
      _runtime.Start();

      var unityContainer = _runtime.ServiceLocator.Container;
      ConfigureContainer(unityContainer);
      return new UnityServiceHost(unityContainer, serviceType, baseAddresses);
    }

    protected override void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<ICqrsService, ServiceImpl>();
    }
  }
}