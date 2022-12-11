using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class IndirectTexture
{
    private readonly Lazy<Texture2D> _lazy;

    public IndirectTexture(string path)
    {
        _lazy = new Lazy<Texture2D>(() => Client.Assets.GetTexture(path));
    }

    public IndirectTexture(Texture2D texture)
    {
        _lazy = new Lazy<Texture2D>(texture);
    }
    
    public static implicit operator IndirectTexture(Texture2D realTexture)
    {
        return new IndirectTexture(realTexture);
    }
    
    public static implicit operator IndirectTexture(string textureAssetPath)
    {
        return new IndirectTexture(textureAssetPath);
    }

    [Pure]
    public Texture2D Get()
    {
        return _lazy.Value;
    }
}
