using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

partial class VirtualWindow
{
    public class Chrome : IUpdateInputHook
    {
        private readonly Clickable _headerClickable = new();
        private readonly HoverState _headerHovered = new();

        private readonly Point _minimumSize;
        private readonly Drag<Vector2> _movementDrag;
        private readonly VirtualWindow _parentWindow;
        private readonly RectResizer _rectResizer;
        private readonly ISizeSettings _sizeSettings;
        private readonly TitleBar _titleBar;
        private readonly int _titleBarThickness;
        private RectangleF? _pendingResizeRect;

        public Chrome(VirtualWindow parentWindow, int titleBarThickness, Point minimumWidgetSize,
            ISizeSettings sizeSettings)
        {
            _parentWindow = parentWindow;
            _titleBarThickness = titleBarThickness;
            _sizeSettings = sizeSettings;
            _minimumSize = minimumWidgetSize + new Point(0, titleBarThickness);
            _rectResizer = new RectResizer();
            _movementDrag = new Drag<Vector2>();
            _titleBar = new TitleBar(_parentWindow, this);

            _headerClickable.ClickInitiated += parentWindow.RequestFocus;
            _rectResizer.Initiated += parentWindow.RequestFocus;
            _movementDrag.Finished += parentWindow.ValidateBounds;
            _rectResizer.Finished += parentWindow.ValidateBounds;
        }

        private RectangleF CanvasRectangle => _parentWindow.CanvasRectangle;
        public Depth Depth => _parentWindow.StartingDepth;
        public StaticImageAsset? Icon => _parentWindow.Icon;

        public RectangleF TitleBarRectangle
        {
            get
            {
                var canvasRect = CanvasRectangle;
                return new RectangleF(new Vector2(canvasRect.X, canvasRect.Y - _titleBarThickness),
                    new Vector2(canvasRect.Width, _titleBarThickness));
            }
        }

        public TitleBar.Layout TitleLayout => _titleBar.GetLayoutStruct();

        public RectangleF WholeWindowRectangle
        {
            get => RectangleF.Union(TitleBarRectangle, CanvasRectangle);
            set
            {
                _parentWindow.Position = value.Location;
                var newSize = (value.Size - TitleBarRectangle.Size.JustY()).ToPoint();
                _parentWindow._widget.Size = newSize;
                Resized?.Invoke();
            }
        }

        public string Title => _parentWindow.Title;

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            if (_sizeSettings is ResizableSizeSettings resizableWindowSizeSettings)
            {
                HandleResizing(input, hitTestStack);
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

            _headerClickable.Poll(input.Mouse, _headerHovered);

            if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
            {
                _movementDrag.End();
            }

            _titleBar.UpdateInput(input, hitTestStack);
        }

        public event Action? Resized;

        private void HandleResizing(ConsumableInput input, HitTestStack hitTestStack)
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
                    Resized?.Invoke();
                }
            }
        }

        public void Draw(Painter painter, IGuiTheme theme, bool isInFocus)
        {
            theme.DrawWindowChrome(painter, this, isInFocus);

            if (_pendingResizeRect.HasValue)
            {
                painter.DrawLineRectangle(_pendingResizeRect.Value.WithHeight(TitleBarRectangle.Height),
                    new LineDrawSettings {Depth = Depth.Front + 100, Color = Color.Gray, Thickness = 2});
                painter.DrawLineRectangle(_pendingResizeRect.Value,
                    new LineDrawSettings {Depth = Depth.Front + 100, Color = Color.Gray, Thickness = 2});
            }
        }
    }
}
