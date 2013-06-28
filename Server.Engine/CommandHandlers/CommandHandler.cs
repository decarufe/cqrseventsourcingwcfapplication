using System;
using Server.Engine.Commands;
using Server.Engine.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace Server.Engine.CommandHandlers
{
  public class CommandHandler :
    IHandleCommands<SetNameCommand>
  {
    private readonly IDomainRepository _repository;

    public CommandHandler(IDomainRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ICommandHandlingContext<SetNameCommand> handlingContext)
    {
      var architecture = _repository.GetById<Architecture>(handlingContext.Command.Id);
      if (architecture == null)
      {
        architecture = new Architecture(handlingContext.Command.Id, handlingContext.Command.Name);
      }
      else
      {
        architecture.ChangeName(handlingContext.Command.Name);
      }
      _repository.Save(architecture);
    }
  }
}