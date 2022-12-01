﻿using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class Drag<T> where T : new()
{
    public bool IsDragging { get; private set; }
    public Vector2 TotalDelta { get; private set; }
    public T? StartingValue { get; private set; }

    public void Start(T startingValue)
    {
        IsDragging = true;
        StartingValue = startingValue;
    }

    public void End()
    {
        IsDragging = false;
        StartingValue = default;
        TotalDelta = Vector2.Zero;
    }

    public void AddDelta(Vector2 delta)
    {
        if (IsDragging)
        {
            TotalDelta += delta;
        }
    }
}