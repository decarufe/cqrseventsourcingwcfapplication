using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Server.Contracts;
using Server.Engine;
using Unity.Wcf;

namespace Server.Wcf
{
  public class WcfServiceFactory : UnityServiceHostFactory
  {
    private TrainingRuntime _runtime;

    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    {
      _runtime = new TrainingRuntime();
      _runtime.Start();
      
      var unityContainer = _runtime.ServiceLocator.Container;
      ConfigureContainer(unityContainer);
      return new UnityServiceHost(unityContainer, serviceType, baseAddresses);
    }

    protected override void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<IService1, ServiceImpl>();
    }
  }
}