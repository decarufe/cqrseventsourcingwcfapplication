using System;
using Microsoft.Practices.Unity;
using Server.Contracts;
using Server.Engine.Commands;
using Server.ReadModels;
using SimpleCqrs.Commanding;

namespace Server.Engine
{
  public class ServiceImpl : IService1
  {
    private readonly ICommandBus _commandBus;

    public ServiceImpl(IUnityContainer container)
    {
      _commandBus = container.Resolve<ICommandBus>();
    }

    public void SetName(Guid id, string name)
    {
      _commandBus.Send(new SetNameCommand(id, name));
    }

    public string GetName(Guid id)
    {
      var architectureView = Persistance.Instance.Get(id);
      return architectureView.Name;
    }
  }
}