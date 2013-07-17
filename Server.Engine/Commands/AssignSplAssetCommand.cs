using System;
using SimpleCqrs.Commanding;

namespace Server.Engine.Commands
{
  public class AssignSplAssetCommand : ICommand
  {
    private readonly string _assetName;
    private readonly long _splElementId;
    private readonly Guid _id;

    public AssignSplAssetCommand(Guid id, long splElementId, string assetName)
    {
      _id = id;
      _splElementId = splElementId;
      _assetName = assetName;
    }

    public string AssetName
    {
      get { return _assetName; }
    }

    public long SplElementId
    {
      get { return _splElementId; }
    }

    public Guid Id
    {
      get { return _id; }
    }

  }
}