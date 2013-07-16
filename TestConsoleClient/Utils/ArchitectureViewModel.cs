using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleClient.CqrsServiceReference;

namespace TestConsoleClient.Utils
{
  public static class ArchitectureViewModel
  {
    private const string Indentation = "  ";
    public static void SystemBreakdown(SystemEntity rootSystem, IEnumerable<SystemEntity> systems)
    {
      PrintSbsRecusively(rootSystem, systems.ToList(), Indentation);
    }

    private static void PrintSbsRecusively(SystemEntity parentSystem, List<SystemEntity> systems, string indentation)
    {
      Console.WriteLine(UtilsResource.FormatString_SystemDisplay, indentation, parentSystem.Name);
      foreach (var systemEntity in systems.Where(x => x.ParentSystemName == parentSystem.Name))
      {
        PrintSbsRecusively(systemEntity, systems, indentation + Indentation);
      }
    }

    public static void Deployment(Node node, List<Dispatchable> disaptchables)
    {
      Console.WriteLine(UtilsResource.NodeItemDisplay, node.Name);
      foreach (var executable in node.Executables)
      {
        Console.WriteLine(UtilsResource.FormatString_ExecutableDisplay, Indentation, executable);
      }
      foreach (var dispatcher in node.Dispatchers)
      {
        Console.WriteLine(UtilsResource.FormatString_DispatcherDisplay, Indentation, dispatcher);
        string dispatcher1 = dispatcher;
        foreach (var dispatchable in disaptchables.Where(x => x.Dispatcher == dispatcher1))
        {
          Console.WriteLine(UtilsResource.FormatString_DispatchableDisplay, Indentation, dispatchable);
        }
      }
    }
  }
}