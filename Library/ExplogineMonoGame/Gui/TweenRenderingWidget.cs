using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExTween;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class TweenRenderingWidget : Widget, IUpdateInput
{
    private readonly ITween _rootTween;
    private float _pixelsPerSecond;

    public TweenRenderingWidget(ITween rootTween, Vector2 position, Point size, Depth depth) : base(position, size,
        depth)
    {
        _rootTween = rootTween;
        _pixelsPerSecond = 100f;
        ViewBoundsLeft = 0f;
    }

    public float PlayHeadX => _rootTween.TotalDuration.GetCurrentTime() * _pixelsPerSecond;
    public float ViewBoundsWidth => Size.X;

    public float ViewBoundsRight => ViewBoundsLeft + ViewBoundsWidth;
    public float ViewBoundsLeft { get; private set; }
    public float RootDuration => _rootTween.TotalDuration.GetDuration();
    public float ViewBoundsCenter => ViewBoundsLeft + ViewBoundsWidth / 2;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (IsHovered)
        {
            var scrollDelta = input.Mouse.ScrollDelta(true);
            if (scrollDelta != 0)
            {
                if (scrollDelta > 0)
                {
                    ZoomIn(input, hitTestStack);
                }

                if (scrollDelta < 0)
                {
                    ZoomOut(input, hitTestStack);
                }
            }

            if (input.Mouse.GetButton(MouseButton.Middle, true).IsDown)
            {
                var delta = input.Mouse.Delta(hitTestStack.WorldMatrix);
                ViewBoundsLeft -= delta.X;
            }
        }

        ViewBoundsLeft = Math.Clamp(ViewBoundsLeft, 0, RootDuration * _pixelsPerSecond);
    }

    public void DrawContent(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);
        painter.BeginSpriteBatch(Matrix.CreateTranslation(new Vector3(-ViewBoundsLeft, 0, 0)));
        painter.Clear(Color.White.WithMultipliedOpacity(0.5f));

        DrawTween(painter, _rootTween, Vector2.Zero, Size.Y);
        DrawPlayHead(painter);

        painter.EndSpriteBatch();
        Client.Graphics.PopCanvas();
    }

    private void DrawPlayHead(Painter painter)
    {
        var playHeadRect = new RectangleF(PlayHeadX, 0, 0, Size.Y).Inflated(2, 0);
        painter.DrawRectangle(playHeadRect, new DrawSettings {Depth = Depth.Front, Color = Color.Black});
    }

    private RectangleF DrawTween(Painter painter, ITween tween, Vector2 startingPosition, float height)
    {
        if (tween is DynamicTween dynamicTween)
        {
            return DrawTween(painter, dynamicTween.GenerateIfNotAlready(), startingPosition, height);
        }

        var duration = tween.TotalDuration.GetDuration();
        var originalRectangle =
            new RectangleF(startingPosition, new Vector2(duration * _pixelsPerSecond, height));
        var fullRectangle = originalRectangle;
        var fillColor = Color.Red;
        var unfilledColor = Color.OrangeRed;
        var currentTime = tween.TotalDuration.GetCurrentTime();

        if (tween is TweenCollection collection)
        {
            if (collection is MultiplexTween)
            {
                var childHeight = fullRectangle.Height / collection.ChildCount;
                var position = fullRectangle.TopLeft;
                foreach (var child in collection.Children())
                {
                    DrawTween(painter, child, position, childHeight);
                    position.Y += childHeight;
                }
            }

            if (collection is SequenceTween)
            {
                var position = fullRectangle.TopLeft;
                foreach (var child in collection.Children())
                {
                    var result = DrawTween(painter, child, position, fullRectangle.Height);
                    position.X += result.Width;
                }
            }
        }
        else
        {
            if (duration > 0)
            {
                var percent = currentTime / duration;
                var progressRectangle = new RectangleF(fullRectangle.Location,
                    new Vector2(fullRectangle.Width * percent, fullRectangle.Height));

                if (tween is IValueTween valueTween)
                {
                    fillColor = new NoiseBasedRng(valueTween.TweenableHashCode()).NextColor();
                    unfilledColor = new Color(fillColor.B - 10, fillColor.G - 10, fillColor.B - 10);
                }

                painter.DrawLineRectangle(fullRectangle,
                    new LineDrawSettings {Color = Color.Black, Depth = Depth.Middle - 5});
                painter.DrawRectangle(fullRectangle,
                    new DrawSettings {Color = unfilledColor, Depth = Depth.Middle});
                painter.DrawRectangle(progressRectangle,
                    new DrawSettings {Color = fillColor, Depth = Depth.Middle - 1});
            }
        }

        return originalRectangle;
    }

    public void ZoomIn(ConsumableInput input, HitTestStack hitTestStack)
    {
        _pixelsPerSecond += 120;
    }

    public void ZoomOut(ConsumableInput input, HitTestStack hitTestStack)
    {
        _pixelsPerSecond -= 120;
    }
}
