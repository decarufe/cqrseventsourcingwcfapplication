﻿using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class RemoveNodeCommand : ICommand
  {
    private readonly string _name;
    private readonly Guid _id;

    public RemoveNodeCommand(Guid id, string name)
    {
      _id = id;
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    public Guid Id
    {
      get { return _id; }
    }
  }
}