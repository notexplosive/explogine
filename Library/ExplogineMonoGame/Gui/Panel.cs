using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class Panel : IGuiWidget, IPreDrawWidget, IDisposable, IThemed
{
    private Action<ConsumableInput, HitTestStack>? _extraUpdateInputBehavior;

    public Panel(RectangleF rectangle, Depth depth, IGuiTheme theme)
    {
        Rectangle = rectangle;
        Depth = depth;
        Widget = new Widget(rectangle, depth);
        Theme = theme;
    }

    public float ScrollPositionY { get; set; }

    public Widget Widget { get; }

    public Gui InnerGui { get; } = new();

    public RectangleF Rectangle { get; }
    public Depth Depth { get; }

    public void Dispose()
    {
        Widget.Dispose();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var translation = Matrix.CreateTranslation(new Vector3(-Rectangle.TopLeft, 0)) * Matrix.Invert(CameraMatrix());
        var localHitTest = hitTestStack.AddLayer(translation, Depth, Rectangle);
        InnerGui.UpdateInput(input, localHitTest);
        _extraUpdateInputBehavior?.Invoke(input, localHitTest);
    }

    public void PrepareDraw(Painter painter, IGuiTheme uiTheme)
    {
        Client.Graphics.PushCanvas(Widget.Canvas);
        InnerGui.PrepareCanvases(painter, Theme);
        painter.Clear(uiTheme.BackgroundColor);
        painter.BeginSpriteBatch(CameraMatrix());
        InnerGui.Draw(painter, Theme);
        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    public IGuiTheme Theme { get; }

    private Matrix CameraMatrix()
    {
        return Matrix.CreateTranslation(new Vector3(0, -ScrollPositionY, 0));
    }

    public void Draw(Painter painter)
    {
        Widget.Draw(painter);
    }

    public void AddUpdateInputBehavior(Action<ConsumableInput, HitTestStack> behavior)
    {
        _extraUpdateInputBehavior += behavior;
    }
}
