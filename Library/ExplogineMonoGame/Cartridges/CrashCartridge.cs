using System;
using System.IO;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Cartridges;

public class CrashCartridge : ICartridge
{
    private readonly string _reportText;
    private readonly LazyInitializedFont _font = new("engine/console-font", 32);
    private readonly LazyInitializedFont _titleFont = new("engine/console-font", 100);

    public CrashCartridge(Exception exception)
    {
        Client.Graphics.Painter.ResetToCleanState();

        ThrownException = exception;

        var fileName = "explogine-crash.log";
        var fileInfo = new FileInfo(Path.Join(Client.FileSystem.Local.GetCurrentDirectory(), fileName));
        _reportText =
            $"The program has crashed!\n\nWe're very sorry this happened.\nA copy of this report, and a full log can be found at:\n{fileInfo.FullName}\n\nCrash report:\n{ThrownException.Message}\n\nStacktrace:\n{ThrownException.StackTrace}";
        Client.Debug.Log(_reportText);

        Client.Debug.LogFile.WriteBufferAsFilename(fileName);
    }

    public Exception ThrownException { get; }

    public void OnCartridgeStarted()
    {
    }

    public void Update(float dt)
    {
    }

    public void Draw(Painter painter)
    {
        if (_font == null || _titleFont == null)
        {
            Client.Debug.Log("Font was not loaded, we couldn't render crash cart");
            Client.Exit();
            return;
        }

        painter.BeginSpriteBatch(SamplerState.LinearWrap, Matrix.Identity);

        painter.Clear(Color.DarkBlue);

        painter.DrawStringAtPosition(_titleFont, "heck! :(", Point.Zero, new DrawSettings());

        painter.DrawStringWithinRectangle(_font, _reportText,
            new Rectangle(new Point(0, _titleFont.FontSize), Client.Window.Size), Alignment.TopLeft,
            new DrawSettings());

        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public CartridgeConfig CartridgeConfig { get; } = new();
}
