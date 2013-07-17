using System;
using System.Linq;
using TestConsoleClient.CqrsServiceReference;
using TestConsoleClient.Utils;

namespace TestConsoleClient.Menus
{
  public static class ViewBomMenu
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
            var splAssets = client.GetSplAssets(selectedArchitecture).ToList();
            ArchitectureViewModel.Bom(splAssets);
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