using System;
using Microsoft.Xna.Framework;
using NotCore.AssetManagement;

namespace NotCore.Cartridges;

public class LoadingCartridge : ICartridge, ICommandLineParameterProvider
{
    private const int ProgressBarHeight = 40;
    private const int ProgressBarWidth = 400;
    private readonly Loader _loader;
    private readonly Canvas _loadingBarGraphic;
    private readonly Canvas _progressSliceGraphic;

    public LoadingCartridge(Loader loader)
    {
        _loader = loader;

        _progressSliceGraphic = new Canvas(1, LoadingCartridge.ProgressBarHeight);
        _loadingBarGraphic = new Canvas(LoadingCartridge.ProgressBarWidth, LoadingCartridge.ProgressBarHeight);
        var painter = Client.Graphics.Painter;

        Client.Graphics.PushCanvas(_progressSliceGraphic);
        painter.Clear(Color.Yellow);
        Client.Graphics.PopCanvas();
    }

    public void Update(float dt)
    {
        var expectedFrameDuration = 1 / 60f;
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
            Client.TriggerDoneLoading();
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
            painter.Draw(_progressSliceGraphic.Texture, new Vector2(i, 0));
        }

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();

        // main canvas draw
        painter.Clear(Color.Black);
        painter.BeginSpriteBatch();
        painter.Draw(_loadingBarGraphic.Texture,
            Client.Graphics.ScreenSize.ToVector2() / 2 - _loadingBarGraphic.Texture.Bounds.Size.ToVector2() / 2f);
        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return _loader.IsDone();
    }

    public void SetupFormalParameters(CommandLineArguments args)
    {
        args.AddParameter<bool>("fullscreen");
        args.AddParameter<bool>("skipsnapshot");
        args.AddParameter<string>("demo");
    }

    ~LoadingCartridge()
    {
        _loadingBarGraphic.Dispose();
        _progressSliceGraphic.Dispose();
    }
}
