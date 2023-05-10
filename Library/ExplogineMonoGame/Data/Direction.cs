using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public class Direction
{
    private readonly Point _internalPoint;
    private readonly string _name;

    private Direction(string name, Point givenPoint)
    {
        _name = name;
        _internalPoint = givenPoint;
    }

    public static Direction Up { get; } = new("Up", new Point(0, -1));
    public static Direction Right { get; } = new("Right", new Point(1, 0));
    public static Direction Down { get; } = new("Down", new Point(0, 1));
    public static Direction Left { get; } = new("Left", new Point(-1, 0));
    public static Direction None { get; } = new("None", Point.Zero);

    public Direction Previous
    {
        get
        {
            if (this == Direction.Up)
            {
                return Direction.Left;
            }

            if (this == Direction.Right)
            {
                return Direction.Up;
            }

            if (this == Direction.Down)
            {
                return Direction.Right;
            }

            if (this == Direction.Left)
            {
                return Direction.Down;
            }

            return Direction.None;
        }
    }

    public Direction Next
    {
        get
        {
            if (this == Direction.Up)
            {
                return Direction.Right;
            }

            if (this == Direction.Right)
            {
                return Direction.Down;
            }

            if (this == Direction.Down)
            {
                return Direction.Left;
            }

            if (this == Direction.Left)
            {
                return Direction.Up;
            }

            return Direction.None;
        }
    }

    public Direction Opposite
    {
        get
        {
            if (this == Direction.Up)
            {
                return Direction.Down;
            }

            if (this == Direction.Right)
            {
                return Direction.Left;
            }

            if (this == Direction.Down)
            {
                return Direction.Up;
            }

            if (this == Direction.Left)
            {
                return Direction.Right;
            }

            return Direction.None;
        }
    }

    public static implicit operator Point(Direction direction)
    {
        return direction._internalPoint;
    }

    public override string ToString()
    {
        return _name;
    }

    public Point ToPoint()
    {
        return _internalPoint;
    }

    public static Direction PointToDirection(Point point)
    {
        var absX = Math.Abs(point.X);
        var absY = Math.Abs(point.Y);
        if (absX > absY)
        {
            if (point.X < 0)
            {
                return Direction.Left;
            }

            if (point.X > 0)
            {
                return Direction.Right;
            }
        }

        if (absX < absY)
        {
            if (point.Y < 0)
            {
                return Direction.Up;
            }

            if (point.Y > 0)
            {
                return Direction.Down;
            }
        }

        return Direction.None;
    }

    public Vector2 ToGridCellSizedVector(float tileSize)
    {
        return ToPoint().ToVector2() * tileSize / 2;
    }

    public float Radians()
    {
        if (this == Direction.Up)
        {
            return MathF.PI;
        }

        if (this == Direction.Right)
        {
            return MathF.PI + MathF.PI / 2;
        }

        if (this == Direction.Down)
        {
            return 0;
        }

        if (this == Direction.Left)
        {
            return MathF.PI / 2;
        }

        return 0;
    }
    
    public Keys ToArrowKey()
    {
        if (this == Direction.Up)
        {
            return Keys.Up;
        }

        if (this == Direction.Down)
        {
            return Keys.Down;
        }

        if (this == Direction.Left)
        {
            return Keys.Left;
        }

        if (this == Direction.Right)
        {
            return Keys.Right;
        }

        throw new Exception($"Cannot get key from direction {this}");
    }
    
    public Keys ToWasd()
    {
        if (this == Direction.Up)
        {
            return Keys.W;
        }

        if (this == Direction.Down)
        {
            return Keys.S;
        }

        if (this == Direction.Left)
        {
            return Keys.A;
        }

        if (this == Direction.Right)
        {
            return Keys.D;
        }

        throw new Exception($"Cannot get key from direction {this}");
    }
    
    public Buttons ToDPadButton()
    {
        if (this == Direction.Up)
        {
            return Buttons.DPadUp;
        }

        if (this == Direction.Down)
        {
            return Buttons.DPadDown;
        }

        if (this == Direction.Left)
        {
            return Buttons.DPadLeft;
        }

        if (this == Direction.Right)
        {
            return Buttons.DPadRight;
        }

        throw new Exception($"Cannot get key from direction {this}");
    }

    public override bool Equals(object? obj)
    {
        return obj is Direction direction &&
               _internalPoint.Equals(direction._internalPoint);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_internalPoint);
    }

    public static bool operator ==(Direction lhs, Direction rhs)
    {
        return lhs._internalPoint == rhs._internalPoint;
    }

    public static bool operator !=(Direction lhs, Direction rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    ///     All of the "real" directions (excluding "None")
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Direction> EachCardinal()
    {
        yield return Direction.Right;
        yield return Direction.Down;
        yield return Direction.Left;
        yield return Direction.Up;
    }
}
