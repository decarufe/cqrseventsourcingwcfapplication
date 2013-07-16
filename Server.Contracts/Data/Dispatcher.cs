﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("Dispatcher: {Name}, Node: {Node}")]
  public class Dispatcher : SystemEntity
  {
    [DataMember(Order = 0)]
    public string Node { get; set; }

    [DataMember(Order = 1)]
    public IEnumerable<string> Dispatchables { get; set; }
  }
}