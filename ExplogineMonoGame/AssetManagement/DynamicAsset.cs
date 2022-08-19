using System;

namespace ExplogineMonoGame.AssetManagement;

public class DynamicAsset : Asset
{
    public DynamicAsset(string key, IDisposable disposable) : base(key)
    {
        Content = disposable;
    }

    public IDisposable Content { get; }

    public void Unload()
    {
        Content.Dispose();
    }
}
