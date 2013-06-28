using System;
using Server.Engine.Commands;
using SimpleCqrs.Commanding;

namespace Server.Engine.CommandHandlers
{
  public class CommandHandler :
    IHandleCommands<SetNameCommand>
  {
    public void Handle(ICommandHandlingContext<SetNameCommand> handlingContext)
    {
      Console.WriteLine((string) handlingContext.Command.Name);
    }
  }
}