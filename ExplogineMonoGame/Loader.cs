﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public delegate IAsset LoadEvent();

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

    public bool IsDone()
    {
        return _loadEventIndex >= LoadEventCount;
    }

    public void LoadNext()
    {
        var asset = _loadEvents[_loadEventIndex]();
        Client.Assets.AddAsset(asset);
        _loadEventIndex++;
        Console.WriteLine("Loading: " + MathF.Floor(Percent * 100f) + "%");
    }

    private IEnumerable<LoadEvent> StaticContentLoadEvents()
    {
        foreach (var key in Loader.GetKeysFromContentDirectory())
        {
            yield return () => LoadAsset(key);
        }
    }

    private IAsset LoadAsset(string key)
    {
        var texture2D = AttemptLoad<Texture2D>(key);
        if (texture2D != null)
        {
            return new TextureAsset(key, texture2D);
        }

        var soundEffect = AttemptLoad<SoundEffect>(key);
        if (soundEffect != null)
        {
            return new SoundAsset(key, soundEffect);
        }

        var spriteFont = AttemptLoad<SpriteFont>(key);
        if (spriteFont != null)
        {
            return new SpriteFontAsset(key, spriteFont);
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

    public void AddDynamicLoadEvent(LoadEvent loadEvent)
    {
        _loadEvents.Add(loadEvent);
    }
}