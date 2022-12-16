using System;
using System.Collections;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public class Arrangement : IEnumerable<KeyValuePair<string, BakedElement>>
{
    private readonly OneToMany<string, BakedElement> _namedRects;

    public Arrangement(OneToMany<string, BakedElement> namedRects, RectangleF usedSpace)
    {
        _namedRects = namedRects;
        UsedSpace = usedSpace;
    }

    public RectangleF UsedSpace { get; }

    public IEnumerator<KeyValuePair<string, BakedElement>> GetEnumerator()
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

    public List<BakedElement> FindElements(string name)
    {
        if (_namedRects.ContainsKey(name))
        {
            return _namedRects.Get(name);
        }

        return new List<BakedElement>();
    }

    public BakedElement FindElement(string name)
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

    public IEnumerable<BakedElement> AllElements()
    {
        foreach (var rect in _namedRects.Values)
        {
            yield return rect;
        }
    }
    
    public static Arrangement Create(RectangleF outerRectangle, RowSettings settings, IElement[] rawElements,
        int nestLevel = 0)
    {
        outerRectangle.Inflate(-settings.Margin.X, -settings.Margin.Y);
        var elements = ConvertToFixedElements(rawElements, settings, outerRectangle.Size);
        var namedRects = new OneToMany<string, BakedElement>();
        var estimatedPosition = new Vector2();
        var usedPerpendicularSize = 0f;

        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var naiveRectangle = new RectangleF(outerRectangle.TopLeft + estimatedPosition, element.GetSize());
            estimatedPosition += naiveRectangle.Size.JustAxis(settings.Axis);
            if (i < elements.Length - 1)
            {
                estimatedPosition += new Vector2(settings.PaddingBetweenElements).JustAxis(settings.Axis);
            }

            if (element.Name is ElementName name)
            {
                var oppositeAxis = settings.Axis.Opposite();
                usedPerpendicularSize = Math.Max(usedPerpendicularSize, naiveRectangle.Size.GetAxis(oppositeAxis));
            }
        }

        var usedSize = Vector2Extensions.FromAxisFirst(settings.Axis, estimatedPosition.GetAxis(settings.Axis),
            usedPerpendicularSize);

        var usedRectangle = RectangleF.FromSizeAlignedWithin(outerRectangle, usedSize, settings.Alignment);
        var alongPosition = new Vector2();
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var unalignedRectangle = new RectangleF(usedRectangle.TopLeft + alongPosition, element.GetSize());
            var alignedPosition = unalignedRectangle.Location;

            if (settings.Orientation == Orientation.Horizontal)
            {
                if (settings.Alignment.Vertical == VerticalAlignment.Center)
                {
                    alignedPosition.Y += usedRectangle.Size.Y / 2 - unalignedRectangle.Size.Y / 2f;
                }

                if (settings.Alignment.Vertical == VerticalAlignment.Bottom)
                {
                    alignedPosition.Y += usedRectangle.Size.Y - unalignedRectangle.Size.Y;
                }
            }
            else
            {
                if (settings.Alignment.Horizontal == HorizontalAlignment.Center)
                {
                    alignedPosition.X += usedRectangle.Size.X / 2 - unalignedRectangle.Size.X / 2f;
                }

                if (settings.Alignment.Horizontal == HorizontalAlignment.Right)
                {
                    alignedPosition.X += usedRectangle.Size.X - unalignedRectangle.Size.X;
                }
            }

            var elementRectangle = new RectangleF(alignedPosition, unalignedRectangle.Size);

            alongPosition += elementRectangle.Size.JustAxis(settings.Axis);
            if (i < elements.Length - 1)
            {
                alongPosition += new Vector2(settings.PaddingBetweenElements).JustAxis(settings.Axis);
            }

            if (element.Name is ElementName name)
            {
                namedRects.Add(name, new BakedElement(elementRectangle, name.Text, nestLevel));
            }

            if (element.Children.HasValue)
            {
                var childArrangement = Arrangement.Create(elementRectangle, element.Children.Value.RowSettings,
                    element.Children.Value.Elements, nestLevel + 1);
                foreach (var keyVal in childArrangement)
                {
                    namedRects.Add(keyVal.Key, keyVal.Value);
                }
            }
        }

        return new Arrangement(namedRects, usedRectangle);
    }

    private static Element[] ConvertToFixedElements(IElement[] rawElements, RowSettings settings, Vector2 outerSize)
    {
        var result = new Element[rawElements.Length];

        for (var i = 0; i < rawElements.Length; i++)
        {
            var rawElement = rawElements[i];

            if (rawElement is Element element1)
            {
                result[i] = element1;
            }
            else if (rawElement is DynamicElement dynamicElement)
            {
                result[i] = dynamicElement.GetElement(settings.Axis);
            }
            else
            {
                throw new Exception($"Unknown element type: {result[i].GetType().Name}");
            }
        }

        var elements = result;
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
}
