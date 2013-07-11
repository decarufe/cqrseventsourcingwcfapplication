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
    IHandleCommands<SetNameCommand>,
    IHandleCommands<AddSystemCommand>,
    IHandleCommands<RemoveSystemCommand>,
    IHandleCommands<AddNodeCommand>,
    IHandleCommands<RemoveNodeCommand>,
    IHandleCommands<AddExecutableCommand>,
    IHandleCommands<RemoveExecutableCommand>,
    IHandleCommands<CommitVersionCommand>
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
      catch (AggregateRootNotFoundException)
      {
        DomainModel.Create(handlingContext.Command.Id, handlingContext.Command.Name);
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
          domainModel.AddSystem(handlingContext.Command.Name,handlingContext.Command.ParentSystemName);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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
          domainModel.RemoveSystem(handlingContext.Command.Name);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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
          domainModel.AddNode(handlingContext.Command.Name, handlingContext.Command.ParentSystemName);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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
          domainModel.RemoveNode(handlingContext.Command.Name);
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
          domainModel.AddExecutable(handlingContext.Command.Name, handlingContext.Command.ParentSystemName);
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
          domainModel.RemoveExecutable(handlingContext.Command.Name);
        else
          throw new AggregateRootNotFoundException(handlingContext.Command.Id, typeof(DomainModel));

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