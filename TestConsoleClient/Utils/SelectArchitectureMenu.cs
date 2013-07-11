using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleClient.CqrsService;
using TestConsoleClient.Menus;

namespace TestConsoleClient.Utils
{
  public static class SelectArchitectureMenu
  {
    public static Guid Select(CqrsServiceClient client)
    {
      var done = false;
      while (!done)
      {
        Console.Clear();
        Console.WriteLine();
        var architectures = client.GetList().ToList();
        if (architectures.Any())
        {
          DisplayArchitectures(architectures);

          Console.WriteLine();
          Console.WriteLine(MainMenuResource.MenuItem_Refresh);
          Console.WriteLine(MainMenuResource.MenuItem_Exit);

          var answer = Console.ReadLine();
          int selectedIndex;
          if (Int32.TryParse(answer, out selectedIndex))
          {
            var normalizedIndex = selectedIndex - 1;
            if (normalizedIndex >= 0
                && normalizedIndex < architectures.Count)
            {
              return Guid.Parse(architectures[normalizedIndex].ReadModelId);
            }

            ErrorManagement.PrintError(UtilsResource.IndexOutOfRangeError);
          }
          else
          {
            switch (answer)
            {
              case "q":
              case "Q":
                done = true;
                break;
            }
          }

        }
        else
        {
          ErrorManagement.PrintError(UtilsResource.ArchitecturesEmptyError);
          done = true;
        }
      }

      return Guid.Empty;
    }

    private static void DisplayArchitectures(IEnumerable<DomainModelDto> architectures)
    {
      var counter = 0;
      foreach (var architecture in architectures)
      {
        counter++;
        Console.WriteLine(UtilsResource.ArchitectureItemDisplay, counter, architecture.Name, architecture.Version);
      }
    }
  }
}