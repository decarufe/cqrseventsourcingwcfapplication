using Server.Contracts.Events;
using SimpleCqrs.Eventing;

namespace Server.ReadModel.Endpoint
{
  public class ArchitectureEventHandler : 
    IHandleDomainEvents<ArchitectureCreatedEvent>,
    IHandleDomainEvents<NameChangedEvent>
  {
    private readonly ArchitectureRepository _repository;

    public ArchitectureEventHandler(ArchitectureRepository repository)
    {
      _repository = repository;
    }

    public void Handle(ArchitectureCreatedEvent domainEvent)
    {
      _repository.Add(new Architecture
      {
        Id = domainEvent.AggregateRootId.ToString()
      });
    }

    public void Handle(NameChangedEvent domainEvent)
    {
      Architecture architecture = _repository.GetById(domainEvent.AggregateRootId.ToString());
      architecture.Name = domainEvent.NewName;
      _repository.Update(architecture);
    }
  }
}