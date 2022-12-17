﻿using System;
using System.Collections;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Layout;

public class LayoutArrangement : IEnumerable<KeyValuePair<string, BakedLayoutElement>>
{
    private readonly OneToMany<string, BakedLayoutElement> _namedRects;

    public LayoutArrangement(OneToMany<string, BakedLayoutElement> namedRects, RectangleF usedSpace,
        LayoutElementGroup rawGroup)
    {
        RawGroup = rawGroup;
        _namedRects = namedRects;
        UsedSpace = usedSpace;
    }

    public LayoutElementGroup RawGroup { get; }
    public RectangleF UsedSpace { get; }

    public IEnumerator<KeyValuePair<string, BakedLayoutElement>> GetEnumerator()
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

    public List<BakedLayoutElement> FindElements(string name)
    {
        if (_namedRects.ContainsKey(name))
        {
            return _namedRects.Get(name);
        }

        return new List<BakedLayoutElement>();
    }

    public BakedLayoutElement FindElement(string name)
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

    public IEnumerable<BakedLayoutElement> AllElements()
    {
        foreach (var rect in _namedRects.Values)
        {
            yield return rect;
        }
    }
}
