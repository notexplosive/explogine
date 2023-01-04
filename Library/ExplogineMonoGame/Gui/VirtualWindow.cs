using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public partial class VirtualWindow : IUpdateInputHook, IUpdateHook, IDisposable
{
    public delegate void WindowEvent(VirtualWindow window);

    private readonly Chrome _chrome;
    private readonly Widget _widget;
    private readonly Content _content;

    public VirtualWindow(RectangleF rectangle, Depth depth)
    {
        _widget = new Widget(rectangle, depth - 1);
        _chrome = new Chrome(this, 32, rectangle.Size.ToPoint());
        _content = new Content(this);

    }

    public Canvas Canvas => _widget.Canvas;

    public RectangleF CanvasRectangle => _widget.Rectangle;

    public Depth StartingDepth
    {
        // off-by-one because we want the widget to be the source of truth, but we also want to draw chrome below the widget
        get => _widget.Depth + 1;
        set => _widget.Depth = value - 1;
    }

    public Vector2 Position
    {
        get => _widget.Position;
        set => _widget.Position = value;
    }

    public void Dispose()
    {
        _widget.Dispose();
    }

    public void Update(float dt)
    {
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _widget.UpdateHovered(hitTestStack);
        _chrome.UpdateInput(input, hitTestStack);
        _content.UpdateInput(input, hitTestStack);
    }

    public void Draw(Painter painter, IGuiTheme theme)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.Clear(Color.DarkBlue);
        Client.Graphics.PopCanvas();

        _chrome.Draw(painter, theme);
        _widget.Draw(painter);
    }

    private void SetRectangle(RectangleF resizedRect)
    {
        _widget.Rectangle = resizedRect;
    }

    private void RequestFocus()
    {
        RequestedFocus?.Invoke(this);
    }

    public event WindowEvent? RequestedFocus;
}
