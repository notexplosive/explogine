using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Data;

public class Direction
{
    private readonly Point _internalPoint;

    private Direction(string name, Point givenPoint)
    {
        Name = name;
        _internalPoint = givenPoint;
    }

    public string Name { get; }

    public static Direction Up { get; } = new("Up", new Point(0, -1));
    public static Direction Right { get; } = new("Right", new Point(1, 0));
    public static Direction Down { get; } = new("Down", new Point(0, 1));
    public static Direction Left { get; } = new("Left", new Point(-1, 0));
    public static Direction None { get; } = new("None", Point.Zero);

    public Direction Previous
    {
        get
        {
            if (this == Up)
            {
                return Left;
            }

            if (this == Right)
            {
                return Up;
            }

            if (this == Down)
            {
                return Right;
            }

            if (this == Left)
            {
                return Down;
            }

            return None;
        }
    }

    public Direction Next
    {
        get
        {
            if (this == Up)
            {
                return Right;
            }

            if (this == Right)
            {
                return Down;
            }

            if (this == Down)
            {
                return Left;
            }

            if (this == Left)
            {
                return Up;
            }

            return None;
        }
    }

    public Direction Opposite
    {
        get
        {
            if (this == Up)
            {
                return Down;
            }

            if (this == Right)
            {
                return Left;
            }

            if (this == Down)
            {
                return Up;
            }

            if (this == Left)
            {
                return Right;
            }

            return None;
        }
    }

    public static implicit operator Point(Direction direction)
    {
        return direction._internalPoint;
    }

    public override string ToString()
    {
        return Name;
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
                return Left;
            }

            if (point.X > 0)
            {
                return Right;
            }
        }

        if (absX < absY)
        {
            if (point.Y < 0)
            {
                return Up;
            }

            if (point.Y > 0)
            {
                return Down;
            }
        }

        return None;
    }

    public Vector2 ToVector()
    {
        return ToPoint().ToVector2();
    }

    /// <summary>
    ///     Returns vector with magnitude = tileSize / 2
    /// </summary>
    public Vector2 ToGridCellSizedVector(float tileSize)
    {
        return ToPoint().ToVector2() * tileSize / 2;
    }

    public float Radians()
    {
        if (this == Up)
        {
            return MathF.PI;
        }

        if (this == Right)
        {
            return MathF.PI + MathF.PI / 2;
        }

        if (this == Down)
        {
            return 0;
        }

        if (this == Left)
        {
            return MathF.PI / 2;
        }

        return 0;
    }

    public Keys ToArrowKey()
    {
        if (this == Up)
        {
            return Keys.Up;
        }

        if (this == Down)
        {
            return Keys.Down;
        }

        if (this == Left)
        {
            return Keys.Left;
        }

        if (this == Right)
        {
            return Keys.Right;
        }

        throw new Exception($"Cannot get key from direction {this}");
    }

    public Keys ToWasd()
    {
        if (this == Up)
        {
            return Keys.W;
        }

        if (this == Down)
        {
            return Keys.S;
        }

        if (this == Left)
        {
            return Keys.A;
        }

        if (this == Right)
        {
            return Keys.D;
        }

        throw new Exception($"Cannot get key from direction {this}");
    }

    public Buttons ToDPadButton()
    {
        if (this == Up)
        {
            return Buttons.DPadUp;
        }

        if (this == Down)
        {
            return Buttons.DPadDown;
        }

        if (this == Left)
        {
            return Buttons.DPadLeft;
        }

        if (this == Right)
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

    public static bool operator ==(Direction? lhs, Direction? rhs)
    {
        if (lhs is not null && rhs is not null)
        {
            return lhs._internalPoint == rhs._internalPoint;
        }

        return lhs is null && rhs is null;
    }

    public static bool operator !=(Direction? lhs, Direction? rhs)
    {
        return !(lhs == rhs);
    }

    /// <summary>
    ///     All of the "real" directions (excluding "None")
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Direction> EachCardinal()
    {
        yield return Right;
        yield return Down;
        yield return Left;
        yield return Up;
    }

    public static Direction FromName(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return None;
        }

        foreach (var direction in EachCardinal())
        {
            if (direction.Name.ToLower() == name.ToLower())
            {
                return direction;
            }
        }

        return None;
    }

    public Axis GetAxis()
    {
        if (this == Left || this == Right)
        {
            return Axis.X;
        }

        if (this == Up || this == Down)
        {
            return Axis.Y;
        }

        throw new Exception($"Direction {Name} does not have an axis");
    }

    public static Direction EstimateFromVector(Vector2 vector, float tolerance = 0.25f)
    {
        var toleranceSquared = tolerance * tolerance;
        foreach (var direction in EachCardinal())
        {
            if ((vector.Normalized() - direction.ToVector()).LengthSquared() < toleranceSquared)
            {
                return direction;
            }
        }

        return None;
    }
}
