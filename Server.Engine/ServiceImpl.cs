using Server.Contracts;
using Server.Engine.Commands;
using SimpleCqrs.Commanding;

namespace Server.Engine
{
  public class ServiceImpl : IService1
  {
    private readonly ICommandBus _commandBus;

    public ServiceImpl(ICommandBus commandBus)
    {
      _commandBus = commandBus;
    }

    public void SetName(string name)
    {
      _commandBus.Send(new SetNameCommand() {Name = name});
    }
  }
}