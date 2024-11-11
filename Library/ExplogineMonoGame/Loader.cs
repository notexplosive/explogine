using System;
using System.Collections.Generic;
using System.IO;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public delegate Asset LoadEventFunction();

public class Loader
{
    private readonly IRuntime _runtime;
    private readonly ContentManager _content;
    private readonly List<ILoadEvent> _loadEvents = new();
    private int _loadEventIndex;

    public Loader(IRuntime runtime, ContentManager content)
    {
        _runtime = runtime;
        _content = content;
        foreach (var loadEvent in StaticContentLoadEvents())
        {
            _loadEvents.Add(loadEvent);
        }
    }

    private int LoadEventCount => _loadEvents.Count;
    public float Percent => (float) _loadEventIndex / LoadEventCount;
    public string NextStatus { get; private set; } = "Loading";

    public T ForceLoad<T>(string key) where T : Asset
    {
        if (IsDone())
        {
            return Client.Assets.GetAsset<T>(key);
        }

        ILoadEvent? foundLoadEvent = null;
        foreach (var loadEvent in _loadEvents)
        {
            if (loadEvent.Key == key)
            {
                foundLoadEvent = loadEvent;
            }
        }

        if (foundLoadEvent is AssetLoadEvent assetLoadEvent)
        {
            _loadEvents.Remove(assetLoadEvent);
            var asset = assetLoadEvent.ExecuteAndReturnAsset();
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
        var currentLoadEvent = _loadEvents[_loadEventIndex];
        currentLoadEvent.Execute();

        _loadEventIndex++;

        if (_loadEventIndex < _loadEvents.Count)
        {
            NextStatus = _loadEvents[_loadEventIndex].Info ?? _loadEvents[_loadEventIndex].Key;
        }
        else
        {
            NextStatus = "Done!";
        }
    }

    private IEnumerable<AssetLoadEvent> StaticContentLoadEvents()
    {
        foreach (var key in GetKeysFromContentDirectory())
        {
            yield return new AssetLoadEvent(key, key, () => LoadAsset(key));
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

        var effect = AttemptLoad<Effect>(key);
        if (effect != null)
        {
            return new EffectAsset(effect);
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

    private string[] GetKeysFromContentDirectory()
    {
        Client.Debug.LogVerbose($"Scanning for Content at {_content.RootDirectory}");
        
        var fileNames = _runtime.FileSystem.Local.GetFilesAt(Client.ContentBaseDirectory, "xnb");
        var keys = new List<string>();

        foreach (var fileName in fileNames)
        {
            Client.Debug.LogVerbose($"Found Content: {fileName}");
            var extension = new FileInfo(fileName).Extension;
            // Remove `.xnb`
            var withoutExtension = fileName.Substring(0, fileName.Length - extension.Length);
            // Remove `Content/`
            var withoutPrefix = withoutExtension.Substring(Client.ContentBaseDirectory.Length + 1);
            Client.Debug.LogVerbose($"Keying as: {withoutPrefix}");
            keys.Add(withoutPrefix);
        }

        return keys.ToArray();
    }

    public void Unload()
    {
        _content.Unload();
    }

    public void AddLoadEvent(ILoadEvent assetLoadEvent)
    {
        _loadEvents.Add(assetLoadEvent);
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
            if (loadEvent != null)
            {
                AddLoadEvent(loadEvent);
            }
        }
    }
}
