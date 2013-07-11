using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignExecutableToNodeCommand : ICommand
  {
    private readonly string _nodeName;
    private readonly string _executableName;
    private readonly Guid _id;

    public AssignExecutableToNodeCommand(Guid id, string executableName, string nodeName)
    {
      _id = id;
      _executableName = executableName;
      _nodeName = nodeName;
    }

    public string ExecutableName
    {
      get { return _executableName; }
    }

    public string NodeName
    {
      get { return _nodeName; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}
