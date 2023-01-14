using System;
using System.IO;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class CrashCartridge : Cartridge
{
    private readonly IndirectFont _font = new("engine/console-font", 32);
    private readonly string _reportText;
    private readonly IndirectFont _titleFont = new("engine/console-font", 100);

    public CrashCartridge(App app, Exception exception) : base(app)
    {
        Client.Graphics.Painter.ResetToCleanState();

        ThrownException = exception;

        var fileName = "explogine-crash.log";
        var fileInfo = new FileInfo(Path.Join(app.FileSystem.Local.GetCurrentDirectory(), fileName));
        _reportText =
            $"The program has crashed!\n\nWe're very sorry this happened.\nA copy of this report, and a full log can be found at:\n{fileInfo.FullName}\n\nCrash report:\n{ThrownException.Message}\n\nStacktrace:\n{ThrownException.StackTrace}";
        Client.Debug.LogError(_reportText);

        Client.Debug.LogFile.WriteBufferAsFilename(fileName);
    }

    public Exception ThrownException { get; }

    public override void OnCartridgeStarted()
    {
    }

    public override void Update(float dt)
    {
    }
    
    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public override void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(Matrix.Identity);

        painter.Clear(Color.DarkBlue);

        var rect = new Rectangle(new Point(0, 0), App.Window.Size);
        rect.Inflate(-10, -10);

        painter.DrawStringWithinRectangle(_titleFont, "heck! :(", rect, Alignment.TopLeft, new DrawSettings());
        rect.Height -= _titleFont.FontSize;
        rect.Location += new Point(0,_titleFont.FontSize);
        painter.DrawStringWithinRectangle(_font, _reportText,
            rect, Alignment.TopLeft,
            new DrawSettings());

        painter.EndSpriteBatch();
    }

    public override bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public override void Unload()
    {
    }

    public override CartridgeConfig CartridgeConfig { get; } = new();
}
