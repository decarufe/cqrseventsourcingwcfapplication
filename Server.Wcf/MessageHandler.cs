using Rhino.ServiceBus;
using Server.Contracts;
using Server.Engine;

namespace Server.Wcf
{
  public class MessageHandler :
    ConsumerOf<EntityChangedMessage>
  {
    private readonly Bridge _bridge;

    public MessageHandler(Bridge bridge)
    {
      _bridge = bridge;
    }

    public void Consume(EntityChangedMessage message)
    {
      _bridge.SendMessage(message);
    }
  }
}