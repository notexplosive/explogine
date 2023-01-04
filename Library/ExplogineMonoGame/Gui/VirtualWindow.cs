using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class VirtualWindow : IUpdateInputHook, IUpdateHook, IDisposable
{
    private readonly Chrome _chrome;
    private readonly Widget _widget;
    private readonly HoverState _windowHoverState = new();

    public VirtualWindow(RectangleF rectangle, Depth depth)
    {
        _widget = new Widget(rectangle, depth - 1);
        _chrome = new Chrome(this, 32, rectangle.Size.ToPoint());
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

    public void Draw(Painter painter, IGuiTheme theme)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.Clear(Color.DarkBlue);
        Client.Graphics.PopCanvas();

        _chrome.Draw(painter, theme);
        _widget.Draw(painter);
    }

    public void Update(float dt)
    {
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(_widget.Rectangle, StartingDepth, _windowHoverState);
        _widget.UpdateHovered(hitTestStack);
        _chrome.UpdateInput(input, hitTestStack);
    }

    private void SetRectangle(RectangleF resizedRect)
    {
        _widget.Rectangle = resizedRect;
    }

    public class Chrome : IUpdateInputHook
    {
        private readonly HoverState _headerHovered = new();
        private readonly Point _minimumSize;
        private readonly Drag<Vector2> _movementDrag;
        private readonly VirtualWindow _parentWindow;
        private readonly RectResizer _rectResizer;
        private readonly int _titleBarThickness;
        private RectangleF? _pendingResizeRect;

        public Chrome(VirtualWindow parentWindow, int titleBarThickness, Point minimumSize)
        {
            _parentWindow = parentWindow;
            _titleBarThickness = titleBarThickness;
            _minimumSize = minimumSize + new Point(0, titleBarThickness);
            _rectResizer = new RectResizer();
            _movementDrag = new Drag<Vector2>();
        }

        private RectangleF CanvasRectangle => _parentWindow.CanvasRectangle;
        public Depth Depth => _parentWindow.StartingDepth;

        public RectangleF TitleBarRectangle
        {
            get
            {
                var canvasRect = CanvasRectangle;
                return new RectangleF(new Vector2(canvasRect.X, canvasRect.Y - _titleBarThickness),
                    new Vector2(canvasRect.Width, _titleBarThickness));
            }
        }

        public RectangleF WholeWindowRectangle => RectangleF.Union(TitleBarRectangle, CanvasRectangle);

        public void Draw(Painter painter, IGuiTheme theme)
        {
            theme.DrawWindowChrome(painter, this);

            if (_pendingResizeRect.HasValue)
            {
                painter.DrawLineRectangle(_pendingResizeRect.Value,
                    new LineDrawSettings {Depth = Depth.Front + 100, Color = Color.White});
            }
        }

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            var resizedWholeWindowRect =
                _rectResizer.GetResizedRect(input, hitTestStack, WholeWindowRectangle, Depth, 10);

            if (_rectResizer.HasGrabbed)
            {
                _pendingResizeRect = resizedWholeWindowRect;
            }
            else
            {
                if (_pendingResizeRect.HasValue)
                {
                    // We need to reduce this back down to just the canvas, any inflation due to chrome needs to be undone
                    var canvasRect =
                        _pendingResizeRect.Value.ResizedOnEdge(RectEdge.Top, new Vector2(0, _titleBarThickness));
                    _parentWindow.SetRectangle(canvasRect);
                    _pendingResizeRect = null;
                }
            }

            if (_movementDrag.IsDragging)
            {
                _parentWindow.Position = _movementDrag.StartingValue + _movementDrag.TotalDelta;
            }

            _movementDrag.AddDelta(input.Mouse.Delta(hitTestStack.WorldMatrix));

            hitTestStack.AddZone(TitleBarRectangle, Depth, _headerHovered);

            if (_headerHovered && input.Mouse.GetButton(MouseButton.Left).WasPressed)
            {
                _movementDrag.Start(_parentWindow.Position);
            }

            if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
            {
                _movementDrag.End();
            }
        }
    }
}
