using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace ExplogineMonoGame.Layout;

public static class L
{
    public static LayoutElement FixedElement(float x, float y)
    {
        return new LayoutElement(new ElementBlankName(), new FixedEdgeSize(x), new FixedEdgeSize(y));
    }

    public static LayoutElement FixedElement(string name, float x, float y)
    {
        return L.FixedElement(x, y) with {Name = new ElementName(name)};
    }

    public static LayoutElement FixedElement(string name, Vector2 size)
    {
        return L.FixedElement(name, size.X, size.Y);
    }

    public static LayoutElement Group(LayoutElement parentElement, Style settings,
        LayoutElement[] children)
    {
        return parentElement with {Children = new LayoutElementGroup(settings, children)};
    }

    public static LayoutElement FillVertical(string name, float horizontalSize)
    {
        return L.FillVertical(horizontalSize) with {Name = new ElementName(name)};
    }

    public static LayoutElement FillVertical(float horizontalSize)
    {
        return new LayoutElement(new ElementBlankName(), new FixedEdgeSize(horizontalSize), new FillEdgeSize());
    }

    public static LayoutElement FillHorizontal(string name, float verticalSize)
    {
        return L.FillHorizontal(verticalSize) with {Name = new ElementName(name)};
    }

    public static LayoutElement FillHorizontal(float verticalSize)
    {
        return new LayoutElement(new ElementBlankName(), new FillEdgeSize(), new FixedEdgeSize(verticalSize));
    }

    public static LayoutElement Fill(Orientation orientation, string name, float perpendicularSize)
    {
        switch (orientation)
        {
            case Orientation.Horizontal:
                return L.FillHorizontal(name, perpendicularSize);
            case Orientation.Vertical:
                return L.FillHorizontal(name, perpendicularSize);
            default:
                throw new Exception("Unknown orientation");
        }
    }

    public static LayoutElement FillBoth(string name)
    {
        return new LayoutElement(new ElementName(name), new FillEdgeSize(), new FillEdgeSize());
    }

    public static LayoutElement FillBoth()
    {
        return new LayoutElement(new ElementBlankName(), new FillEdgeSize(), new FillEdgeSize());
    }

    public static LayoutArrangement Compute(RectangleF outerRectangle, LayoutElementGroup root)
    {
        return L.ComputeNested(outerRectangle, root);
    }

    public static string ToJson(LayoutArrangement arrangement)
    {
        return JsonConvert.SerializeObject(new LayoutSerialization.SerializedGroup(arrangement.RawGroup),
            Formatting.Indented);
    }

    public static LayoutArrangement FromJson(RectangleF outerRectangle, string json)
    {
        var serialized = JsonConvert.DeserializeObject<LayoutSerialization.SerializedGroup>(json);
        var group = serialized.Deserialize();

        return L.Compute(outerRectangle, group);
    }

    private static LayoutArrangement ComputeNested(RectangleF outerRectangle, LayoutElementGroup group,
        int nestLevel = 0)
    {
        var settings = group.Style;
        var rawElements = group.Elements;
        outerRectangle.Inflate(-settings.Margin.X, -settings.Margin.Y);
        var elements = L.ConvertToFixedElements(rawElements, settings, outerRectangle.Size);
        var namedRects = new OneToMany<string, BakedLayoutElement>();
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

            if (element.Name is ElementName)
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
                namedRects.Add(name, new BakedLayoutElement(elementRectangle, name.Text, nestLevel));
            }

            if (element.Children.HasValue)
            {
                var childArrangement = L.ComputeNested(elementRectangle, element.Children.Value, nestLevel + 1);
                foreach (var keyVal in childArrangement)
                {
                    namedRects.Add(keyVal.Key, keyVal.Value);
                }
            }
        }
        
        return new LayoutArrangement(namedRects, usedRectangle, group);
    }

    private static LayoutElement[] ConvertToFixedElements(LayoutElement[] elements, Style settings,
        Vector2 outerSize)
    {
        var result = new LayoutElement[elements.Length];

        for (var i = 0; i < elements.Length; i++)
        {
            result[i] = elements[i];
        }

        var indexOfUnsizedElements = new HashSet<int>();
        for (var i = 0; i < result.Length; i++)
        {
            var element = result[i];
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

        foreach (var element in result)
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
            totalAvailableSpace.GetAxis(settings.Axis) - settings.PaddingBetweenElements * (result.Length - 1));

        // tally up all stretched elements per axis
        foreach (var i in indexOfUnsizedElements)
        {
            foreach (var axis in Axis.Each)
            {
                var sizeAlongAxis = result[i].GetAxis(axis);
                if (sizeAlongAxis is FillEdgeSize)
                {
                    numberOfStretchedElementsOnAxis[axis]++;
                }
            }
        }

        // replace stretched sizes with static sizes
        foreach (var i in indexOfUnsizedElements)
        {
            var unsizedElement = result[i];

            var size = Vector2.Zero;
            foreach (var axis in Axis.Each)
            {
                var sizeAlongAxis = unsizedElement.GetAxis(axis);

                if (sizeAlongAxis is FixedEdgeSize fixedEdgeSize)
                {
                    size.SetAxis(axis, fixedEdgeSize.Amount);
                }
                else if (sizeAlongAxis is FillEdgeSize)
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

            result[i] = unsizedElement with {X = new FixedEdgeSize(size.X), Y = new FixedEdgeSize(size.Y)};
        }

        return result;
    }

    public static LayoutElementGroup Root(Style settings, params LayoutElement[] elements)
    {
        return new LayoutElementGroup(settings, elements);
    }

    public static LayoutElement[] CreateMany(int size, LayoutElement sourceElement)
    {
        var elements = new LayoutElement[size];
        for (int i = 0; i < size; i++)
        {
            elements[i] = sourceElement;
        }

        return elements;
    }
}
