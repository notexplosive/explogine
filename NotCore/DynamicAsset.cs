using System;

namespace NotCore;

public class DynamicAsset : IAsset
{
    public DynamicAsset(string key, IDisposable disposable)
    {
        Content = disposable;
        Key = key;
    }

    public IDisposable Content { get; }

    public string Key { get; }

    public void Unload()
    {
        Content.Dispose();
    }
}
