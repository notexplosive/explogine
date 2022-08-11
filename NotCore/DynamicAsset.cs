using System;

namespace NotCore;

public class DynamicAsset : IAsset
{
    public IDisposable Content { get; }

    public DynamicAsset(string key, IDisposable disposable)
    {
        Content = disposable;
        Key = key;
    }

    public string Key { get; }

    public void Unload()
    {
        Content.Dispose();
    }
}
