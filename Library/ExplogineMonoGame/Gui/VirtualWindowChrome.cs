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
        private readonly int _titleBarThickness;
        private readonly IWindowSizeSettings _windowSizeSettings;
        private RectangleF? _pendingResizeRect;

        public Chrome(VirtualWindow parentWindow, int titleBarThickness, Point minimumWidgetSize, IWindowSizeSettings windowSizeSettings)
        {
            _parentWindow = parentWindow;
            _titleBarThickness = titleBarThickness;
            _windowSizeSettings = windowSizeSettings;
            _minimumSize = minimumWidgetSize + new Point(0, titleBarThickness);
            _rectResizer = new RectResizer();
            _movementDrag = new Drag<Vector2>();
            _headerClickable.ClickInitiated += parentWindow.RequestFocus;
            _rectResizer.Initiated += parentWindow.RequestFocus;
            _movementDrag.Finished += parentWindow.ValidateBounds;
            _rectResizer.Finished += parentWindow.ValidateBounds;
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

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            if (_windowSizeSettings is ResizableWindowSizeSettings resizableWindowSizeSettings)
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
        }

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
