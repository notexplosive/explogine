using System;
using ExplogineMonoGame.AssetManagement;

namespace ExplogineMonoGame;

public interface ILoadEvent
{
    string Key { get; }
    string? Info { get; }
    void Execute();
}

public readonly record struct VoidLoadEvent(string Key, string? Info, Action Function) : ILoadEvent
{
    public VoidLoadEvent(string key, Action eventFunction) : this(key, null, eventFunction)
    {
    }
    
    public void Execute()
    {
        Function.Invoke();
    }
}

public readonly record struct AssetLoadEvent(string Key, string? Info, LoadEventFunction Function) : ILoadEvent
{
    public void Execute()
    {
        ExecuteAndReturnAsset();
    }

    public AssetLoadEvent(string key, LoadEventFunction eventFunction) : this(key, null, eventFunction)
    {
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
