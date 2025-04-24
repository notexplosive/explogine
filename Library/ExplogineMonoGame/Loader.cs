using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public delegate Asset LoadEventFunction();

public class Loader
{
    private readonly ContentManager? _content;
    private readonly List<ILoadEvent> _loadEvents = new();
    private readonly IRuntime _runtime;
    private int _loadEventIndex;
    private List<Task> _pendingTasks = new();

    public Loader(IRuntime runtime, ContentManager? content)
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

    public string NextStatus
    {
        get
        {
            if (_loadEvents.IsValidIndex(_loadEventIndex))
            {
                return _loadEvents[_loadEventIndex].Info ?? _loadEvents[_loadEventIndex].Key;
            }

            return "Loading";
        }
    }

    public void ForceLoadVoid(string key)
    {
        ILoadEvent? foundLoadEvent = null;
        foreach (var loadEvent in _loadEvents)
        {
            if (loadEvent.Key == key)
            {
                foundLoadEvent = loadEvent;
            }
        }

        if (foundLoadEvent != null)
        {
            _loadEvents.Remove(foundLoadEvent);
            foundLoadEvent.Execute();
        }
    }

    public T ForceLoadAsset<T>(string key) where T : Asset
    {
        if (_content == null)
        {
            Client.Debug.LogVerbose("This loader doesn't have static content, trying to load from cache");
            return Client.Assets.GetAsset<T>(key);
        }
        
        Client.Debug.LogVerbose($"ForceLoad: {key}");
        if (HasExecutedAllEvents())
        {
            Client.Debug.LogVerbose("Already cached, returning");
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

        if (foundLoadEvent != null)
        {
            _loadEvents.Remove(foundLoadEvent);
        }

        if (foundLoadEvent is AssetLoadEvent assetLoadEvent)
        {
            Client.Debug.LogVerbose("Found load event, running");
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

    public bool HasExecutedAllEvents()
    {
        return _loadEventIndex >= LoadEventCount;
    }

    public bool IsFullyDone()
    {
        return HasExecutedAllEvents() && AllPendingTasksFinished();
    }

    private bool AllPendingTasksFinished()
    {
        foreach (var task in _pendingTasks)
        {
            if (!task.IsCompleted)
            {
                return false;
            }
        }

        return true;
    }

    public void LoadNext()
    {
        var currentLoadEvent = _loadEvents[_loadEventIndex];

        if (currentLoadEvent is ThreadedVoidLoadEvent threadedEvent)
        {
            _pendingTasks.Add(threadedEvent.ExecuteThreaded());
        }
        else
        {
            currentLoadEvent.Execute();
        }

        _loadEventIndex++;
    }

    private IEnumerable<AssetLoadEvent> StaticContentLoadEvents()
    {
        foreach (var key in GetKeysFromContentDirectory())
        {
            yield return new AssetLoadEvent(key, key, () =>
            {
                Client.Debug.LogVerbose($"Attempting to load static content asset: {key}");
                return LoadStaticAsset(key);
            });
        }
    }

    private Asset LoadStaticAsset(string key)
    {
        var texture2D = AttemptLoadStatic<Texture2D>(key);
        if (texture2D != null)
        {
            return new TextureAsset(texture2D);
        }

        var soundEffect = AttemptLoadStatic<SoundEffect>(key);
        if (soundEffect != null)
        {
            return new SoundAsset(soundEffect);
        }

        var spriteFont = AttemptLoadStatic<SpriteFont>(key);
        if (spriteFont != null)
        {
            return new SpriteFontAsset(spriteFont);
        }

        var effect = AttemptLoadStatic<Effect>(key);
        if (effect != null)
        {
            return new EffectAsset(effect);
        }

        throw new Exception($"Unsupported/Unidentified Asset: {key}");
    }

    private T? AttemptLoadStatic<T>(string key) where T : class
    {
        try
        {
            return _content?.Load<T>(key);
        }
        catch (InvalidCastException)
        {
            return null;
        }
    }

    private string[] GetKeysFromContentDirectory()
    {
        if (_content == null)
        {
            return Array.Empty<string>();
        }
        
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
        _content?.Unload();
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

    public void AddLoadEventsFromCartridge(Cartridge cartridge)
    {
        if (cartridge is ILoadEventProvider loadEventProvider)
        {
            foreach (var loadEvent in loadEventProvider.LoadEvents(Client.Graphics.Painter))
            {
                if (loadEvent != null)
                {
                    AddLoadEvent(loadEvent);
                }
            }
        }
    }

    public event Action? BeforeLoadItem;

    public event Action? AfterLoadItem;
    
    public void LoadNextChunkOfItems()
    {
        var expectedFrameDuration = 1 / 60f;

        // If we dedicate the whole frame to loading we'll effectively block on the UI thread.
        // If we leave a tiny bit of headroom then on most frames we can still do UI operations
        // (such as move the window) during the loading screen
        var percentOfFrameAllocatedForLoading = 0.5f;

        var maxTime = expectedFrameDuration * percentOfFrameAllocatedForLoading;

        var timeAtStartOfUpdate = DateTime.Now;
        while (!HasExecutedAllEvents())
        {
            BeforeLoadItem?.Invoke();
            
            LoadNext();

            AfterLoadItem?.Invoke();

            var timeSpentLoading = DateTime.Now - timeAtStartOfUpdate;
            if (timeSpentLoading.TotalSeconds > maxTime)
            {
                break;
            }
        }
    }
}
