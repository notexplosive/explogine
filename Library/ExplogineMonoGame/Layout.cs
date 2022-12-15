using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var alongSize = new Vector2();
        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            var rectangle = new RectangleF(startingPosition + alongSize, element.GetSize());
            alongSize += rectangle.Size.JustAxis(settings.Axis);
            if (i < elements.Length - 1)
            {
                alongSize += new Vector2(settings.PaddingBetweenElements).JustAxis(settings.Axis);
            }

            if (element.Name is ElementName name)
            {
                namedRects.Add(name, rectangle);
            }
        }

        return new Arrangement(namedRects);
    }

    public class Arrangement : IEnumerable<RectangleF>
    {
        private readonly OneToMany<string, RectangleF> _namedRects;

        public Arrangement(OneToMany<string, RectangleF> namedRects)
        {
            _namedRects = namedRects;
        }

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
                Client.Debug.LogWarning($"Attempted to get element '{name}' but found {matchingElements.Count} matches.");
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
    }
}
