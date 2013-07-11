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
         Console.WriteLine("Select Operation: ");
         Console.WriteLine("1) Create Architecture");
         Console.WriteLine("2) View Systems");
         Console.WriteLine("3) View Deployment");
         Console.WriteLine();
         Console.WriteLine("Q) Exit");

         var answer = Console.ReadLine();

         switch (answer)
         {
           case "1":
             CreateMenu.Run();
             break;
           case "2":
             break;
           case "3":
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