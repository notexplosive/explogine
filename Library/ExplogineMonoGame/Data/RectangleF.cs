using System;
using System.Diagnostics.Contracts;
using ExTween;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public struct RectangleF : IEquatable<RectangleF>
{
    public RectangleF(Rectangle rectangle)
    {
        Location = rectangle.Location.ToVector2();
        Size = rectangle.Size.ToVector2();
    }

    public RectangleF(Vector2 location, Vector2 size)
    {
        Location = location;
        Size = size;
    }

    public RectangleF(float x, float y, float width, float height) : this(new Vector2(x, y), new Vector2(width, height))
    {
    }

    public static implicit operator RectangleF(Rectangle rect)
    {
        return rect.ToRectangleF();
    }

    public override string ToString()
    {
        return $"({Location.X}, {Location.Y}) ({Size.X}, {Size.Y})";
    }

    public bool Equals(RectangleF other)
    {
        return Location.Equals(other.Location) && Size.Equals(other.Size);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Rectangle rectangle)
        {
            return Equals(rectangle.ToRectangleF());
        }

        return obj is RectangleF other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Location, Size);
    }

    public static bool operator ==(RectangleF left, RectangleF right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RectangleF left, RectangleF right)
    {
        return !left.Equals(right);
    }

    public Vector2 Location { get; set; }
    public Vector2 Size { get; set; }
    public float Width => Size.X;
    public float Height => Size.Y;
    public Vector2 Center => Location + Size / 2f;
    public float Left => Location.X;
    public float Right => Location.X + Size.X;
    public float Top => Location.Y;
    public float Bottom => Location.Y + Size.Y;
    public float X => Location.X;
    public float Y => Location.Y;
    public static RectangleF Empty => new(Vector2.Zero, Vector2.Zero);
    public Vector2 TopLeft => Location;
    public Vector2 BottomRight => Location + Size;
    public Vector2 BottomLeft => new(Location.X, Location.Y + Size.Y);
    public Vector2 TopRight => new(Location.X + Size.X, Location.Y);
    public float Area => Width * Height;

    public bool IsEmpty()
    {
        return Size.Length() <= 0;
    }

    [Pure]
    public Rectangle ToRectangle()
    {
        return new Rectangle(Location.ToPoint(), Size.ToPoint());
    }

    [Pure]
    public bool Contains(Point point)
    {
        return Contains(point.ToVector2());
    }

    [Pure]
    public bool Contains(Rectangle containedRect)
    {
        return Contains(containedRect.ToRectangleF());
    }

    [Pure]
    public bool Contains(RectangleF containedRect)
    {
        return Contains(containedRect.Location) &&
               Contains(containedRect.Location + containedRect.Size - new Vector2(1));
    }

    [Pure]
    public bool Contains(Vector2 vector)
    {
        var normalizedPoint = vector - Location;
        return normalizedPoint.X < Size.X && normalizedPoint.Y < Size.Y && normalizedPoint.X >= 0 &&
               normalizedPoint.Y >= 0;
    }

    [Pure]
    public bool Contains(int x, int y)
    {
        return Contains(new Point(x, y));
    }

    [Pure]
    public bool Contains(float x, float y)
    {
        return Contains(new Vector2(x, y));
    }

    public void Deconstruct(out float x, out float y, out float width, out float height)
    {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    public void Inflate(float horizontalAmount, float verticalAmount)
    {
        Location -= new Vector2(horizontalAmount, verticalAmount);
        Size += new Vector2(horizontalAmount * 2, verticalAmount * 2);
    }

    [Pure]
    public static RectangleF Intersect(RectangleF rectA, RectangleF rectB)
    {
        if (!rectA.Intersects(rectB))
        {
            return RectangleF.Empty;
        }

        var aX = MathF.Max(rectA.Location.X, rectB.Location.X);
        var aY = MathF.Max(rectA.Location.Y, rectB.Location.Y);

        var bX = MathF.Min(rectA.BottomRight.X, rectB.BottomRight.X);
        var bY = MathF.Min(rectA.BottomRight.Y, rectB.BottomRight.Y);

        var a = new Vector2(aX, aY);
        var b = new Vector2(bX, bY);

        var location = a;
        var bottomRight = b;

        var size = bottomRight - location;

        if (size.X <= 0 || size.Y <= 0)
        {
            return RectangleF.Empty;
        }

        return new RectangleF(location, size);
    }

    [Pure]
    public static RectangleF Union(RectangleF a, RectangleF b)
    {
        var location = Vector2.Min(a.Location, b.Location);
        var bottomRight = Vector2.Max(a.BottomRight, b.BottomRight);
        var size = bottomRight - location;
        return new RectangleF(location, size);
    }

    [Pure]
    public bool Intersects(RectangleF other)
    {
        return other.Contains(TopLeft) || other.Contains(BottomRight) || other.Contains(TopRight) ||
               other.Contains(BottomLeft) || Contains(other.TopLeft) || Contains(other.BottomRight) ||
               Contains(other.TopRight) || Contains(other.BottomLeft);
    }

    [Pure]
    public RectangleF Moved(Vector2 offsetAmount)
    {
        var rectangle = this;
        rectangle.Offset(offsetAmount);
        return rectangle;
    }

    public void Offset(Point point)
    {
        Offset(point.ToVector2());
    }

    public void Offset(Vector2 vector)
    {
        Location += vector;
    }

    public void Offset(int x, int y)
    {
        Offset(new Vector2(x, y));
    }

    public void Offset(float x, float y)
    {
        Offset(new Vector2(x, y));
    }

    [Pure]
    public RectangleF Inflated(Vector2 amount)
    {
        return Inflated(amount.X, amount.Y);
    }

    [Pure]
    public RectangleF Inflated(float horizontalAmount, float verticalAmount)
    {
        var copy = this;
        copy.Inflate(horizontalAmount, verticalAmount);
        return copy;
    }

    [Pure]
    public RectangleF ResizedOnEdge(RectEdge edge, Vector2 delta)
    {
        var x = X;
        var y = Y;
        var width = Width;
        var height = Height;

        void ResizeOnCardinalEdge(RectEdge localEdge)
        {
            switch (localEdge)
            {
                case RectEdge.Bottom:
                    height += delta.Y;
                    break;
                case RectEdge.Right:
                    width += delta.X;
                    break;
                case RectEdge.Left:
                    width -= delta.X;
                    x += delta.X;
                    break;
                case RectEdge.Top:
                    height -= delta.Y;
                    y += delta.Y;
                    break;
            }
        }

        switch (edge)
        {
            case RectEdge.Bottom:
            case RectEdge.Right:
            case RectEdge.Left:
            case RectEdge.Top:
                ResizeOnCardinalEdge(edge);
                break;
            case RectEdge.BottomLeft:
                ResizeOnCardinalEdge(RectEdge.Bottom);
                ResizeOnCardinalEdge(RectEdge.Left);
                break;
            case RectEdge.BottomRight:
                ResizeOnCardinalEdge(RectEdge.Bottom);
                ResizeOnCardinalEdge(RectEdge.Right);
                break;
            case RectEdge.TopLeft:
                ResizeOnCardinalEdge(RectEdge.Top);
                ResizeOnCardinalEdge(RectEdge.Left);
                break;
            case RectEdge.TopRight:
                ResizeOnCardinalEdge(RectEdge.Right);
                ResizeOnCardinalEdge(RectEdge.Top);
                break;
        }

        return new RectangleF(x, y, width, height);
    }

    [Pure]
    public float GetEdge(RectEdge edge)
    {
        return edge switch
        {
            RectEdge.Top => Top,
            RectEdge.Left => Left,
            RectEdge.Right => Right,
            RectEdge.Bottom => Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, $"Unable to obtain {edge} as a float")
        };
    }

    /// <summary>
    ///     Extrudes a rectangle from the edge of an existing rectangle
    /// </summary>
    [Pure]
    public RectangleF GetRectangleFromEdge(RectEdge edge, float thickness)
    {
        return edge switch
        {
            RectEdge.Right => new RectangleF(Right, Top, thickness, Height),
            RectEdge.Top => new RectangleF(Left, Top - thickness, Width, thickness),
            RectEdge.Left => new RectangleF(Left - thickness, Top, thickness, Height),
            RectEdge.Bottom => new RectangleF(Left, Bottom, Width, thickness),
            RectEdge.TopLeft => new RectangleF(Left - thickness, Top - thickness, thickness, thickness),
            RectEdge.TopRight => new RectangleF(Right, Top - thickness, thickness, thickness),
            RectEdge.BottomLeft => new RectangleF(Left - thickness, Bottom, thickness, thickness),
            RectEdge.BottomRight => new RectangleF(Right, Bottom, thickness, thickness),
            _ => throw new Exception($"Unrecognized edge {edge}")
        };
    }

    [Pure]
    public RectangleF ConstrainedTo(RectangleF outer)
    {
        if (outer.Contains(TopLeft) && outer.Contains(TopRight) && outer.Contains(BottomRight) &&
            outer.Contains(BottomLeft))
        {
            return this;
        }

        var result = this;

        if (result.Right > outer.Right)
        {
            result.Location = new Vector2(outer.Right - result.Size.X, result.Y);
        }

        if (result.Bottom > outer.Bottom)
        {
            result.Location = new Vector2(result.X, outer.Bottom - result.Size.Y);
        }

        if (result.Left < outer.Left)
        {
            result.Location = new Vector2(outer.X, result.Y);
        }

        if (result.Top < outer.Top)
        {
            result.Location = new Vector2(result.X, outer.Y);
        }

        return result;
    }

    [Pure]
    public float EdgeDisplacement(RectEdge edge, RectangleF outer)
    {
        var innerEdge = GetEdge(edge);
        var outerEdge = outer.GetEdge(edge);

        switch (edge)
        {
            case RectEdge.Top:
            case RectEdge.Left:
                return outerEdge - innerEdge;
            case RectEdge.Right:
            case RectEdge.Bottom:
                return innerEdge - outerEdge;
            default:
                throw new Exception($"Invalid Edge: {edge}");
        }
    }

    [Pure]
    public RectangleF ConstrainSizeTo(RectangleF outerRect)
    {
        var result = this;
        var edges = new[] {RectEdge.Top, RectEdge.Left, RectEdge.Right, RectEdge.Bottom};
        foreach (var edge in edges)
        {
            var displacement = EdgeDisplacement(edge, outerRect);
            if (displacement > 0)
            {
                switch (edge)
                {
                    case RectEdge.Top:
                        result.Location += new Vector2(0, displacement);
                        result.Size -= new Vector2(0, displacement);
                        break;
                    case RectEdge.Left:
                        result.Location += new Vector2(displacement, 0);
                        result.Size -= new Vector2(displacement, 0);
                        break;
                    case RectEdge.Right:
                        result.Size -= new Vector2(displacement, 0);
                        break;
                    case RectEdge.Bottom:
                        result.Size -= new Vector2(0, displacement);
                        break;
                }
            }
        }

        return result;
    }

    [Pure]
    public RectangleF InflatedMaintainAspectRatio(float longSideAmount)
    {
        var horizontalAmount = longSideAmount;
        var verticalAmount = longSideAmount;

        if (Width < Height)
        {
            var aspectRatio = Height / Width;
            verticalAmount = longSideAmount * aspectRatio;
        }
        else
        {
            var aspectRatio = Width / Height;
            horizontalAmount = longSideAmount * aspectRatio;
        }

        return Inflated(horizontalAmount, verticalAmount);
    }

    [Pure]
    public Polygon ToPolygon()
    {
        return new Polygon(
            Center,
            new[]
            {
                new Vector2(Left, Top) - Center,
                new Vector2(Right, Top) - Center,
                new Vector2(Right, Bottom) - Center,
                new Vector2(Left, Bottom) - Center
            });
    }

    [Pure]
    public Matrix CanvasToScreen(Point outputDimensions, float angle = 0)
    {
        var halfSize = Size / 2;
        var rotation =
            Matrix.CreateTranslation(new Vector3(-halfSize, 0))
            * Matrix.CreateRotationZ(-angle)
            * Matrix.CreateTranslation(new Vector3(halfSize, 0));
        var translation =
            Matrix.CreateTranslation(new Vector3(-Location, 0));
        return translation * rotation * Matrix.CreateScale(new Vector3(outputDimensions.X / Width,
            outputDimensions.Y / Height, 1));
    }

    [Pure]
    public Matrix ScreenToCanvas(Point outputDimensions, float angle = 0)
    {
        return Matrix.Invert(CanvasToScreen(outputDimensions, angle));
    }

    /// <summary>
    ///     Deflates the ViewRect centered on a focus point such that the focus point is at the same relative position before
    ///     and after the deflation.
    /// </summary>
    /// <param name="zoomAmount">
    ///     Amount to deflate the long side of the viewBounds by (short side will deflate by the correct
    ///     amount relative to aspect ratio)
    /// </param>
    /// <param name="focusPosition">Position to zoom towards in WorldSpace (aka: the same space as the ViewBounds rect)</param>
    /// <returns></returns>
    [Pure]
    public RectangleF GetZoomedInBounds(float zoomAmount, Vector2 focusPosition)
    {
        var focusRelativeToViewBounds = focusPosition - Location;
        var relativeScalar = focusRelativeToViewBounds.StraightDivide(Width, Height);
        var zoomedInBounds = InflatedMaintainAspectRatio(-zoomAmount);

        // center zoomed in bounds on focus
        zoomedInBounds.Location = focusPosition - zoomedInBounds.Size / 2f;

        // offset zoomed in bounds so focus is in the same relative spot
        var focusRelativeToZoomedInBounds =
            relativeScalar.StraightMultiply(zoomedInBounds.Width, zoomedInBounds.Height);
        var newFocusPosition = focusRelativeToZoomedInBounds + zoomedInBounds.Location;
        var oldFocusPosition = focusRelativeToViewBounds + Location;
        zoomedInBounds.Offset(oldFocusPosition - newFocusPosition);

        return zoomedInBounds;
    }

    /// <summary>
    ///     Inflates the ViewRect centered on a focus point such that the focus point is at the same relative position before
    ///     and after the deflation.
    /// </summary>
    /// <param name="zoomAmount">Amount to deflate the viewBounds by</param>
    /// <param name="focusPosition">Position to zoom towards in WorldSpace (aka: the same space as the ViewBounds rect)</param>
    /// <returns></returns>
    [Pure]
    public RectangleF GetZoomedOutBounds(float zoomAmount, Vector2 focusPosition)
    {
        var zoomedInBounds = GetZoomedInBounds(zoomAmount, focusPosition);
        var zoomedOutOffset = Center - zoomedInBounds.Center;
        var zoomedOutBounds = zoomedInBounds.InflatedMaintainAspectRatio(zoomAmount * 2);
        zoomedOutBounds.Offset(zoomedOutOffset * 2);
        return zoomedOutBounds;
    }

    [Pure]
    public static RectangleF FromCorners(Vector2 cornerA, Vector2 cornerB)
    {
        var x = MathF.Min(cornerA.X, cornerB.X);
        var y = MathF.Min(cornerA.Y, cornerB.Y);
        var width = MathF.Abs(cornerA.X - cornerB.X);
        var height = MathF.Abs(cornerA.Y - cornerB.Y);
        return new RectangleF(x, y, width, height);
    }

    [Pure]
    public RectangleF ConstrictedToAspectRatio(float aspectRatio)
    {
        var aspect = Vector2Extensions.FromAspectRatio(aspectRatio);
        var result = new RectangleF(Location, aspect * Vector2Extensions.CalculateScalarDifference(Size, aspect));
        return result;
    }

    [Pure]
    public static RectangleF FromSizeAlignedWithin(RectangleF outer, Vector2 innerSize, Alignment alignment)
    {
        return new RectangleF(Vector2.Zero, innerSize).AlignedWithin(outer, alignment);
    }

    [Pure]
    public RectangleF AlignedWithin(RectangleF outer, Alignment alignment)
    {
        var resultPosition = Location;
        resultPosition.X = alignment.Horizontal switch
        {
            HorizontalAlignment.Left => outer.X,
            HorizontalAlignment.Center => outer.Right - outer.Width / 2 - Width / 2,
            HorizontalAlignment.Right => outer.Right - Size.X,
            _ => resultPosition.X
        };

        resultPosition.Y = alignment.Vertical switch
        {
            VerticalAlignment.Top => outer.Y,
            VerticalAlignment.Center => outer.Bottom - outer.Height / 2 - Height / 2,
            VerticalAlignment.Bottom => outer.Bottom - Size.Y,
            _ => resultPosition.Y
        };

        return new RectangleF(resultPosition, Size);
    }

    [Pure]
    public RectangleF WithPosition(Vector2 position)
    {
        return new RectangleF(position, Size);
    }

    [Pure]
    public RectangleF WithHeight(float height)
    {
        return new RectangleF(Location, new Vector2(Size.X, height));
    }

    [Pure]
    public RectangleF WithWidth(float width)
    {
        return new RectangleF(Location, new Vector2(width, Size.Y));
    }

    [Pure]
    public RectangleF MovedByOrigin(DrawOrigin origin)
    {
        return Moved(origin.Calculate(Size));
    }

    [Pure]
    public RectangleF MovedToZero()
    {
        return new RectangleF(Vector2.Zero, Size);
    }

    [Pure]
    public bool Envelopes(RectangleF smallerRect)
    {
        var overlap = RectangleF.Intersect(this, smallerRect);
        return overlap.Area >= smallerRect.Area;
    }

    [Pure]
    public static RectangleF InflateFrom(Vector2 headPosition, float width, float height)
    {
        return new RectangleF(headPosition, Vector2.Zero).Inflated(width, height);
    }
    
    [Pure]
    public static RectangleF Lerp(RectangleF startingValue, RectangleF targetValue, float percent)
    {
        var x = FloatExtensions.Lerp(startingValue.Location.X, targetValue.Location.X, percent);
        var y = FloatExtensions.Lerp(startingValue.Location.Y, targetValue.Location.Y, percent);
        var width = FloatExtensions.Lerp(startingValue.Size.X, targetValue.Size.X, percent);
        var height = FloatExtensions.Lerp(startingValue.Size.Y, targetValue.Size.Y, percent);

        return new RectangleF(x, y, width, height);
    }
}
