using Server.Contracts;
using Server.Engine.Events;
using Server.ReadModels;
using SimpleCqrs.Eventing;

namespace Server.Engine.EventHandles
{
  public class ArchitectureEventHandler :
    IHandleDomainEvents<ArchitectureCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>
  {
    public void Handle(ArchitectureCreatedEvent domainEvent)
    {
      Persistance.Instance.Save(domainEvent.AggregateRootId, new ArchitectureView());
    }

    public void Handle(NameChangedEvent domainEvent)
    {
      var architectureView = Persistance.Instance.Get(domainEvent.AggregateRootId);
      architectureView.Name = domainEvent.NewName;
      Persistance.Instance.Save(domainEvent.AggregateRootId, architectureView);
    }
  }
}