using System;
using System.Diagnostics.Contracts;
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
    public Vector2 TopRight => new(Location.X + Size.Y, Location.Y);
    public float Area => Width * Height;

    public bool IsEmpty()
    {
        return Size.Length() <= 0;
    }

    public Rectangle ToRectangle()
    {
        return new Rectangle(Location.ToPoint(), Size.ToPoint());
    }

    public bool Contains(Point point)
    {
        return Contains(point.ToVector2());
    }

    public bool Contains(Rectangle containedRect)
    {
        return Contains(containedRect.ToRectangleF());
    }

    public bool Contains(RectangleF containedRect)
    {
        return Contains(containedRect.Location) &&
               Contains(containedRect.Location + containedRect.Size - new Vector2(1));
    }

    public bool Contains(Vector2 vector)
    {
        var normalizedPoint = vector - Location;
        return normalizedPoint.X < Size.X && normalizedPoint.Y < Size.Y && normalizedPoint.X >= 0 &&
               normalizedPoint.Y >= 0;
    }

    public bool Contains(int x, int y)
    {
        return Contains(new Point(x, y));
    }

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

        var location = Vector2.Min(a, b);
        var bottomRight = Vector2.Max(a, b);

        var size = bottomRight - location;

        if (size.X == 0 || size.Y == 0)
        {
            return RectangleF.Empty;
        }

        return new RectangleF(location, size);
    }

    public static RectangleF Union(RectangleF a, RectangleF b)
    {
        var location = Vector2.Min(a.Location, b.Location);
        var bottomRight = Vector2.Max(a.BottomRight, b.BottomRight);
        var size = bottomRight - location;
        return new RectangleF(location, size);
    }

    public bool Intersects(RectangleF other)
    {
        var otherTopLeft = other.Location;
        var otherBottomRight = other.BottomRight;
        var otherTopRight = other.Location + new Vector2(Width, 0);
        var otherBottomLeft = other.Location + new Vector2(0, Height);

        var topLeft = Location;
        var bottomRight = BottomRight;
        var topRight = Location + new Vector2(Width, 0);
        var bottomLeft = Location + new Vector2(0, Height);

        return other.Contains(topLeft) || other.Contains(bottomRight) || other.Contains(topRight) ||
               other.Contains(bottomLeft) || Contains(otherTopLeft) || Contains(otherBottomRight) ||
               Contains(otherTopRight) || Contains(otherBottomLeft);
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

    public RectangleF Inflated(float horizontalAmount, float verticalAmount)
    {
        var copy = this;
        copy.Inflate(horizontalAmount, verticalAmount);
        return copy;
    }

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

    public RectangleF GetEdgeRect(RectEdge edge, float thickness)
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

    public RectangleF InflatedMaintainAspectRatio(float longSideAmount)
    {
        var horizontalAmount = longSideAmount;
        var verticalAmount = longSideAmount;

        if (Width > Height)
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
    public Matrix CanvasToScreen(Point outputDimensions, float angle)
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
    public Matrix ScreenToCanvas(Point outputDimensions, float angle)
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
}
