using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server.Backend.Endpoint
{
  [ServiceContract]
  public interface ICommandService
  {
    [OperationContract]
    void DoWork();
  }
}
