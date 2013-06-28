using System;
using Server.Engine.Commands;
using Server.Engine.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace Server.Engine.CommandHandlers
{
  public class ArchitectureCommandHandler :
    IHandleCommands<SetNameCommand>
  {
    private readonly IDomainRepository _repository;

    public ArchitectureCommandHandler(IDomainRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ICommandHandlingContext<SetNameCommand> handlingContext)
    {
      var architecture = new Architecture(handlingContext.Command.Id, handlingContext.Command.Name);
      _repository.Save(architecture);
    }
  }
}