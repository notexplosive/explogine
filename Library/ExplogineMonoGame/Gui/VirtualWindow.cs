using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public partial class VirtualWindow : IUpdateInputHook, IDisposable
{
    public delegate void WindowEvent(VirtualWindow window);

    private readonly Body _body;

    private readonly Chrome _chrome;
    private readonly Widget _widget;

    public VirtualWindow(RectangleF rectangle, IWindowSizeSettings windowSizeSettings, IWindowContent content,
        Depth depth)
    {
        _widget = new Widget(rectangle, depth - 1);
        _chrome = new Chrome(this, 32, rectangle.Size.ToPoint(), windowSizeSettings);
        _body = new Body(this, content);
    }

    public Canvas Canvas => _widget.Canvas;
    public RectangleF CanvasRectangle => _widget.Rectangle;
    public RectangleF WholeRectangle => _chrome.WholeWindowRectangle;
    public RectangleF TitleBarRectangle => _chrome.TitleBarRectangle;

    public Depth StartingDepth
    {
        // off-by-one because we want the widget to be the source of truth, but we also want to draw chrome below the widget
        get => _widget.Depth + 1;
        set => _widget.Depth = value - 1;
    }

    public Vector2 Position
    {
        get => WholeRectangle.Location;
        set => _widget.Position = value + new Vector2(0, TitleBarRectangle.Height);
    }

    public void Dispose()
    {
        _widget.Dispose();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _widget.UpdateHovered(hitTestStack);
        _chrome.UpdateInput(input, hitTestStack);
        _body.UpdateInput(input, hitTestStack);
    }

    public event WindowEvent? RequestedFocus;
    public event WindowEvent? RequestedConstrainToBounds;

    public void Draw(Painter painter, IGuiTheme theme, bool isInFocus)
    {
        _chrome.Draw(painter, theme, isInFocus);
        _widget.Draw(painter);
    }
    
    public void DrawContent(Painter painter)
    {
        _body.Content.Draw(painter);
    }

    private void SetRectangle(RectangleF resizedRect)
    {
        _widget.Rectangle = resizedRect;
    }

    private void RequestFocus()
    {
        RequestedFocus?.Invoke(this);
    }

    private void ValidateBounds()
    {
        RequestedConstrainToBounds?.Invoke(this);
    }
}
