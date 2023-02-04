using ExplogineMonoGame.Data;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestRectangleF
{
    [Fact]
    public void properties_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        rectangleF.Bottom.Should().Be(rectangleI.Bottom);
        rectangleF.Top.Should().Be(rectangleI.Top);
        rectangleF.Right.Should().Be(rectangleI.Right);
        rectangleF.Left.Should().Be(rectangleI.Left);
        rectangleF.Center.Should().Be(rectangleI.Center.ToVector2());
        rectangleF.X.Should().Be(rectangleI.X);
        rectangleF.Y.Should().Be(rectangleI.Y);
        rectangleF.Width.Should().Be(rectangleI.Width);
        rectangleF.Height.Should().Be(rectangleI.Height);
    }

    [Fact]
    public void contains_point_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var containedPoint = new Point(250, 250);
        rectangleI.Contains(containedPoint).Should().BeTrue().And.Be(rectangleF.Contains(containedPoint));

        var edgePoint = new Point(300, 300);
        rectangleI.Contains(edgePoint).Should().BeFalse().And.Be(rectangleF.Contains(edgePoint));

        var outPoint = new Point(301, 300);
        rectangleI.Contains(outPoint).Should().BeFalse().And.Be(rectangleF.Contains(outPoint));

        var manyPoints = new[]
        {
            new(100, 100), // top left exactly
            new Point(101, 101), // just inside top left
            new Point(99, 99), // just out of top left

            new Point(150, 100), // along top
            new Point(150, 101), // just in top
            new Point(150, 99), // just out top

            new Point(150, 300), // along bottom
            new Point(150, 299), // just in bottom
            new Point(150, 301), // just out bottom

            new Point(100, 300), // bottom left
            new Point(101, 301), // just in bottom left
            new Point(101, 299), // just out bottom left

            new Point(300, 300), // bottom right
            new Point(299, 299), // just in bottom right
            new Point(301, 301) // just out bottom right
        };

        foreach (var point in manyPoints)
        {
            rectangleF.Contains(point).Should().Be(rectangleI.Contains(point),
                $"{point} is {(rectangleI.Contains(point) ? "inside" : "outside")}");
        }
    }

    [Fact]
    public void contains_vector_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var containedVector = new Vector2(250, 250);
        rectangleI.Contains(containedVector).Should().BeTrue().And.Be(rectangleF.Contains(containedVector));

        var edgeVector = new Vector2(300, 300);
        rectangleI.Contains(edgeVector).Should().BeFalse().And.Be(rectangleF.Contains(edgeVector));

        var outVector = new Vector2(301, 300);
        rectangleI.Contains(outVector).Should().BeFalse().And.Be(rectangleF.Contains(outVector));
    }

    [Fact]
    public void contains_floats_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var containedFloats = new Vector2(250, 250);
        rectangleI.Contains(containedFloats.X, containedFloats.Y).Should().BeTrue().And
            .Be(rectangleF.Contains(containedFloats.X, containedFloats.Y));

        var edgeFloats = new Vector2(300, 300);
        rectangleI.Contains(edgeFloats.X, edgeFloats.Y).Should().BeFalse().And
            .Be(rectangleF.Contains(edgeFloats.X, edgeFloats.Y));

        var outFloats = new Vector2(301, 300);
        rectangleI.Contains(outFloats.X, outFloats.Y).Should().BeFalse().And
            .Be(rectangleF.Contains(outFloats.X, outFloats.Y));
    }

    [Fact]
    public void contains_ints_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var containedInts = new Point(250, 250);
        rectangleI.Contains(containedInts.X, containedInts.Y).Should().BeTrue().And
            .Be(rectangleF.Contains(containedInts.X, containedInts.Y));

        var edgeInts = new Point(300, 300);
        rectangleI.Contains(edgeInts.X, edgeInts.Y).Should().BeFalse().And
            .Be(rectangleF.Contains(edgeInts.X, edgeInts.Y));

        var outInts = new Point(301, 300);
        rectangleI.Contains(outInts.X, outInts.Y).Should().BeFalse().And
            .Be(rectangleF.Contains(outInts.X, outInts.Y));
    }

    [Fact]
    public void contains_rect_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var containedRect = new Rectangle(new Point(250, 250), new Point(10, 10));
        rectangleI.Contains(containedRect).Should().BeTrue().And.Be(rectangleF.Contains(containedRect));

        var containedButOverSizedRect = new Rectangle(new Point(250, 250), new Point(100, 10));
        rectangleI.Contains(containedButOverSizedRect).Should().BeFalse().And
            .Be(rectangleF.Contains(containedButOverSizedRect));

        var edgeRect = new Rectangle(new Point(280, 280), new Point(20));
        rectangleI.Contains(edgeRect).Should().BeTrue().And.Be(rectangleF.Contains(edgeRect));

        var fullyOutRect = new Rectangle(new Point(301, 300), new Point(10, 10));
        rectangleI.Contains(fullyOutRect).Should().BeFalse().And.Be(rectangleF.Contains(fullyOutRect));
    }

    [Fact]
    public void deconstruct_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        rectangleI.Deconstruct(out var x, out var y, out var width, out var height);
        rectangleF.Deconstruct(out var fX, out var fY, out var fWidth, out var fHeight);

        fX.Should().Be(x);
        fY.Should().Be(y);
        fWidth.Should().Be(width);
        fHeight.Should().Be(height);
    }

    [Fact]
    public void inflate_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        rectangleF.Inflate(10, 10);
        rectangleI.Inflate(10, 10);

        rectangleF.Width.Should().Be(rectangleI.Width);
        rectangleF.Height.Should().Be(rectangleI.Height);
        rectangleF.X.Should().Be(rectangleI.X);
        rectangleF.Y.Should().Be(rectangleI.Y);
    }

    [Fact]
    public void intersect_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleI = rectangleF.ToRectangle();

        var slightOverlap = new Rectangle(50, 150, 100, 100);
        var exactOverlap = new Rectangle(100, 100, 200, 200);
        var envelop = new Rectangle(0, 0, 400, 400);
        var disjoint = new Rectangle(400, 400, 10, 10);
        var disjointToTheLeft = new Rectangle(-100, 100, 10, 10);

        rectangleF.Intersects(slightOverlap.ToRectangleF()).Should().Be(rectangleI.Intersects(slightOverlap)).And
            .BeTrue();
        rectangleF.Intersects(exactOverlap.ToRectangleF()).Should().Be(rectangleI.Intersects(exactOverlap)).And
            .BeTrue();
        rectangleF.Intersects(envelop.ToRectangleF()).Should().Be(rectangleI.Intersects(envelop)).And.BeTrue();
        rectangleF.Intersects(disjoint.ToRectangleF()).Should().Be(rectangleI.Intersects(disjoint)).And.BeFalse();
        rectangleF.Intersects(disjointToTheLeft.ToRectangleF()).Should().Be(rectangleI.Intersects(disjointToTheLeft)).And.BeFalse();
    }

    [Fact]
    public void static_intersect_same_as_rect()
    {
        var subject = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));

        var disjoint = new RectangleF(new Vector2(400, 100), new Vector2(20, 20));
        var touching = new RectangleF(new Vector2(300, 100), new Vector2(20, 20));
        var sharedEdge = new RectangleF(new Vector2(150, 90), new Vector2(20, 20));
        var sharedCorner = new RectangleF(new Vector2(275, 75), new Vector2(50, 50));
        var enveloped = new RectangleF(new Vector2(150, 150), new Vector2(20, 20));
        var disjointToTheLeft = new RectangleF(-100, 100, 10, 10);

        RectangleF.Intersect(subject, disjoint)
            .Should().Be(RectangleF.Empty)
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), disjoint.ToRectangle()));

        RectangleF.Intersect(subject, touching)
            .Should().Be(RectangleF.Empty)
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), touching.ToRectangle()));

        RectangleF.Intersect(subject, sharedEdge)
            .Should().Be(new Rectangle(150, 100, 20, 10))
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), sharedEdge.ToRectangle()));

        RectangleF.Intersect(subject, sharedCorner)
            .Should().Be(new Rectangle(275, 100, 25, 25))
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), sharedCorner.ToRectangle()));

        RectangleF.Intersect(subject, enveloped)
            .Should().Be(new Rectangle(150, 150, 20, 20))
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), enveloped.ToRectangle()));
        
        RectangleF.Intersect(subject, disjointToTheLeft)
            .Should().Be(Rectangle.Empty)
            .And.Be(Rectangle.Intersect(subject.ToRectangle(), disjointToTheLeft.ToRectangle()));
    }

    [Fact]
    public void static_union_same_as_rect()
    {
        var rectangleF1 = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));
        var rectangleF2 = new RectangleF(new Vector2(200, 100), new Vector2(20, 20));

        var rectangleI1 = rectangleF1.ToRectangle();
        var rectangleI2 = rectangleF2.ToRectangle();

        var unionI = Rectangle.Union(rectangleI1, rectangleI2);
        var unionF = RectangleF.Union(rectangleF1, rectangleF2);
        unionF.Should().Be(unionI);
    }

    [Fact]
    public void offset_same_as_rect()
    {
        var rectangleF = new RectangleF(new Vector2(100, 100), new Vector2(200, 200));

        {
            var point = new Point(10, 10);
            var rectF = rectangleF;
            var rect = rectangleF.ToRectangle();
            rect.Offset(point);
            rectF.Offset(point);
            rectF.Location.Should().Be(rect.Location.ToVector2());
        }

        {
            var vector = new Vector2(20, 20);
            var rectF = rectangleF;
            var rect = rectangleF.ToRectangle();
            rect.Offset(vector);
            rectF.Offset(vector);
            rectF.Location.Should().Be(rect.Location.ToVector2());
        }

        {
            var intOffset = 15;
            var rectF = rectangleF;
            var rect = rectangleF.ToRectangle();
            rect.Offset(intOffset, intOffset);
            rectF.Offset(intOffset, intOffset);
            rectF.Location.Should().Be(rect.Location.ToVector2());
        }

        {
            var floatOffset = 100f;
            var rectF = rectangleF;
            var rect = rectangleF.ToRectangle();
            rect.Offset(floatOffset, floatOffset);
            rectF.Offset(floatOffset, floatOffset);
            rectF.Location.Should().Be(rect.Location.ToVector2());
        }
    }

    [Fact]
    public void constrain_to_outer_rect_pinning()
    {
        var outer = new RectangleF(100, 50, 200, 300);

        // positions
        var middle = 120;
        var left = 20;
        var top = 20;
        var below = 350;
        var right = 350;
        var topLeftPosition = new Vector2(left, top);
        var topPosition = new Vector2(middle, top);
        var topRightPosition = new Vector2(right, top);
        var bottomPosition = new Vector2(middle, below);
        var rightPosition = new Vector2(right, middle);
        var leftPosition = new Vector2(left, middle);
        var bottomRightPosition = new Vector2(right, below);

        var smallSize = new Vector2(100, 100);
        new RectangleF(topLeftPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, smallSize));
        new RectangleF(topPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(topPosition.X, outer.Y), smallSize));
        new RectangleF(topRightPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - smallSize.X, outer.Y), smallSize));
        new RectangleF(bottomPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(bottomPosition.X, outer.Bottom - smallSize.Y), smallSize));
        new RectangleF(rightPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - smallSize.X, rightPosition.Y), smallSize));
        new RectangleF(leftPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Left, leftPosition.Y), smallSize));
        new RectangleF(bottomRightPosition, smallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - smallSize.X, outer.Bottom - smallSize.Y), smallSize));
        
        // If it's too wide we align to the left of the rectangle and do our best with the rest
        var tooWideSize = new Vector2(250, 100);
        new RectangleF(topLeftPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, tooWideSize));
        new RectangleF(topPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, outer.Y), tooWideSize));
        new RectangleF(topRightPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, outer.Y), tooWideSize));
        new RectangleF(bottomPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, outer.Bottom - tooWideSize.Y), tooWideSize));
        new RectangleF(rightPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, rightPosition.Y), tooWideSize));
        new RectangleF(leftPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, leftPosition.Y), tooWideSize));
        new RectangleF(bottomRightPosition, tooWideSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, outer.Bottom - tooWideSize.Y), tooWideSize));

        // If it's too tall we align the top of the rectangle and do our best with the rest
        var tooTallSize = new Vector2(100, 350);
        new RectangleF(topLeftPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, tooTallSize));
        new RectangleF(topPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(topPosition.X, outer.Y), tooTallSize));
        new RectangleF(topRightPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - tooTallSize.X, outer.Y), tooTallSize));
        new RectangleF(bottomPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(bottomPosition.X, outer.Y), tooTallSize));
        new RectangleF(rightPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - tooTallSize.X, outer.Y), tooTallSize));
        new RectangleF(leftPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.X, outer.Y), tooTallSize));
        new RectangleF(bottomRightPosition, tooTallSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(new Vector2(outer.Right - tooTallSize.X, outer.Y), tooTallSize));
        
        // If it's an exact match it fits perfectly every time
        var exactMatchSize = outer.Size;
        new RectangleF(topLeftPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(topPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(topRightPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(bottomPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(rightPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(leftPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
        new RectangleF(bottomRightPosition, exactMatchSize).ConstrainedTo(outer)
            .Should().Be(new RectangleF(outer.TopLeft, exactMatchSize));
    }
}
