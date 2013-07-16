using System;

namespace TestConsoleClient.Utils
{
  public static class ErrorManagement
  {
    public static void PrintError(string error)
    {
      Console.WriteLine();
      Console.WriteLine("========================================================================");
      Console.WriteLine("= ERROR - You Dumb Ass!!!!");
      Console.WriteLine("=");
      Console.WriteLine("= {0}", error);
      Console.WriteLine("========================================================================");
      Console.WriteLine();
      Console.WriteLine("Press Enter to continue...");
      Console.ReadLine();
    }
     
  }
}