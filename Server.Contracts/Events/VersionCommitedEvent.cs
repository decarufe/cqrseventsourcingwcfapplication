using System;
using SimpleCqrs.Eventing;

namespace Server.Contracts.Events
{
  public class VersionCommitedEvent : DomainEvent
  {
    public string NewVersion { get; set; }
  }
}