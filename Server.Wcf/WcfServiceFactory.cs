using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;
using Server.Contracts;
using Server.Engine;
using SimpleCqrs.Eventing;
using SimpleCqrs.Rhino.ServiceBus;
using Unity.Wcf;
using Utils;

namespace Server.Wcf
{
  public class WcfServiceFactory : UnityServiceHostFactory
  {
    private DomainModelRuntime _runtime;
    private DefaultHost _host;

    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
      PrepareQueues.Prepare("msmq://localhost/CQRS.Backend", QueueType.Standard);

      _host = new DefaultHost();
      _host.Start<BackendBootStrapper>();

      _runtime = new DomainModelRuntime(new RsbEventBus((IServiceBus)_host.Bus));
      _runtime.Start();

      var unityContainer = _runtime.ServiceLocator.Container;
      ConfigureContainer(unityContainer);
      return new UnityServiceHost(unityContainer, serviceType, baseAddresses);
    }

    protected override void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<ICqrsService, ServiceImpl>();
      container.RegisterInstance((IServiceBus)_host.Bus);
    }
  }
}