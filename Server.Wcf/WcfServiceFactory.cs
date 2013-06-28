using System;
using System.ServiceModel;
using Microsoft.Practices.Unity;
using Server.Contracts;
using Server.Engine;
using SimpleCqrs.Commanding;
using Unity.Wcf;

namespace Server.Wcf
{
  public class WcfServiceFactory : UnityServiceHostFactory
  {
    protected override void ConfigureContainer(IUnityContainer container)
    {
      container.RegisterType<IService1, ServiceImpl>();
    }
  }
}