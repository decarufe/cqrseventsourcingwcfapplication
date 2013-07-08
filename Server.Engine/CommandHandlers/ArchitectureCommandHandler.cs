using System;
using Server.Engine.Commands;
using Server.Engine.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace Server.Engine.CommandHandlers
{
  public class ArchitectureCommandHandler :
    IHandleCommands<SetNameCommand>
  {
    private readonly IDomainRepository _repository;
    private readonly ISnapshotStore _snapshotStore;

    public ArchitectureCommandHandler(IDomainRepository repository, ISnapshotStore snapshotStore)
    {
      _repository = repository;
      _snapshotStore = snapshotStore;
    }

    public void Handle(ICommandHandlingContext<SetNameCommand> handlingContext)
    {
      Architecture architecture;
      try
      {
        architecture = _repository.GetById<Architecture>(handlingContext.Command.Id);
        if (architecture == null)
          architecture = Architecture.Create(handlingContext.Command.Id, handlingContext.Command.Name);
        architecture.ChangeName(handlingContext.Command.Name);

        _repository.Save(architecture);
      }
      catch (AggregateRootNotFoundException e)
      {
        architecture = Architecture.Create(handlingContext.Command.Id, handlingContext.Command.Name);
      }
    }
  }
}