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
      Architecture architecture;
      try
      {
        architecture = _repository.GetExistingById<Architecture>(handlingContext.Command.Id);
        architecture.ChangeName(handlingContext.Command.Name);
      }
      catch (AggregateRootNotFoundException e)
      {
        architecture = new Architecture(handlingContext.Command.Id, handlingContext.Command.Name);
      }
      _repository.Save(architecture);
    }
  }
}