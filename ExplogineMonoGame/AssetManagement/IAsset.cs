using System;

namespace ExplogineMonoGame.AssetManagement;

public interface IAsset
{
    public string Key { get; }
}

public abstract class Asset : IAsset, IDisposable
{
    private readonly IDisposable? _ownedContent;
    public string Key { get; }

    protected Asset(string key, IDisposable? ownedContent)
    {
        Key = key;
        _ownedContent = ownedContent;
    }

    public void Dispose()
    {
        _ownedContent?.Dispose();
    }
}