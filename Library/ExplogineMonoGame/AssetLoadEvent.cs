using System;
using ExplogineMonoGame.AssetManagement;

namespace ExplogineMonoGame;

public interface ILoadEvent
{
    string Key { get; }
    void Execute();
}

public readonly record struct VoidLoadEvent(string Key, Action Function) : ILoadEvent
{
    public void Execute()
    {
        Function.Invoke();
    }
}

public readonly record struct AssetLoadEvent(string Key, LoadEventFunction Function) : ILoadEvent
{
    public void Execute()
    {
        ExecuteAndReturnAsset();
    }

    public Asset ExecuteAndReturnAsset()
    {
        var asset = Function.Invoke();
        Client.Assets.AddAsset(Key, asset);
        return asset;
    }

    public override string ToString()
    {
        return Key;
    }
}
