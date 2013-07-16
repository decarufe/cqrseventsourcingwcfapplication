using System;
using System.Linq;
using TestConsoleClient.CqrsServiceReference;
using TestConsoleClient.Utils;

namespace TestConsoleClient.Menus
{
  public static class ViewSystemsMenu
  {
    public static void Run()
    {
      var done = false;
      while (!done)
      {
        using (var client = new CqrsServiceClient())
        {
          var selectedArchitecture = SelectArchitectureMenu.Select(client);

          if (selectedArchitecture != Guid.Empty)
          {
            var systems = client.GetSystems(selectedArchitecture);
            var rootSystems = systems.Where(x => x.ParentSystemId == 0);

            foreach (var system in rootSystems)
            {
              ArchitectureViewModel.SystemBreakdown(system,systems);
            }
          }
          else
          {
            return;
          }
        }

        Console.WriteLine();
        Console.WriteLine(MainMenuResource.MenuItem_Continue);
        Console.WriteLine(MainMenuResource.MenuItem_Exit);
        var answer = Console.ReadLine();
        if (answer == "q" || answer == "Q")
        {
          done = true;
        }
      }
    }
     
  }
}