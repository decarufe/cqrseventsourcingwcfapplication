using System;
using System.Linq;
using Server.Engine;
using SimpleCqrs;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace Server.Wcf
{
  public class Global : System.Web.HttpApplication
  {

    private TrainingRuntime _runtime;

    protected void Application_Start(object sender, EventArgs e)
    {
      _runtime = new TrainingRuntime();
      _runtime.Start();
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_Error(object sender, EventArgs e)
    {

    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {
      _runtime.Shutdown();
    }
  }
}