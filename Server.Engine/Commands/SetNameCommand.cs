using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class SetNameCommand : ICommand
  {
    public string Name { get; set; }
  }
}