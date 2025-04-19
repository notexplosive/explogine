using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Gui;

/// <summary>
///     Despite it having UpdateInput(), Scrollbar is NOT an IGuiWidget because it isn't owned by the Gui.
///     It is drawn with a GuiTheme though so it belongs in this namespace.
/// </summary>
public class Scrollbar
{
    private readonly Drag<float> _thumbDrag = new();

    public Scrollbar(ScrollableArea scrollableScrollableArea, Orientation orientation, Depth depth, float thickness)
    {
        ScrollableArea = scrollableScrollableArea;
        Depth = depth;
        AlongAxis = Axis.FromOrientation(orientation);
        Thickness = thickness;
    }

    public float Thickness { get; }
    public bool BodyHovered { get; private set; }
    public bool ThumbHovered { get; private set; }
    public Axis AlongAxis { get; }
    public ScrollableArea ScrollableArea { get; }
    public Depth Depth { get; }

    public RectangleF BodyRectangle
    {
        get
        {
            var area = ScrollableArea;
            var thicknessAsSize = new Vector2(Thickness);
            var bodyRectPosition = (area.CanvasSize.ToVector2() - thicknessAsSize).JustAxis(AlongAxis.Opposite());

            var bodyRectSize =
                Vector2Extensions.FromAxisFirst(AlongAxis,
                    area.CanvasSize.ToVector2().GetAxis(AlongAxis),
                    thicknessAsSize.GetAxis(AlongAxis.Opposite()))
                - thicknessAsSize.JustAxis(AlongAxis); // subtract thickness for the clearance at the end
            return new RectangleF(bodyRectPosition, bodyRectSize);
        }
    }

    public float CurrentPercent
    {
        get => CurrentUnits / MaxUnits;
        set => CurrentUnits = MaxUnits * value;
    }

    private float CurrentUnits
    {
        get => ScrollableArea.ViewBounds.Location.GetAxis(AlongAxis);
        set => ScrollableArea.SetPosition(ScrollableArea.ViewBounds.Location.JustAxis(AlongAxis.Opposite()) +
                                          new Vector2(value).JustAxis(AlongAxis));
    }

    private float MaxUnits => ScrollableArea.InnerWorldBoundaries.BottomRight.GetAxis(AlongAxis);

    public float ThumbScalar =>
        ScrollableArea.ViewBounds.Size.GetAxis(AlongAxis) / ScrollableArea.InnerWorldBoundaries.Size.GetAxis(AlongAxis);

    public RectangleF ThumbRectangle
    {
        get
        {
            var bodyRect = BodyRectangle;
            return new RectangleF(bodyRect.TopLeft + bodyRect.Size.JustAxis(AlongAxis) * CurrentPercent,
                new Vector2(bodyRect.Width, bodyRect.Height).StraightMultiply(
                    Vector2Extensions.FromAxisFirst(AlongAxis, ThumbScalar, 1f)));
        }
    }

    public float ThumbPercent => ThumbRectangle.Size.GetAxis(AlongAxis) / BodyRectangle.Size.GetAxis(AlongAxis);

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var mousePosition = input.Mouse.Position(hitTestStack.WorldMatrix);
        hitTestStack.AddZone(BodyRectangle, Depth, UnsetBodyHovered, SetBodyHovered);
        hitTestStack.AddZone(ThumbRectangle, Depth - 1, UnsetThumbHovered, SetThumbHovered);

        var mousePercent = mousePosition.GetAxis(AlongAxis) / ScrollableArea.CanvasSize.GetAxis(AlongAxis);

        if (ThumbHovered)
        {
            Client.Cursor.Set(MouseCursor.Hand);
        }

        if (input.Mouse.GetButton(MouseButton.Left).WasPressed)
        {
            if (BodyHovered)
            {
                CurrentPercent = mousePercent - ThumbPercent / 2;
            }

            if (ThumbHovered || BodyHovered)
            {
                _thumbDrag.Start(CurrentPercent);
            }
        }

        if (input.Mouse.GetButton(MouseButton.Left).WasReleased)
        {
            _thumbDrag.End();
        }

        _thumbDrag.AddDelta(input.Mouse.Delta(hitTestStack.WorldMatrix) / ScrollableArea.CanvasSize.GetAxis(AlongAxis));

        if (_thumbDrag.IsDragging)
        {
            CurrentPercent = _thumbDrag.StartingValue + _thumbDrag.TotalDelta.GetAxis(AlongAxis);
        }
    }

    private void UnsetThumbHovered()
    {
        ThumbHovered = false;
    }

    private void UnsetBodyHovered()
    {
        BodyHovered = false;
    }

    private void SetThumbHovered()
    {
        ThumbHovered = true;
    }

    private void SetBodyHovered()
    {
        BodyHovered = true;
    }
}
