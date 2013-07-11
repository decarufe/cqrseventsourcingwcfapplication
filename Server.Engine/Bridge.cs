using System;
using Server.Contracts;

namespace Server.Engine
{
  public class Bridge
  {
    // TODO: C'est laid!
    private static Action<EntityChangedMessage> _action;

    public Bridge()
    {
    }

    public void RegisterAction(Action<EntityChangedMessage> action)
    {
      _action = action;
    }

    public void SendMessage(EntityChangedMessage message)
    {
      _action(message);
    }
  }
}