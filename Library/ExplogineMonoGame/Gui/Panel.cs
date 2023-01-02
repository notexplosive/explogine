using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class Panel : IGuiWidget, IPreDrawWidget, IDisposable
{
    public Panel(RectangleF rectangle, Depth depth)
    {
        Rectangle = rectangle;
        Depth = depth;
        Canvas = new Canvas(Rectangle.Size.ToPoint());
    }

    public Canvas Canvas { get; }
    public Gui InnerGui { get; } = new();

    public RectangleF Rectangle { get; }
    public Depth Depth { get; }

    public void Dispose()
    {
        Canvas.Dispose();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var translation = Matrix.CreateTranslation(new Vector3(-Rectangle.TopLeft, 0));
        InnerGui.UpdateInput(input, hitTestStack.AddLayer(translation, Depth));
    }

    public void PreDraw(Painter painter, IGuiTheme uiTheme)
    {
        Client.Graphics.PushCanvas(Canvas);
        InnerGui.PrepareCanvases(painter, uiTheme);        
        painter.BeginSpriteBatch(Matrix.Identity);
        InnerGui.Draw(painter, uiTheme);
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public void Draw(Painter painter, IGuiTheme uiTheme)
    {
        painter.DrawRectangle(Rectangle, new DrawSettings {Depth = Depth, Color = uiTheme.BackgroundColor});
        painter.DrawAsRectangle(Canvas.Texture, Rectangle, new DrawSettings {Depth = Depth - 1});
    }
}
