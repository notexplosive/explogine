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

            if (element.Children.HasValue)
            {
                var childElements = Layout.ConvertToFixedElements(element.Children.Value.Elements,
                    element.Children.Value.RowSettings, element.GetSize());

                var childArrangement = Layout.CreateRow(elementRectangle.TopLeft,
                    element.Children.Value.RowSettings, childElements);

                foreach (var keyVal in childArrangement)
                {
                    namedRects.Add(keyVal.Key, keyVal.Value);
                }
            }
        }

        var totalSize = Vector2Extensions.FromAxisFirst(settings.Axis, alongPosition.GetAxis(settings.Axis),
            usedPerpendicularSize);

        return new Arrangement(namedRects, new RectangleF(startingPosition, totalSize));
    }

    public static Arrangement CreateRowWithin(RectangleF outerRectangle, RowSettings settings, IElement[] rawElements)
    {
        var elements = Layout.ConvertToFixedElements(rawElements, settings, outerRectangle.Size);
        return Layout.CreateRow(outerRectangle.TopLeft, settings, elements);
    }

    private static Element[] ConvertToFixedElements(IElement[] rawElements, RowSettings settings, Vector2 outerSize)
    {
        var elements = Layout.GetElements(rawElements, settings.Axis);
        var indexOfUnsizedElements = new HashSet<int>();
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            if (element.X is not FixedEdgeSize || element.Y is not FixedEdgeSize)
            {
                indexOfUnsizedElements.Add(i);
            }
        }

        var totalAvailableSpace = outerSize;

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

            elements[i] = unsizedElement with {X = new FixedEdgeSize(size.X), Y = new FixedEdgeSize(size.Y)};
        }

        return elements;
    }

    private static Element[] GetElements(IElement[] rawElements, Axis axis)
    {
        var result = new Element[rawElements.Length];

        for (var i = 0; i < rawElements.Length; i++)
        {
            var rawElement = rawElements[i];

            if (rawElement is Element element)
            {
                result[i] = element;
            }
            else if (rawElement is DynamicElement dynamicElement)
            {
                result[i] = dynamicElement.GetElement(axis);
            }
            else
            {
                throw new Exception($"Unknown element type: {result[i].GetType().Name}");
            }
        }

        return result;
    }

    public class Arrangement : IEnumerable<KeyValuePair<string, RectangleF>>
    {
        private readonly OneToMany<string, RectangleF> _namedRects;

        public Arrangement(OneToMany<string, RectangleF> namedRects, RectangleF usedSpace)
        {
            _namedRects = namedRects;
            UsedSpace = usedSpace;
        }

        public RectangleF UsedSpace { get; }

        public IEnumerator<KeyValuePair<string, RectangleF>> GetEnumerator()
        {
            foreach (var keyValuePair in _namedRects)
            {
                yield return keyValuePair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<RectangleF> FindElements(string name)
        {
            return _namedRects.Get(name);
        }

        public RectangleF FindElement(string name)
        {
            var matchingElements = FindElements(name);

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

        public IEnumerable<RectangleF> AllElements()
        {
            foreach (var rect in _namedRects.Values)
            {
                yield return rect;
            }
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

    public interface IElement
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

    private readonly record struct ElementBlankName : IElementName;

    private readonly record struct StretchedEdgeSize : IEdgeSize;

    public readonly record struct FixedEdgeSize(float Amount) : IEdgeSize
    {
        public static implicit operator float(FixedEdgeSize size)
        {
            return size.Amount;
        }
    }

    public readonly record struct ElementChildren(RowSettings RowSettings, IElement[] Elements);

    public readonly record struct Element
        (IElementName Name, IEdgeSize X, IEdgeSize Y, ElementChildren? Children = null) : IElement
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

        public static Element StretchedBoth(string name)
        {
            return new Element(new ElementName(name), new StretchedEdgeSize(), new StretchedEdgeSize());
        }

        public static IElement StretchedAlong(string name, float perpendicularSize)
        {
            return new DynamicElement(alongAxis =>
            {
                return alongAxis.ReturnIfXElseY(
                    () => new Element(new ElementName(name), new StretchedEdgeSize(),
                        new FixedEdgeSize(perpendicularSize)),
                    () => new Element(new ElementName(name), new FixedEdgeSize(perpendicularSize),
                        new StretchedEdgeSize())
                );
            });
        }

        public static IElement StretchedPerpendicular(string name, float alongSize)
        {
            return new DynamicElement(alongAxis =>
            {
                return alongAxis.ReturnIfXElseY(
                    () => new Element(new ElementName(name), new FixedEdgeSize(alongSize), new StretchedEdgeSize()),
                    () => new Element(new ElementName(name), new StretchedEdgeSize(), new FixedEdgeSize(alongSize))
                );
            });
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

        public Vector2 GetSize()
        {
            if (X is FixedEdgeSize fixedX && Y is FixedEdgeSize fixedY)
            {
                return new Vector2(fixedX, fixedY);
            }

            throw new Exception("Cannot get size");
        }

        public Element WithChildren(RowSettings rowSettings, IElement[] elements)
        {
            return this with {Children = new ElementChildren(rowSettings, elements)};
        }
    }

    private readonly record struct DynamicElement : IElement
    {
        private readonly Func<Axis, Element> _fromAxisFunction;

        public DynamicElement(Func<Axis, Element> fromAxisFunction)
        {
            _fromAxisFunction = fromAxisFunction;
        }

        public Element GetElement(Axis axis)
        {
            return _fromAxisFunction(axis);
        }
    }
}
