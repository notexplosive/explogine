using System;
using System.IO;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Cartridges;

public class CrashCartridge : ICartridge
{
    private readonly string _reportText;
    private Font? _font;

    public CrashCartridge(Exception exception)
    {
        Client.Graphics.Painter.ResetToCleanState();
        
        ThrownException = exception;

        var fileName = "explogine-crash.log";
        var fileInfo = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), fileName));
        _reportText =
            $"The game has crashed!\n\nWe're very sorry this happened.\nA copy of this report, and a full log can be found at:\n{fileInfo.FullName}\n\nCrash report:\n{ThrownException.Message}\n\nStacktrace:\n{ThrownException.StackTrace}";
        Client.Debug.Log(_reportText);

        Client.Debug.LogFile.WriteBufferAsFilename(fileInfo.FullName);
    }

    public Exception ThrownException { get; }

    public void OnCartridgeStarted()
    {
        _font = Client.Assets.GetFont("engine/console-font", 32);
    }

    public void Update(float dt)
    {
    }

    public void Draw(Painter painter)
    {
        if (_font == null)
        {
            Client.Debug.Log("Font was not loaded, we couldn't render crash cart");
            Client.Exit();
            return;
        }

        painter.BeginSpriteBatch(SamplerState.LinearWrap, Matrix.Identity);

        painter.DrawStringWithinRectangle(_font, _reportText, new Rectangle(Point.Zero, Client.Window.Size),
            new DrawSettings());

        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }
}
