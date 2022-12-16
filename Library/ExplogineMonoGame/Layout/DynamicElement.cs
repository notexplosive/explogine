using System;
using ExplogineCore.Data;

namespace ExplogineMonoGame.Layout;

internal readonly record struct DynamicElement : IElement
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
