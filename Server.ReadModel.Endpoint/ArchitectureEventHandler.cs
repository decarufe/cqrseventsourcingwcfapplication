using System.Diagnostics;
using Rhino.ServiceBus;
using Server.Contracts.Events;
using SimpleCqrs.Eventing;

namespace Server.ReadModel.Endpoint
{
  public class ArchitectureEventHandler : 
    ConsumerOf<ArchitectureCreatedEvent>,
    ConsumerOf<NameChangedEvent>
  {
    private readonly ArchitectureRepository _repository;

    public ArchitectureEventHandler(ArchitectureRepository repository)
    {
      _repository = repository;
    }

    public void Consume(ArchitectureCreatedEvent message)
    {
      Debug.WriteLine("Message: {0}", message.AggregateRootId);
      _repository.Add(new Architecture
      {
        Id = message.AggregateRootId.ToString()
      });
    }

    public void Consume(NameChangedEvent message)
    {
      Architecture architecture = _repository.GetById(message.AggregateRootId.ToString());
      architecture.Name = message.NewName;
      _repository.Update(architecture);
    }
  }
}