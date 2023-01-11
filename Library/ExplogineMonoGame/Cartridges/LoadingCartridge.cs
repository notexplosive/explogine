﻿using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class LoadingCartridge : ICartridge
{
    private const int ProgressBarHeight = 40;
    private const int ProgressBarWidth = 400;
    private const float RingBufferSize = 5;

    private readonly Font _font;
    private readonly Loader _loader;
    private readonly LinkedList<string> _statusRingBuffer;
    private bool _doneLoading;
    private int _endingDelayFrames = 10;
    private int _startingDelayFrames = 10;

    public LoadingCartridge(Loader loader)
    {
        _loader = loader;

        var spriteFont = loader.ForceLoad<SpriteFontAsset>("engine/console-font");
        loader.ForceLoad<TextureAsset>("white-pixel");
        _font = new Font(spriteFont.SpriteFont, 32);
        _statusRingBuffer = new LinkedList<string>();
    }

    public CartridgeConfig CartridgeConfig { get; } = new();

    public void OnCartridgeStarted()
    {
        Client.FinishedLoading.Add(() => _doneLoading = true);
    }

    public void Update(float dt)
    {
        if (_startingDelayFrames > 0)
        {
            return;
        }
        
        var expectedFrameDuration = 1 / 60f;

        // If we dedicate the whole frame to loading we'll effectively block on the UI thread.
        // If we leave a tiny bit of headroom then on most frames we can still do UI operations
        // (such as move the window) during the loading screen
        var percentOfFrameAllocatedForLoading = 0.9f;

        var maxTime = expectedFrameDuration * percentOfFrameAllocatedForLoading;

        var timeAtStartOfUpdate = DateTime.Now;
        var itemsLoadedThisCycle = 0;
        while (!_loader.IsDone())
        {
            _loader.LoadNext();

            _statusRingBuffer.AddFirst(_loader.Status);
            while (_statusRingBuffer.Count > LoadingCartridge.RingBufferSize)
            {
                _statusRingBuffer.RemoveLast();
            }

            var timeSpentLoading = DateTime.Now - timeAtStartOfUpdate;
            itemsLoadedThisCycle++;
            if (timeSpentLoading.TotalSeconds > maxTime)
            {
                break;
            }
        }
    }

    public void Draw(Painter painter)
    {
        // main canvas draw
        var loadingBarRect =
            RectangleF.FromSizeAlignedWithin(
                Client.Window.RenderResolution.ToRectangleF(),
                new Vector2(LoadingCartridge.ProgressBarWidth, LoadingCartridge.ProgressBarHeight), Alignment.Center);

        var loadingBarFillRect =
            RectangleF.FromSizeAlignedWithin(
                Client.Window.RenderResolution.ToRectangleF(),
                new Vector2(LoadingCartridge.ProgressBarWidth, LoadingCartridge.ProgressBarHeight), Alignment.Center);

        loadingBarFillRect.Size = new Vector2(loadingBarFillRect.Size.X * _loader.Percent, loadingBarFillRect.Size.Y);

        painter.Clear(Color.Black);
        painter.BeginSpriteBatch();
        painter.DrawRectangle(loadingBarRect, new DrawSettings {Color = Color.White, Depth = Depth.Middle});

#if DEBUG
        var fillColor = Color.LightBlue;
#else
        var fillColor = Color.Orange;
#endif

        painter.DrawRectangle(loadingBarFillRect, new DrawSettings {Color = fillColor, Depth = Depth.Middle - 1});

        painter.DrawStringWithinRectangle(_font, MathF.Floor(_loader.Percent * 100f) + "%",
            loadingBarRect, Alignment.Center, new DrawSettings {Color = Color.Black});

        var fragments = new List<FormattedText.IFragment>();

        var bufferSize = LoadingCartridge.RingBufferSize;
        var itemIndex = 0;
        foreach (var item in _statusRingBuffer)
        {
            fragments.Add(new FormattedText.Fragment(_font, item + "\n",
                Color.White.WithMultipliedOpacity(1 - itemIndex / bufferSize)));
            itemIndex++;
        }

        var formattedText = new FormattedText(fragments.ToArray());

        painter.DrawFormattedStringWithinRectangle(formattedText,
            loadingBarRect.Moved(new Vector2(0, 128)), Alignment.TopCenter, new DrawSettings {Color = Color.White});
        painter.EndSpriteBatch();

        // Wait a few frames before we start so we're certain the window is totally ready and is drawing content
        if (_startingDelayFrames > 0)
        {
            _startingDelayFrames--;
            return;
        }
        
        // Wait until we've drawn a few frames of the finished loading bar before we move on
        // This way we make sure we're at a stable framerate before we start the game in earnest.
        if (_loader.IsDone())
        {
            if (_endingDelayFrames > 0)
            {
                _endingDelayFrames--;
                return;
            }

            Client.FinishedLoading.BecomeReady();
        }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return _doneLoading;
    }

    public void Unload()
    {
    }
}
