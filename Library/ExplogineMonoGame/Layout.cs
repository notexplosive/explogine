using System;
using System.Collections;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public static class Layout
{
    /// <summary>
    ///     Creates a row of rectangles
    /// </summary>
    /// <param name="startingPosition">Top left position of the first rectangle</param>
    /// <param name="settings"></param>
    /// <param name="elements">Specifications for each element in the row</param>
    /// <returns></returns>
    public static Arrangement CreateRow(Vector2 startingPosition, RowSettings settings, Element[] elements)
    {
        var namedRects = new OneToMany<string, RectangleF>();
        var alongPosition = new Vector2();
        var usedPerpendicularSize = 0f;
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var elementRectangle = new RectangleF(startingPosition + alongPosition, element.GetSize());
            alongPosition += elementRectangle.Size.JustAxis(settings.Axis);
            if (i < elements.Length - 1)
            {
                alongPosition += new Vector2(settings.PaddingBetweenElements).JustAxis(settings.Axis);
            }

            if (element.Name is ElementName name)
            {
                namedRects.Add(name, elementRectangle);

                var oppositeAxis = settings.Axis.Opposite();
                usedPerpendicularSize = Math.Max(usedPerpendicularSize, elementRectangle.Size.GetAxis(oppositeAxis));
            }
        }

        var totalSize = Vector2Extensions.FromAxisFirst(settings.Axis, alongPosition.GetAxis(settings.Axis),
            usedPerpendicularSize);

        return new Arrangement(namedRects, new RectangleF(startingPosition, totalSize));
    }

    public static Arrangement CreateRowWithin(RectangleF outerRectangle, RowSettings settings, Element[] elements)
    {
        var indexOfUnsizedElements = new HashSet<int>();
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            if (element.X is not FixedEdgeSize || element.Y is not FixedEdgeSize)
            {
                indexOfUnsizedElements.Add(i);
            }
        }

        var totalAvailableSpace = outerRectangle.Size;

        var numberOfStretchedElementsOnAxis = new Dictionary<Axis, int>
        {
            {Axis.X, 0},
            {Axis.Y, 0}
        };

        foreach (var element in elements)
        {
            foreach (var axis in Axis.Each)
            {
                var sizeAlongAxis = element.GetAxis(axis);
                if (sizeAlongAxis is FixedEdgeSize fixedEdgeSize)
                {
                    if (axis == settings.Axis)
                    {
                        totalAvailableSpace.SetAxis(axis, totalAvailableSpace.GetAxis(axis) - fixedEdgeSize.Amount);
                    }
                }
            }
        }

        totalAvailableSpace.SetAxis(settings.Axis,
            totalAvailableSpace.GetAxis(settings.Axis) - settings.PaddingBetweenElements * (elements.Length - 1));

        // tally up all stretched elements per axis
        foreach (var i in indexOfUnsizedElements)
        {
            foreach (var axis in Axis.Each)
            {
                var sizeAlongAxis = elements[i].GetAxis(axis);
                if (sizeAlongAxis is StretchedEdgeSize)
                {
                    numberOfStretchedElementsOnAxis[axis]++;
                }
            }
        }

        // replace stretched sizes with static sizes
        foreach (var i in indexOfUnsizedElements)
        {
            var unsizedElement = elements[i];

            var size = Vector2.Zero;
            foreach (var axis in Axis.Each)
            {
                var sizeAlongAxis = unsizedElement.GetAxis(axis);

                if (sizeAlongAxis is FixedEdgeSize fixedEdgeSize)
                {
                    size.SetAxis(axis, fixedEdgeSize.Amount);
                }
                else if (sizeAlongAxis is StretchedEdgeSize)
                {
                    var isAlong = axis == settings.Axis;
                    if (isAlong)
                    {
                        var spaceToUse = totalAvailableSpace.GetAxis(axis) / numberOfStretchedElementsOnAxis[axis];
                        size.SetAxis(axis, spaceToUse);
                    }
                    else
                    {
                        size.SetAxis(axis, totalAvailableSpace.GetAxis(axis));
                    }
                }
            }

            elements[i] = new Element(unsizedElement.Name, new FixedEdgeSize(size.X), new FixedEdgeSize(size.Y));
        }

        return Layout.CreateRow(outerRectangle.TopLeft, settings, elements);
    }

    public class Arrangement : IEnumerable<RectangleF>
    {
        private readonly OneToMany<string, RectangleF> _namedRects;

        public Arrangement(OneToMany<string, RectangleF> namedRects, RectangleF usedSpace)
        {
            _namedRects = namedRects;
            UsedSpace = usedSpace;
        }

        public RectangleF UsedSpace { get; }

        public IEnumerator<RectangleF> GetEnumerator()
        {
            foreach (var rect in _namedRects.Values)
            {
                yield return rect;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<RectangleF> GetElements(string name)
        {
            return _namedRects.Get(name);
        }

        public RectangleF GetElement(string name)
        {
            var matchingElements = GetElements(name);

            if (matchingElements.Count == 0)
            {
                throw new Exception($"No element found '{name}'");
            }

            if (matchingElements.Count > 1)
            {
                Client.Debug.LogWarning(
                    $"Attempted to get element '{name}' but found {matchingElements.Count} matches.");
            }

            return matchingElements[0];
        }
    }

    public readonly record struct RowSettings(Orientation Orientation, int PaddingBetweenElements)
    {
        public Axis Axis
        {
            get
            {
                return Orientation switch
                {
                    Orientation.Horizontal => Axis.X,
                    Orientation.Vertical => Axis.Y,
                    _ => throw new Exception("Invalid orientation")
                };
            }
        }
    }

    public interface IEdgeSize
    {
    }

    public interface IElementName
    {
    }

    public readonly record struct ElementName(string Text) : IElementName
    {
        public override string ToString()
        {
            return Text;
        }

        public static implicit operator string(ElementName name)
        {
            return name.Text;
        }
    }

    public readonly record struct ElementBlankName : IElementName;

    public readonly record struct StretchedEdgeSize : IEdgeSize;

    public readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
    {
        public static implicit operator float(FixedEdgeSize size)
        {
            return size.Amount;
        }
    }

    public readonly record struct Element(IElementName Name, IEdgeSize X, IEdgeSize Y)
    {
        public static Element Fixed(string name, float x, float y)
        {
            return new Element(new ElementName(name), new FixedEdgeSize(x), new FixedEdgeSize(y));
        }

        public static Element StretchedHorizontal(string name, float verticalSize)
        {
            return new Element(new ElementName(name), new StretchedEdgeSize(), new FixedEdgeSize(verticalSize));
        }

        public static Element StretchedVertical(string name, float horizontalSize)
        {
            return new Element(new ElementName(name), new FixedEdgeSize(horizontalSize), new StretchedEdgeSize());
        }

        public static Element FixedSpacer(float size)
        {
            return new Element(new ElementBlankName(), new FixedEdgeSize(size), new FixedEdgeSize(size));
        }

        public Vector2 GetSize()
        {
            if (X is FixedEdgeSize fixedX && Y is FixedEdgeSize fixedY)
            {
                return new Vector2(fixedX, fixedY);
            }

            throw new Exception("Cannot get size");
        }

        public IEdgeSize GetAxis(Axis axis)
        {
            if (axis == Axis.X)
            {
                return X;
            }

            if (axis == Axis.Y)
            {
                return Y;
            }

            throw new Exception("Unknown axis");
        }
    }
}
