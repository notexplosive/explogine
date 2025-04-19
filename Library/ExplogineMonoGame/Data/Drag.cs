using System;
using Microsoft.Xna.Framework;

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

        Started?.Invoke();
    }

    public void End()
    {
        var wasDragging = IsDragging;

        IsDragging = false;
        StartingValue = default;
        TotalDelta = Vector2.Zero;

        if (wasDragging)
        {
            Finished?.Invoke();
        }
    }

    public void AddDelta(Vector2 delta)
    {
        if (IsDragging)
        {
            TotalDelta += delta;
        }
    }

    public event Action? Finished;
    public event Action? Started;
}
