using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;

namespace Server.Engine.Events
{
  public class NameChangedEvent : DomainEvent
  {
    public string NewName { set; get; }
  }
}
