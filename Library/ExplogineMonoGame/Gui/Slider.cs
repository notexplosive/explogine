using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Gui;

public class Slider : IGuiWidget
{
    private readonly Drag<RectangleF> _thumbDrag;
    private readonly int _totalNotches;

    public Slider(RectangleF entireRectangle, int totalNotches, Depth depth, Wrapped<int> state)
    {
        Depth = depth;
        State = state;
        EntireRectangle = entireRectangle;
        BodyRectangle = entireRectangle.Inflated(new Vector2(-5).JustAxis(Axis.X.Opposite()));
        _totalNotches = totalNotches;
        _thumbDrag = new Drag<RectangleF>();
    }

    public bool BodyHovered { get; private set; }
    public bool ThumbHovered { get; private set; }
    public bool ThumbEngaged => ThumbHovered || _thumbDrag.IsDragging;
    public bool IsDragging => _thumbDrag.IsDragging;
    public RectangleF BodyRectangle { get; }
    public RectangleF EntireRectangle { get; }

    public RectangleF RunwayRectangle
    {
        get
        {
            var alongSize = BodyRectangle.Size.GetAxis(Axis.X) - ThumbRectangle.Size.GetAxis(Axis.X);
            var perpSize = BodyRectangle.Size.GetAxis(Axis.X.Opposite());
            var startPosition = BodyRectangle.TopLeft + ThumbRectangle.Size.JustAxis(Axis.X);
            var size = Vector2Extensions.FromAxisFirst(Axis.X, alongSize, perpSize);
            return new RectangleF(startPosition, size);
        }
    }

    public RectangleF ThumbRectangle =>
        new(
            EntireRectangle.TopLeft
            + new Vector2(ThumbTravelDistance * State.Value / _totalNotches).JustAxis(Axis.X),
            ThumbSize);

    public Vector2 ThumbSize =>
        Vector2Extensions.FromAxisFirst(Axis.X, 32, EntireRectangle.Size.GetAxis(Axis.X.Opposite()));
    public Wrapped<int> State { get; }
    public Depth Depth { get; }

    public float ThumbTravelDistance =>
        BodyRectangle.Size.GetAxis(Axis.X) - ThumbSize.GetAxis(Axis.X);

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(BodyRectangle, Depth, UnsetBodyHovered, SetBodyHovered);
        hitTestStack.AddZone(ThumbRectangle, Depth - 1, UnsetThumbHovered, SetThumbHovered);

        var position = input.Mouse.Position(hitTestStack.WorldMatrix);

        if (ThumbHovered)
        {
            Client.Window.SetCursor(MouseCursor.Hand);
        }

        if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
        {
            if (BodyHovered)
            {
                State.Value = GetNotchValueAt(position);
            }

            if (ThumbHovered || BodyHovered)
            {
                _thumbDrag.Start(ThumbRectangle);
            }
        }

        if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
        {
            _thumbDrag.End();
        }

        _thumbDrag.AddDelta(input.Mouse.Delta(hitTestStack.WorldMatrix));

        if (_thumbDrag.IsDragging)
        {
            State.Value = GetNotchValueAt(_thumbDrag.StartingValue.Center + _thumbDrag.TotalDelta);
        }
    }

    private int GetNotchValueAt(Vector2 position)
    {
        var relativePosition = position - BodyRectangle.TopLeft;

        // subtract half the thumbs size so we're centered
        relativePosition -= new Vector2(ThumbSize.GetAxis(Axis.X) / 2f).JustAxis(Axis.X);
        var totalSize = ThumbTravelDistance;
        var percent = relativePosition.GetAxis(Axis.X) / totalSize;
        var result = MathF.Round(percent * _totalNotches, MidpointRounding.ToEven);

        return Math.Clamp((int) result, 0, _totalNotches);
    }

    private void SetThumbHovered()
    {
        ThumbHovered = true;
    }

    private void UnsetThumbHovered()
    {
        ThumbHovered = false;
    }

    private void UnsetBodyHovered()
    {
        BodyHovered = false;
    }

    private void SetBodyHovered()
    {
        BodyHovered = true;
    }
}
