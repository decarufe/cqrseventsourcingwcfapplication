using System.Collections.Generic;
using System.Reflection;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Unity;
using Server.Contracts;
using Server.Contracts.Events;

namespace Server.Wcf
{
  public class ClientBootStrapper : UnityBootStrapper
  {
  }

  public class ArchitectureEventHandler :
    ConsumerOf<ArchitectureCreatedEvent>,
    ConsumerOf<NameChangedEvent>
  {
    public void Consume(ArchitectureCreatedEvent message)
    {
      
    }

    public void Consume(NameChangedEvent message)
    {
      
    }
  }
}