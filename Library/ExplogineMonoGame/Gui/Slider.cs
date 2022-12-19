using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Gui;

public class Slider : IGuiWidget
{
    private readonly Drag<RectangleF> _thumbDrag = new();
    private readonly int _totalNotches;

    public Slider(RectangleF entireRectangle, Orientation orientation, int totalNotches, Depth depth,
        Wrapped<int> state)
    {
        AlongAxis = Axis.FromOrientation(orientation);
        Depth = depth;
        State = state;
        EntireRectangle = entireRectangle;
        BodyRectangle = entireRectangle.Inflated(new Vector2(-5).JustAxis(AlongAxis.Opposite()));
        _totalNotches = totalNotches;
    }

    public Axis AlongAxis { get; }
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
            var alongSize = BodyRectangle.Size.GetAxis(AlongAxis) - ThumbRectangle.Size.GetAxis(AlongAxis);
            var perpSize = BodyRectangle.Size.GetAxis(AlongAxis.Opposite());
            var startPosition = BodyRectangle.TopLeft + ThumbRectangle.Size.JustAxis(AlongAxis);
            var size = Vector2Extensions.FromAxisFirst(AlongAxis, alongSize, perpSize);
            return new RectangleF(startPosition, size);
        }
    }

    public RectangleF ThumbRectangle =>
        new(
            EntireRectangle.TopLeft
            + new Vector2(ThumbTravelDistance * State.Value / _totalNotches).JustAxis(AlongAxis),
            ThumbSize);

    public Vector2 ThumbSize =>
        Vector2Extensions.FromAxisFirst(AlongAxis, 32, EntireRectangle.Size.GetAxis(AlongAxis.Opposite()));

    public Wrapped<int> State { get; }
    public Depth Depth { get; }

    public float ThumbTravelDistance =>
        BodyRectangle.Size.GetAxis(AlongAxis) - ThumbSize.GetAxis(AlongAxis);

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
        relativePosition -= new Vector2(ThumbSize.GetAxis(AlongAxis) / 2f).JustAxis(AlongAxis);
        var totalSize = ThumbTravelDistance;
        var percent = relativePosition.GetAxis(AlongAxis) / totalSize;
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
