using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.AssetManagement;

public enum NinepatchIndex
{
    TopLeft = 0,
    TopCenter = 1,
    TopRight = 2,
    LeftCenter = 3,
    Center = 4,
    RightCenter = 5,
    BottomLeft = 6,
    BottomCenter = 7,
    BottomRight = 8
}

public enum Side
{
    Right,
    Bottom,
    Left,
    Top
}

public readonly struct NinepatchRects
{
    public readonly Rectangle[] Raw;
    private readonly int[] SidePixelBuffers;
    public readonly Rectangle Inner;
    public readonly Rectangle Outer;
    public readonly bool IsValidNinepatch;

    public NinepatchRects(Rectangle outer, Rectangle inner)
    {
        Debug.Assert(outer.Contains(inner), "InnerRect is not contained by OuterRect");

        var topBuffer = inner.Top - outer.Top;
        var rightBuffer = outer.Right - inner.Right;
        var leftBuffer = inner.Left - outer.Left;
        var bottomBuffer = outer.Bottom - inner.Bottom;

        SidePixelBuffers = new int[4]
        {
            rightBuffer,
            bottomBuffer,
            leftBuffer,
            topBuffer
        };

        Raw = new Rectangle[9]
        {
            new(outer.Left, outer.Top, SidePixelBuffers[(int) Side.Left],
                SidePixelBuffers[(int) Side.Top]),
            new(inner.Left, outer.Top, inner.Width, SidePixelBuffers[(int) Side.Top]),
            new(inner.Right, outer.Top, SidePixelBuffers[(int) Side.Right],
                SidePixelBuffers[(int) Side.Top]),
            new(outer.Left, inner.Top, SidePixelBuffers[(int) Side.Left], inner.Height),
            inner,
            new(inner.Right, inner.Top, SidePixelBuffers[(int) Side.Right], inner.Height),
            new(outer.Left, inner.Bottom, SidePixelBuffers[(int) Side.Left],
                SidePixelBuffers[(int) Side.Bottom]),
            new(inner.Left, inner.Bottom, inner.Width, SidePixelBuffers[(int) Side.Bottom]),
            new(inner.Right, inner.Bottom, SidePixelBuffers[(int) Side.Right],
                SidePixelBuffers[(int) Side.Bottom])
        };
        Inner = inner;
        Outer = outer;

        IsValidNinepatch = true;
        foreach (var rect in Raw)
        {
            if (rect.Width * rect.Height == 0)
            {
                IsValidNinepatch = false;
            }
        }
    }

    public Rectangle TopLeft => Raw[(int) NinepatchIndex.TopLeft];
    public Rectangle TopCenter => Raw[(int) NinepatchIndex.TopCenter];
    public Rectangle TopRight => Raw[(int) NinepatchIndex.TopRight];
    public Rectangle LeftCenter => Raw[(int) NinepatchIndex.LeftCenter];
    public Rectangle Center => Raw[(int) NinepatchIndex.Center];
    public Rectangle RightCenter => Raw[(int) NinepatchIndex.RightCenter];
    public Rectangle BottomLeft => Raw[(int) NinepatchIndex.BottomLeft];
    public Rectangle BottomCenter => Raw[(int) NinepatchIndex.BottomCenter];
    public Rectangle BottomRight => Raw[(int) NinepatchIndex.BottomRight];
    public int LeftBuffer => SidePixelBuffers[(int) Side.Left];
    public int RightBuffer => SidePixelBuffers[(int) Side.Right];
    public int TopBuffer => SidePixelBuffers[(int) Side.Top];
    public int BottomBuffer => SidePixelBuffers[(int) Side.Bottom];

    public bool IsValidHorizontalThreepatch
    {
        get
        {
            foreach (var rect in new[] {LeftCenter, Center, RightCenter})
            {
                if (rect.Width * rect.Height == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool IsValidVerticalThreepatch
    {
        get
        {
            foreach (var rect in new[] {TopCenter, Center, BottomCenter})
            {
                if (rect.Width * rect.Height == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
