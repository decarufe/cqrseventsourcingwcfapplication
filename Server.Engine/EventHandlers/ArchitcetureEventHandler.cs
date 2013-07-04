using Rhino.ServiceBus;
using Server.Contracts;
using Server.Contracts.Events;
using Server.ReadModels;
using SimpleCqrs.Eventing;

namespace Server.Engine.EventHandlers
{
  public class ArchitcetureEventHandler :
    IHandleDomainEvents<ArchitectureCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>,
    ConsumerOf<ArchitectureCreatedEvent>,
    ConsumerOf<NameChangedEvent>
  {
    public void Handle(ArchitectureCreatedEvent domainEvent)
    {
      Persistance.Instance.Save(domainEvent.AggregateRootId, new ArchitectureView());
    }


    public void Handle(NameChangedEvent domainEvent)
    {
      ArchitectureView architectureView = Persistance.Instance.Get(domainEvent.AggregateRootId);
      architectureView.Id = domainEvent.AggregateRootId;
      architectureView.Name = domainEvent.NewName;
      Persistance.Instance.Save(domainEvent.AggregateRootId, architectureView);
    }

    public void Consume(ArchitectureCreatedEvent message)
    {
      
    }

    public void Consume(NameChangedEvent message)
    {
      
    }
  }
}