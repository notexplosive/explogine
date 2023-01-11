using System;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class Assets
{
    private readonly Dictionary<string, Asset> _lookupTable = new();

    internal Assets()
    {
    }

    public T GetAsset<T>(string key) where T : Asset
    {
        if (!_lookupTable.ContainsKey(key))
        {
            throw new Exception($"No such asset: {key}");
        }

        if (_lookupTable[key] is not T result)
        {
            throw new Exception($"No {typeof(T).Name} with name {key} found");
        }

        return result;
    }
    
    public Effect GetEffect(string key)
    {
        return GetAsset<EffectAsset>(key).Effect;
    }

    public Texture2D GetTexture(string key)
    {
        return GetAsset<TextureAsset>(key).Texture;
    }

    public SoundEffect GetSoundEffect(string key)
    {
        return GetAsset<SoundAsset>(key).SoundEffect;
    }

    public SoundEffectInstance GetSoundEffectInstance(string key)
    {
        return GetAsset<SoundAsset>(key).SoundEffectInstance;
    }

    public Font GetFont(string key, int desiredSize)
    {
        var spriteFont = GetAsset<SpriteFontAsset>(key).SpriteFont;
        return new Font(spriteFont, desiredSize);
    }

    internal void AddAsset(string key, Asset asset)
    {
        _lookupTable.Add(key, asset);
    }

    public void UnloadAll()
    {
        foreach (var item in _lookupTable.Values)
        {
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public IEnumerable<KeyValuePair<string, Asset>> GetAllAssetKeyValuePairs()
    {
        foreach (var item in _lookupTable)
        {
            yield return item;
        }
    }
}
