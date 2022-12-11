using System;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.AssetManagement;

namespace ExplogineMonoGame.Data;

public class IndirectAsset<T> where T : Asset
{
    private readonly Lazy<T> _lazy;

    public IndirectAsset(string assetPath)
    {
        _lazy = new Lazy<T>(() => Client.Assets.GetAsset<T>(assetPath));
    }

    private IndirectAsset(T actualAsset)
    {
        _lazy = new Lazy<T>(actualAsset);
    }

    public static implicit operator IndirectAsset<T>(T realAsset)
    {
        return new IndirectAsset<T>(realAsset);
    }
    
    public static implicit operator IndirectAsset<T>(string assetName)
    {
        return new IndirectAsset<T>(assetName);
    }

    [Pure]
    public T Get()
    {
        return _lazy.Value;
    }
}
