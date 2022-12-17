using System;
using ExplogineCore;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.HitTesting;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class LoadingCartridge : ICartridge
{
    private const int ProgressBarHeight = 40;
    private const int ProgressBarWidth = 400;
    private readonly Font _font;
    private readonly Loader _loader;
    private readonly Canvas _loadingBarGraphic;
    private readonly Canvas _progressSliceGraphic;
    private bool _doneLoading;
    private float _endingDelay = 0.25f;
    private float _startingDelay = 0.25f;

    public LoadingCartridge(Loader loader)
    {
        _loader = loader;

        _progressSliceGraphic = new Canvas(1, LoadingCartridge.ProgressBarHeight);
        _loadingBarGraphic = new Canvas(LoadingCartridge.ProgressBarWidth, LoadingCartridge.ProgressBarHeight);
        var painter = Client.Graphics.Painter;

        Client.Graphics.PushCanvas(_progressSliceGraphic);
        painter.Clear(Color.LightBlue);
        Client.Graphics.PopCanvas();

        var spriteFont = loader.ForceLoad<SpriteFontAsset>("engine/console-font");
        _font = new Font(spriteFont.SpriteFont, 24);
    }

    public CartridgeConfig CartridgeConfig { get; } = new();

    public void OnCartridgeStarted()
    {
        Client.FinishedLoading.Add(() => _doneLoading = true);
    }

    public void Update(float dt)
    {
        if (_startingDelay > 0)
        {
            _startingDelay -= dt;
            return;
        }

        var expectedFrameDuration = 1 / 30f;
        var timeAtStartOfUpdate = DateTime.Now;
        while (!_loader.IsDone())
        {
            _loader.LoadNext();
            var timeSpentLoading = DateTime.Now - timeAtStartOfUpdate;
            if (timeSpentLoading.TotalSeconds > expectedFrameDuration)
            {
                break;
            }
        }

        if (_loader.IsDone())
        {
            if (_endingDelay > 0)
            {
                _endingDelay -= dt;
                return;
            }

            Client.FinishedLoading.BecomeReady();
        }
    }

    public void Draw(Painter painter)
    {
        // draw the loading bar
        Client.Graphics.PushCanvas(_loadingBarGraphic);
        painter.Clear(Color.White);
        painter.BeginSpriteBatch();

        for (var i = 0; i < LoadingCartridge.ProgressBarWidth * _loader.Percent; i++)
        {
            painter.DrawAtPosition(_progressSliceGraphic.Texture, new Vector2(i, 0));
        }

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();

        // main canvas draw
        painter.Clear(Color.Black);
        painter.BeginSpriteBatch();
        painter.DrawAtPosition(_loadingBarGraphic.Texture,
            Client.Window.RenderResolution.ToVector2() / 2 - _loadingBarGraphic.Texture.Bounds.Size.ToVector2() / 2f);

        painter.DrawStringAtPosition(_font, "Loading...", Vector2.Zero, new DrawSettings());
        painter.EndSpriteBatch();
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return _doneLoading;
    }

    public void Unload()
    {
    }

    ~LoadingCartridge()
    {
        _loadingBarGraphic.Dispose();
        _progressSliceGraphic.Dispose();
    }
}
