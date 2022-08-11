﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NotCore;

public class Assets
{
    private readonly Dictionary<string, IAsset> _lookupTable = new();

    private T GetAsset<T>(string key) where T : class, IAsset
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

    public T GetDynamicAsset<T>(string key) where T : class
    {
        var result = GetAsset<DynamicAsset>(key).Content as T;

        if (result == null)
        {
            throw new Exception($"Asset with key {key} cannot be casted as {typeof(T).Name}");
        }
        
        return result;
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

    public SpriteFont GetSpriteFont(string key)
    {
        return GetAsset<SpriteFontAsset>(key).SpriteFont;
    }

    public void AddAsset(IAsset asset)
    {
        _lookupTable.Add(asset.Key, asset);
    }

    public void UnloadAllDynamicContent()
    {
        foreach (var asset in _lookupTable.Values)
        {
            if (asset is DynamicAsset dynamicAsset)
            {
                dynamicAsset.Unload();
            }
        }
    }
}
