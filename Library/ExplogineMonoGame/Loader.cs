﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public delegate Asset LoadEventFunction();

public readonly record struct LoadEvent(string Key, LoadEventFunction Function)
{
    public Asset Execute()
    {
        var asset = Function.Invoke();
        Client.Assets.AddAsset(Key, asset);
        return asset;
    }
}

public class Loader
{
    private readonly ContentManager _content;
    private readonly List<LoadEvent> _loadEvents = new();
    private int _loadEventIndex;

    public Loader(ContentManager content)
    {
        _content = content;
        foreach (var loadEvent in StaticContentLoadEvents())
        {
            _loadEvents.Add(loadEvent);
        }
    }

    private int LoadEventCount => _loadEvents.Count;
    public float Percent => (float) _loadEventIndex / LoadEventCount;

    public T ForceLoad<T>(string key) where T : Asset
    {
        if (IsDone())
        {
            return Client.Assets.GetAsset<T>(key);
        }

        LoadEvent? found = null;
        foreach (var loadEvent in _loadEvents)
        {
            if (loadEvent.Key == key)
            {
                found = loadEvent;
            }
        }

        if (found.HasValue)
        {
            _loadEvents.Remove(found.Value);
            var asset = found.Value.Execute();
            var result = asset as T;

            if (result == null)
            {
                throw new InvalidCastException($"{key} refers to {asset} which cannot be cast as {typeof(T)}");
            }

            return result;
        }

        throw new KeyNotFoundException($"No LoadEvent with key {key}, maybe preload hasn't been completed?");
    }

    public bool IsDone()
    {
        return _loadEventIndex >= LoadEventCount;
    }

    public void LoadNext()
    {
        _loadEvents[_loadEventIndex].Execute();
        _loadEventIndex++;
        Client.Debug.Log("Loading: " + MathF.Floor(Percent * 100f) + "%");
    }

    private IEnumerable<LoadEvent> StaticContentLoadEvents()
    {
        foreach (var key in Loader.GetKeysFromContentDirectory())
        {
            yield return new LoadEvent(key, () => LoadAsset(key));
        }
    }

    private Asset LoadAsset(string key)
    {
        var texture2D = AttemptLoad<Texture2D>(key);
        if (texture2D != null)
        {
            return new TextureAsset(texture2D);
        }

        var soundEffect = AttemptLoad<SoundEffect>(key);
        if (soundEffect != null)
        {
            return new SoundAsset(soundEffect);
        }

        var spriteFont = AttemptLoad<SpriteFont>(key);
        if (spriteFont != null)
        {
            return new SpriteFontAsset(spriteFont);
        }

        throw new Exception($"Unsupported/Unidentified Asset: {key}");
    }

    private T? AttemptLoad<T>(string key) where T : class
    {
        try
        {
            return _content.Load<T>(key);
        }
        catch (InvalidCastException)
        {
            return null;
        }
    }

    private static string[] GetKeysFromContentDirectory()
    {
        var fileNames = Client.FileSystem.GetFilesAtContentDirectory(".", "xnb");
        var keys = new List<string>();

        foreach (var fileName in fileNames)
        {
            var file = new FileInfo(fileName);
            var fullName = file.FullName;
            var fullNameWithoutExtension = fullName.Substring(0, fullName.Length - file.Extension.Length);
            var relativePath = fullNameWithoutExtension.Replace(Client.FileSystem.ContentPath, "");
            var split = relativePath.Split(Path.DirectorySeparatorChar);
            var list = split.ToList();
            list.RemoveAll(string.IsNullOrEmpty);
            keys.Add(string.Join('/', list));
        }

        return keys.ToArray();
    }

    public void Unload()
    {
        _content.Unload();
    }

    public void AddLoadEvent(LoadEvent loadEvent)
    {
        _loadEvents.Add(loadEvent);
    }

    public void AddLoadEvents(IEnumerable<ILoadEventProvider> providers)
    {
        foreach (var provider in providers)
        {
            AddLoadEvents(provider);
        }
    }

    public void AddLoadEvents(ILoadEventProvider provider)
    {
        foreach (var loadEvent in provider.LoadEvents(Client.Graphics.Painter))
        {
            AddLoadEvent(loadEvent);
        }
    }
}