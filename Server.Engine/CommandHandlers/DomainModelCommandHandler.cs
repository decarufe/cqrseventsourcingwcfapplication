using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Server.DomainObjects;
using Server.Engine.Commands;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace Server.Engine.CommandHandlers
{
  [UsedImplicitly]
  public class DomainModelCommandHandler :
    IHandleCommands<AddSystemElementCommand>,
    IHandleCommands<AssignDispatchableToDispatcherCommand>,
    IHandleCommands<AssignDispatcherToNodeCommand>,
    IHandleCommands<AssignExecutableToNodeCommand>,
    IHandleCommands<AssignSplAssetCommand>,
    IHandleCommands<CommitVersionCommand>,
    IHandleCommands<RemoveSystemElementCommand>,
    IHandleCommands<SetNameCommand>
  {
    private readonly IDomainRepository _repository;

    public DomainModelCommandHandler(IDomainRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ICommandHandlingContext<AddSystemElementCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddSystemElement(handlingContext.Command.Name, handlingContext.Command.Type,
                                       handlingContext.Command.ParentSystemId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AssignDispatchableToDispatcherCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AssignDispatchableToDispatcher(handlingContext.Command.DispatchableId,
                                                     handlingContext.Command.DispatcherId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AssignDispatcherToNodeCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AssignDispatcherToNode(handlingContext.Command.DispatcherId, handlingContext.Command.NodeId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AssignExecutableToNodeCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AssignExecutableToNode(handlingContext.Command.ExecutableId, handlingContext.Command.NodeId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AssignSplAssetCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AssignSplAsset(handlingContext.Command.SplElementId, handlingContext.Command.AssetName);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<CommitVersionCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.CommitVersion();
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<RemoveSystemElementCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveSystemElement(handlingContext.Command.ElementId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
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
      catch (AggregateRootNotFoundException)
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