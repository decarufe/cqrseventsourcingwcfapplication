using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestConsoleClient.CqrsServiceReference;
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
          return;
        }

        if (architectureList.All(x => x != name))
        {
          var id = Guid.NewGuid();
          client.SetName(id,name);
          long rootSystemId = client.AddSystem(id, ArchitectureResource.RootSystemName, 0);
          long system1Id = client.AddSystem(id, ArchitectureResource.System1, rootSystemId);
          long system11Id = client.AddSystem(id, ArchitectureResource.System1_1, system1Id);
          long system12Id = client.AddSystem(id, ArchitectureResource.System1_2, system1Id);
          long system13Id = client.AddSystem(id, ArchitectureResource.System1_3, system1Id);
          long system131Id = client.AddSystem(id, ArchitectureResource.System1_3_1, system13Id);
          long system132Id = client.AddSystem(id, ArchitectureResource.System1_3_2, system13Id);
          long system2Id = client.AddSystem(id, ArchitectureResource.System2, rootSystemId);
          long system21Id = client.AddSystem(id, ArchitectureResource.System2_1, system2Id);
          long system22Id = client.AddSystem(id, ArchitectureResource.System2_2, system2Id);
          long system23Id = client.AddSystem(id, ArchitectureResource.System2_3, system2Id);
          long system3Id = client.AddSystem(id, ArchitectureResource.System3, rootSystemId);
          long system31Id = client.AddSystem(id, ArchitectureResource.System3_1, system3Id);
          long system32Id = client.AddSystem(id, ArchitectureResource.System3_2, system3Id);
          long system33Id = client.AddSystem(id, ArchitectureResource.System3_3, system3Id);
          long system311Id = client.AddSystem(id, ArchitectureResource.System3_1_1, system31Id);
          long system312Id = client.AddSystem(id, ArchitectureResource.System3_1_2, system31Id);
          long system313Id = client.AddSystem(id, ArchitectureResource.System3_1_3, system31Id);
          long node1Id = client.AddNode(id, ArchitectureResource.Node1, system11Id);
          long node2Id = client.AddNode(id, ArchitectureResource.Node2, system13Id);
          long dispatcher1Id = client.AddDispatcher(id, ArchitectureResource.Dispatcher1, node1Id);
          long dispatcher2Id = client.AddDispatcher(id, ArchitectureResource.Dispatcher2, node1Id);
          long dispatcher3Id = client.AddDispatcher(id, ArchitectureResource.Dispatcher3, node2Id);
          long dispatcher4Id = client.AddDispatcher(id, ArchitectureResource.Dispatcher4, node2Id);
          long system12ExecId = client.AddExecutable(id, ArchitectureResource.System1_2Exec, system12Id);
          long system13Exec1Id = client.AddExecutable(id, ArchitectureResource.System1_3Exec1, system13Id);
          long system13Exec2Id = client.AddExecutable(id, ArchitectureResource.System1_3Exec2, system13Id);
          long system21ExecId = client.AddExecutable(id, ArchitectureResource.System2_1Exec, system21Id);
          long system22ExecId = client.AddExecutable(id, ArchitectureResource.System2_2Exec, system22Id);
          long system23ExecId = client.AddExecutable(id, ArchitectureResource.System2_3Exec, system23Id);
          long system311ExecId = client.AddExecutable(id, ArchitectureResource.System3_1_1Exec, system311Id);
          long system312ExecId = client.AddExecutable(id, ArchitectureResource.System3_1_2Exec, system312Id);
          long system313ExecId = client.AddExecutable(id, ArchitectureResource.System3_1_3Exec, system313Id);
          long system12DispId = client.AddDispatchable(id, ArchitectureResource.System1_2Disp, system12Id);
          long system13DispId = client.AddDispatchable(id, ArchitectureResource.System1_3Disp, system13Id);
          long system21DispId = client.AddDispatchable(id, ArchitectureResource.System2_1Disp, system21Id);
          long system22DispId = client.AddDispatchable(id, ArchitectureResource.System2_2Disp, system22Id);
          long system23DispId = client.AddDispatchable(id, ArchitectureResource.System2_3Disp, system23Id);
          long system311Disp1Id = client.AddDispatchable(id, ArchitectureResource.System3_1_1Disp1, system311Id);
          long system311Disp2Id = client.AddDispatchable(id, ArchitectureResource.System3_1_1Disp2, system311Id);
          long system311Disp3Id = client.AddDispatchable(id, ArchitectureResource.System3_1_1Disp3, system311Id);
          client.CommitVersion(id);
          client.AssignExecutableToNode(id, system12ExecId, node1Id);
          client.AssignExecutableToNode(id, system13Exec1Id, node1Id);
          client.AssignExecutableToNode(id, system13Exec2Id, node1Id);
          client.AssignExecutableToNode(id, system21ExecId, node1Id);
          client.AssignExecutableToNode(id, system22ExecId, node1Id);
          client.AssignExecutableToNode(id, system23ExecId, node1Id);
          client.AssignExecutableToNode(id, system311ExecId, node2Id);
          client.AssignExecutableToNode(id, system312ExecId, node2Id);
          client.AssignExecutableToNode(id, system313ExecId, node2Id);
          client.CommitVersion(id);
          client.AssignDispatchableToDispatcher(id, system12DispId, dispatcher1Id);
          client.AssignDispatchableToDispatcher(id, system13DispId, dispatcher1Id);
          client.AssignDispatchableToDispatcher(id, system21DispId, dispatcher2Id);
          client.AssignDispatchableToDispatcher(id, system22DispId, dispatcher3Id);
          client.AssignDispatchableToDispatcher(id, system23DispId, dispatcher4Id);
          client.AssignDispatchableToDispatcher(id, system311Disp1Id, dispatcher4Id);
          client.AssignDispatchableToDispatcher(id, system311Disp2Id, dispatcher4Id);
          client.AssignDispatchableToDispatcher(id, system311Disp3Id, dispatcher4Id);
          client.CommitVersion(id);
          client.AssignDispatcherToNode(id, dispatcher4Id, node1Id);
          client.CommitVersion(id);
          client.RemoveSystem(id, system13Id);
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