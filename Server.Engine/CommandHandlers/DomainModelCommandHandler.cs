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
    IHandleCommands<SetNameCommand>,
    IHandleCommands<AddSystemCommand>,
    IHandleCommands<RemoveSystemCommand>,
    IHandleCommands<AddNodeCommand>,
    IHandleCommands<RemoveNodeCommand>,
    IHandleCommands<AddExecutableCommand>,
    IHandleCommands<RemoveExecutableCommand>,
    IHandleCommands<AssignExecutableToNodeCommand>,
    IHandleCommands<AddDispatchableCommand>,
    IHandleCommands<RemoveDispatchableCommand>,
    IHandleCommands<AssignDispatchableToDispatcherCommand>,
    IHandleCommands<AddDispatcherCommand>,
    IHandleCommands<RemoveDispatcherCommand>,
    IHandleCommands<AssignDispatcherToNodeCommand>,
    IHandleCommands<CommitVersionCommand>,
    IHandleCommands<AssignSplAssetCommand>
  {
    private readonly IDomainRepository _repository;

    public DomainModelCommandHandler(IDomainRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ICommandHandlingContext<AddDispatchableCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddDispatchable(handlingContext.Command.Name, handlingContext.Command.ParentSystemId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AddDispatcherCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddDispatcher(handlingContext.Command.Name, handlingContext.Command.NodeId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AddExecutableCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddExecutable(handlingContext.Command.Name, handlingContext.Command.ParentSystemId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AddNodeCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddNode(handlingContext.Command.Name, handlingContext.Command.ParentSystemId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<AddSystemCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AddSystem(handlingContext.Command.Name, handlingContext.Command.ParentSystemId);
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
          domainModel.AssignDispatchableToDispatcher(handlingContext.Command.DispatchableId, handlingContext.Command.DispatcherId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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

    public void Handle(ICommandHandlingContext<RemoveDispatchableCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveDispatchable(handlingContext.Command.DispatchableId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<RemoveDispatcherCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveDispatcher(handlingContext.Command.DispatcherId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<RemoveExecutableCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveExecutable(handlingContext.Command.ExecutableId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<RemoveNodeCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveNode(handlingContext.Command.NodeId);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof (DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }

    public void Handle(ICommandHandlingContext<RemoveSystemCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.RemoveSystem(handlingContext.Command.SystemId);
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

    public void Handle(ICommandHandlingContext<AssignSplAssetCommand> handlingContext)
    {
      try
      {
        var domainModel = _repository.GetById<DomainModel>(handlingContext.Command.Id);
        if (domainModel != null)
          domainModel.AssignSplAsset(handlingContext.Command.SplElementId, handlingContext.Command.AssetName);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

        _repository.Save(domainModel);
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
      }
    }
  }
}