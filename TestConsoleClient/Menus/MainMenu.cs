using System;

namespace TestConsoleClient.Menus
{
  public static class MainMenu
  {
     public static void Run()
     {
       var done = false;
       while (!done)
       {
         Console.Clear();
         Console.WriteLine();
         Console.WriteLine(MainMenuResource.SelectOperation);
         Console.WriteLine(MainMenuResource.MenuItem_CreateArchitecture);
         Console.WriteLine(MainMenuResource.MenuItem_ViewSystems);
         Console.WriteLine(MainMenuResource.MenuItem_ViewDeployment);
         Console.WriteLine();
         Console.WriteLine(MainMenuResource.MenuItem_Exit);
         Console.WriteLine();
         Console.Write(MainMenuResource.SelectionPrompt);

         var answer = Console.ReadLine();

         switch (answer)
         {
           case "1":
             CreateMenu.Run();
             break;
           case "2":
             ViewSystemsMenu.Run();
             break;
           case "3":
             ViewDeploymentMenu.Run();
             break;
           case "q":
           case "Q":
             done = true;
             break;
         }

       }
     }
  }
}