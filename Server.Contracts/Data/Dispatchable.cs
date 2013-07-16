using System.Diagnostics;
using System.Runtime.Serialization;

namespace Server.Contracts.Data
{
  [DataContract]
  [DebuggerDisplay("Dispatchable: {Name} (Parent: {ParentSystemName}), Dispatcher: {Dispatcher}")]
  public class Dispatchable : SystemEntity
  {
    [DataMember(Order = 0)]
    public long Dispatcher { get; set; }
  }
}