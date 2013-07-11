using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleClient.CqrsService;

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
      Console.WriteLine(UtilsResource.FormatString_Var0Var1, indentation, parentSystem.Name);
      foreach (var systemEntity in systems.Where(x => x.ParentSystemName == parentSystem.Name))
      {
        PrintSbsRecusively(systemEntity, systems, indentation + Indentation);
      }
    }

    public static void Deployment(Node node)
    {
      Console.WriteLine(UtilsResource.NodeItemDisplay, node.Name);
      foreach (var executable in node.Executables)
      {
        Console.WriteLine(UtilsResource.FormatString_Var0Var1, Indentation, executable);
      }
    }
  }
}