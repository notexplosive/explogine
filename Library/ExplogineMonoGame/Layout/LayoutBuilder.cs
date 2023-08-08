using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public class LayoutBuilder
{
    private readonly List<Node> _nodes = new();
    private readonly Style _style;

    public LayoutBuilder(Style style)
    {
        _style = style;
    }

    public void Clear()
    {
        _nodes.Clear();
    }

    public void Add(LayoutElement element)
    {
        if (element.Children.HasValue)
        {
            AddGroup(element.Children.Value.Style, element);
        }
        else
        {
            _nodes.Add(new Node(element.Name, element.X, element.Y, null));
        }
    }

    public LayoutBuilder AddGroup(Style style, LayoutElement parentElement)
    {
        var group = new LayoutBuilder(style);

        if (parentElement.Children.HasValue)
        {
            foreach (var child in parentElement.Children.Value.Elements)
            {
                group.Add(child);
            }
        }

        _nodes.Add(new Node(parentElement.Name, parentElement.X, parentElement.Y, group));

        return group;
    }

    public LayoutElementGroup ToLayoutGroup()
    {
        var elements = new List<LayoutElement>();

        foreach (var node in _nodes)
        {
            elements.Add(node.ToElement());
        }

        return L.Root(_style, elements.ToArray());
    }

    private class Node
    {
        private readonly LayoutBuilder? _children;
        private readonly IElementName _name;
        private readonly IEdgeSize _x;
        private readonly IEdgeSize _y;

        public Node(IElementName name, IEdgeSize x, IEdgeSize y, LayoutBuilder? children)
        {
            _name = name;
            _x = x;
            _y = y;
            _children = children;
        }

        public LayoutElement ToElement()
        {
            return new LayoutElement(_name, _x, _y, _children?.ToLayoutGroup());
        }
    }

    public LayoutArrangement Bake(Point outerSize)
    {
        return L.Compute(outerSize, ToLayoutGroup());
    }

    public LayoutArrangement Bake(RectangleF rectangle)
    {
        return L.Compute(rectangle, ToLayoutGroup());
    }
}
