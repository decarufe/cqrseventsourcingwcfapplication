using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;

namespace Server.Engine.Events
{
  class NameChangedEvent : DomainEvent
  {
    public string NewName { set; get; }
  }
}
