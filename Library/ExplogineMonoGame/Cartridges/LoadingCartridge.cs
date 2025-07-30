using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class LoadingCartridge : Cartridge
{
    private const int ProgressBarHeight = 40;
    private const int ProgressBarWidth = 400;
    private const float RingBufferSize = 5;

    private readonly Font _font;
    private readonly Loader _loader;
    private readonly LinkedList<string> _statusRingBuffer;
    private bool _doneLoading;
    private int _endingDelayFrames = 10;
    private int _startingDelayFrames = 5;

    public LoadingCartridge(IRuntime runtime, Loader loader) : base(runtime)
    {
        _loader = loader;

        var spriteFont = loader.ForceLoadAsset<SpriteFontAsset>("engine/console-font");
        loader.ForceLoadAsset<TextureAsset>("white-pixel");
        _font = new Font(spriteFont.SpriteFont, 32);
        _statusRingBuffer = new LinkedList<string>();

        _loader.BeforeLoadItem += BeforeLoadItem;
        _loader.AfterLoadItem += AfterLoadItem;
    }

    public override CartridgeConfig CartridgeConfig { get; } = new();

    private void BeforeLoadItem()
    {
        if (_statusRingBuffer.First?.Value != _loader.NextStatus)
        {
            _statusRingBuffer.AddFirst(_loader.NextStatus);
        }
    }

    private void AfterLoadItem()
    {
        while (_statusRingBuffer.Count > RingBufferSize)
        {
            _statusRingBuffer.RemoveLast();
        }
    }

    public override void OnCartridgeStarted()
    {
    }

    public override void Update(float dt)
    {
        if (_startingDelayFrames > 0)
        {
            return;
        }

        _loader.LoadNextChunkOfItems();

        if (_loader.IsFullyDone())
        {
            _doneLoading = true;
        }
    }

    public override void Draw(Painter painter)
    {
        // main canvas draw
        var loadingBarRect =
            RectangleF.FromSizeAlignedWithin(
                Runtime.Window.RenderResolution.ToRectangleF(),
                new Vector2(ProgressBarWidth, ProgressBarHeight), Alignment.Center);

        var loadingBarFillRect =
            RectangleF.FromSizeAlignedWithin(
                Runtime.Window.RenderResolution.ToRectangleF(),
                new Vector2(ProgressBarWidth, ProgressBarHeight), Alignment.Center);

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

        var bufferSize = RingBufferSize;
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
        if (_loader.IsFullyDone())
        {
            if (_endingDelayFrames > 0)
            {
                _endingDelayFrames--;
                return;
            }
        }
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public override bool ShouldLoadNextCartridge()
    {
        return _doneLoading;
    }

    public override void Unload()
    {
    }
}
