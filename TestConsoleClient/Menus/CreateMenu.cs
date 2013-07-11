using System;
using System.Collections.Generic;
using System.Linq;
using TestConsoleClient.CqrsService;
using TestConsoleClient.Utils;

namespace TestConsoleClient.Menus
{
  public static class CreateMenu
  {
    public static void Run()
    {
      using (var client = new CqrsServiceClient())
      {
        var architectureList = client.GetList().Select(x => x.Name).Distinct().ToList();
        Console.Clear();
        Console.WriteLine();
        DisplayArchitectureList(architectureList);
        Console.WriteLine();
        Console.Write(ArchitectureResource.EnterProductNamePropmt);
        var name = Console.ReadLine();

        if (string.IsNullOrEmpty(name))
        {
          ErrorManagement.PrintError(string.Format("You must specifiy a name for the Architecture!"));
        }

        if (architectureList.All(x => x != name))
        {
          var id = Guid.NewGuid();
          client.SetName(id,name);
          client.AddSystem(id, ArchitectureResource.RootSystemName, null);
          client.AddSystem(id, ArchitectureResource.System1, ArchitectureResource.RootSystemName);
          client.AddSystem(id, ArchitectureResource.System1_1, ArchitectureResource.System1);
          client.AddSystem(id, ArchitectureResource.System1_2, ArchitectureResource.System1);
          client.AddSystem(id, ArchitectureResource.System1_3, ArchitectureResource.System1);
          client.AddSystem(id, ArchitectureResource.System1_3_1, ArchitectureResource.System1_3);
          client.AddSystem(id, ArchitectureResource.System1_3_2, ArchitectureResource.System1_3);
          client.AddSystem(id, ArchitectureResource.System2, ArchitectureResource.RootSystemName);
          client.AddSystem(id, ArchitectureResource.System2_1, ArchitectureResource.System2);
          client.AddSystem(id, ArchitectureResource.System2_2, ArchitectureResource.System2);
          client.AddSystem(id, ArchitectureResource.System2_3, ArchitectureResource.System2);
          client.AddSystem(id, ArchitectureResource.System3, ArchitectureResource.RootSystemName);
          client.AddSystem(id, ArchitectureResource.System3_1, ArchitectureResource.System3);
          client.AddSystem(id, ArchitectureResource.System3_2, ArchitectureResource.System3);
          client.AddSystem(id, ArchitectureResource.System3_3, ArchitectureResource.System3);
          client.AddSystem(id, ArchitectureResource.System3_1_1, ArchitectureResource.System3_1);
          client.AddSystem(id, ArchitectureResource.System3_1_2, ArchitectureResource.System3_1);
          client.AddSystem(id, ArchitectureResource.System3_1_3, ArchitectureResource.System3_1);
          client.AddNode(id, ArchitectureResource.Node1, ArchitectureResource.System1_1);
          client.AddNode(id, ArchitectureResource.Node2, ArchitectureResource.System1_3);
          client.AddExecutable(id, ArchitectureResource.System1_2Exec, ArchitectureResource.System1_2);
          client.AddExecutable(id, ArchitectureResource.System1_3Exec1, ArchitectureResource.System1_3);
          client.AddExecutable(id, ArchitectureResource.System1_3Exec2, ArchitectureResource.System1_3);
          client.AddExecutable(id, ArchitectureResource.System2_1Exec, ArchitectureResource.System2_1);
          client.AddExecutable(id, ArchitectureResource.System2_2Exec, ArchitectureResource.System2_2);
          client.AddExecutable(id, ArchitectureResource.System2_3Exec, ArchitectureResource.System2_3);
          client.AddExecutable(id, ArchitectureResource.System3_1_1Exec, ArchitectureResource.System3_1_1);
          client.AddExecutable(id, ArchitectureResource.System3_1_2Exec, ArchitectureResource.System3_1_2);
          client.AddExecutable(id, ArchitectureResource.System3_1_3Exec, ArchitectureResource.System3_1_3);
          client.CommitVersion(id);
          client.AssignExecutableToNode(id, ArchitectureResource.System1_2Exec, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System1_3Exec1, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System1_3Exec1, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System2_1Exec, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System2_2Exec, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System2_3Exec, ArchitectureResource.Node1);
          client.AssignExecutableToNode(id, ArchitectureResource.System3_1_1Exec, ArchitectureResource.Node2);
          client.AssignExecutableToNode(id, ArchitectureResource.System3_1_2Exec, ArchitectureResource.Node2);
          client.AssignExecutableToNode(id, ArchitectureResource.System3_1_3Exec, ArchitectureResource.Node2);
          client.CommitVersion(id);
          client.RemoveSystem(id, ArchitectureResource.System1_3);
          client.CommitVersion(id);
        }
        else
        {
          ErrorManagement.PrintError(string.Format("An architecture named {0} already exists!", name));
        }

      }
    }


    private static void DisplayArchitectureList(IEnumerable<string> architectureList)
    {
      foreach (var architectureName in architectureList)
      {
        Console.WriteLine(ArchitectureResource.ArchitectureNameDisplayItem, architectureName);
      }
    }
  }
}