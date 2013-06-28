using System;
using Microsoft.Practices.Unity;
using Server.Contracts;
using Server.Engine.Commands;
using Server.ReadModel;
using SimpleCqrs.Commanding;

namespace Server.Engine
{
  public class ServiceImpl : IService1
  {
    private readonly ICommandBus _commandBus;

    public ServiceImpl(IUnityContainer container)
    {
      _commandBus = container.Resolve<ICommandBus>();
      container.RegisterType<IPersistance, Persistance>(new ContainerControlledLifetimeManager());
    }

    public void SetName(Guid id, string name)
    {
      _commandBus.Send(new SetNameCommand(id, name));
    }

    public string GetName(Guid id)
    {
      return Persistance.Instance.Get(id).Name;
    }
  }
}