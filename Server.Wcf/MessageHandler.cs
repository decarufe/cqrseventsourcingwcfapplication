using System.Diagnostics;
using Rhino.ServiceBus;
using Server.Contracts;

namespace Server.Wcf
{
  public class MessageHandler :
    ConsumerOf<EntityChangedMessage>
  {
    public void Consume(EntityChangedMessage message)
    {
      Debug.WriteLine("Entity changed " + message.Id);
    }
  }
}