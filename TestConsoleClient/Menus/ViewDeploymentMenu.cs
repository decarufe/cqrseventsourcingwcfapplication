using System;
using System.Linq;
using TestConsoleClient.CqrsServiceReference;
using TestConsoleClient.Utils;

namespace TestConsoleClient.Menus
{
  public static class ViewDeploymentMenu
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
            var nodes = client.GetNodes(selectedArchitecture).ToList();
            var dispatchables = client.GetDispatchables(selectedArchitecture).ToList();

            foreach (var node in nodes)
            {
              ArchitectureViewModel.Deployment(node, dispatchables);
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