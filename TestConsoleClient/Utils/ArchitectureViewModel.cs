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
      foreach (var systemEntity in systems.Where(x => x.ParentSystemId == parentSystem.Id))
      {
        PrintSbsRecusively(systemEntity, systems, indentation + Indentation);
      }
    }

    public static void Deployment(Node node, List<Executable> executables, List<Dispatcher> dispatchers, List<Dispatchable> disaptchables)
    {
      Console.WriteLine(UtilsResource.NodeItemDisplay, node.Name);
      foreach (var executable in node.Executables)
      {
        var excutableName = executables.First(x => x.Id == executable).Name;
        Console.WriteLine(UtilsResource.FormatString_ExecutableDisplay, Indentation, excutableName);
      }
      foreach (var dispatcher in node.Dispatchers)
      {
        var dispatcherName = dispatchers.First(x => x.Id == dispatcher).Name;
        Console.WriteLine(UtilsResource.FormatString_DispatcherDisplay, Indentation, dispatcherName);
        long dispatcher1 = dispatcher;
        foreach (var dispatchable in disaptchables.Where(x => x.Dispatcher == dispatcher1))
        {
          Console.WriteLine(UtilsResource.FormatString_DispatchableDisplay, Indentation, dispatchable.Name);
        }
      }
    }
  }
}