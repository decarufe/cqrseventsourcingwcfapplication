using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Server.Engine.Commands;
using Server.Engine.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace Server.Engine.CommandHandlers
{
  [UsedImplicitly]
  public class DomainModelCommandHandler :
    IHandleCommands<SetNameCommand>
  {
    private readonly IDomainRepository _repository;

    public DomainModelCommandHandler(IDomainRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ICommandHandlingContext<SetNameCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.ChangeName(handlingContext.Command.Name);
        else
          domainModel = DomainModel.Create(handlingContext.Command.Id, handlingContext.Command.Name);

        _repository.Save(domainModel);
      }
      catch (AggregateRootNotFoundException e)
      {
        DomainModel.Create(handlingContext.Command.Id, handlingContext.Command.Name);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }
  }
}