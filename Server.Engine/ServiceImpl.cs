using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using Server.Contracts;
using Server.Engine.Commands;
using Server.ReadModels;
using SimpleCqrs.Commanding;
using Rhino.ServiceBus.Hosting;
using Rhino.ServiceBus.Msmq;

namespace Server.Engine
{
  public class ServiceImpl : ICqrsService
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
      ArchitectureView architectureView = Persistance<ArchitectureView>.Instance.Get(id.ToString());

      return architectureView.Name;
    }

    public IEnumerable<KeyValuePair<Guid, string>> GetList()
    {
      return from a in Persistance<ArchitectureView>.Instance.GetAll()
               select new KeyValuePair<Guid, string>(Guid.Parse(a.Id), a.Name);
    }
  }
}