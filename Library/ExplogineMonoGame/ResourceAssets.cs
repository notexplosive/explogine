using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExplogineCore;
using ExplogineCore.Aseprite;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ExplogineMonoGame;

public class ResourceAssets
{
    private static ResourceAssets? instanceImpl;

    private readonly Dictionary<string, Canvas?> _dynamicTextures = new();
    private readonly Dictionary<string, SpriteSheet?> _sheets = new();
    private readonly Dictionary<string, SoundEffect?> _soundEffects = new();
    private readonly Dictionary<string, SoundEffectInstance?> _soundInstances = new();

    public static ResourceAssets Instance => instanceImpl ??= new ResourceAssets();

    public Canvas? GetDynamicTexture(string key)
    {
        return _dynamicTextures.GetValueOrDefault(key);
    }

    public SpriteSheet? GetSpriteSheet(string key)
    {
        return _sheets.GetValueOrDefault(key);
    }

    public SoundEffect? GetSoundEffect(string key)
    {
        return _soundEffects.GetValueOrDefault(key);
    }

    public SoundEffectInstance? GetSoundEffectInstance(string key)
    {
        return _soundInstances.GetValueOrDefault(key);
    }

    public IEnumerable<ILoadEvent> LoadEvents(Painter painter)
    {
        var resourceFiles = Client.Debug.RepoFileSystem.GetDirectory("Resource");

        yield return new VoidLoadEvent("sprite-atlas", "Sprite Atlas", () =>
        {
            var texturePath = Path.Join(resourceFiles.GetCurrentDirectory(), "atlas.png");
            var texture = Texture2D.FromFile(Client.Graphics.Device, texturePath);
            var sheetInfo = JsonConvert.DeserializeObject<AsepriteSheetData>(resourceFiles.ReadFile("atlas.json"));

            if (sheetInfo != null)
            {
                foreach (var frame in sheetInfo.Frames)
                {
                    // Remove extension
                    var splitSheetName =
                        frame.Key
                            .Replace(".aseprite", "")
                            .Replace(".ase", "")
                            .Replace(".png", "")
                            .Split(" ").ToList();

                    if (splitSheetName.Count > 1)
                    {
                        // If there is a number suffix, remove it
                        splitSheetName.RemoveAt(splitSheetName.Count - 1);
                    }

                    var sheetName = string.Join(" ", splitSheetName);
                    if (!_sheets.ContainsKey(sheetName))
                    {
                        _sheets.Add(sheetName, new SelectFrameSpriteSheet(texture));
                    }

                    var rect = frame.Value.Frame;
                    (_sheets[sheetName] as SelectFrameSpriteSheet)!.AddFrame(new Rectangle(rect.X, rect.Y, rect.Width,
                        rect.Height));
                }
            }
        });

        yield return new VoidLoadEvent("Sound Effects", () =>
        {
            foreach (var path in resourceFiles.GetFilesAt(".", "ogg"))
            {
                AddOggSound(resourceFiles, path.RemoveFileExtension());
            }

            foreach (var path in resourceFiles.GetFilesAt(".", "wav"))
            {
                AddWavSound(resourceFiles, path.RemoveFileExtension());
            }
        });
    }

    public void Unload()
    {
        Unload(_dynamicTextures);
        Unload(_soundEffects);
        Unload(_soundInstances);
    }

    private void Unload<T>(Dictionary<string, T> dictionary) where T : IDisposable?
    {
        foreach (var sound in dictionary.Values)
        {
            sound?.Dispose();
        }

        dictionary.Clear();
    }

    public void AddDynamicSpriteSheet(string key, Point size, Action generateTexture,
        Func<Texture2D, SpriteSheet?> generateSpriteSheet)
    {
        if (_dynamicTextures.ContainsKey(key))
        {
            _dynamicTextures[key]?.Dispose();
            _dynamicTextures.Remove(key);
        }

        var canvas = new Canvas(size.X, size.Y);
        _dynamicTextures.Add(key, canvas);

        Client.Graphics.PushCanvas(canvas);
        generateTexture();
        Client.Graphics.PopCanvas();

        _sheets.Add(key, generateSpriteSheet(canvas.Texture));
    }

    public void AddOggSound(IFileSystem resourceFiles, string path)
    {
        var vorbis = ReadOgg.ReadVorbis(Path.Join(resourceFiles.GetCurrentDirectory(), path + ".ogg"));
        var soundEffect = ReadOgg.ReadSoundEffect(vorbis);
        _soundInstances[path] = soundEffect.CreateInstance();
        _soundEffects[path] = soundEffect;
    }

    public void AddWavSound(IFileSystem resourceFiles, string path)
    {
        var soundEffect = SoundEffect.FromFile(Path.Join(resourceFiles.GetCurrentDirectory(), path + ".wav"));
        _soundInstances[path] = soundEffect.CreateInstance();
        _soundEffects[path] = soundEffect;
    }

    public void PlaySound(string key, SoundEffectSettings settings)
    {
        var sound = GetSoundEffectInstance(key);

        if (sound != null)
        {
            if (settings.Cached)
            {
                sound.Stop();
            }

            sound.Pan = settings.Pan;
            sound.Pitch = settings.Pitch;
            sound.Volume = settings.Volume;
            sound.IsLooped = settings.Loop;

            sound.Play();
        }
        else
        {
            Client.Debug.LogWarning($"Could not find sound `{key}`");
        }
    }

    public static void Reset()
    {
        Instance.Unload();
        instanceImpl = null;
    }
}
