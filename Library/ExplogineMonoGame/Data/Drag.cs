using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public class Drag
{
    public bool IsDragging { get; private set; }
    public Vector2 TotalDelta { get; private set; }
    public Vector2 StartPosition { get; private set; }

    public void Start(Vector2 startPosition)
    {
        IsDragging = true;
        StartPosition = startPosition;
    }

    public void End()
    {
        IsDragging = false;
        StartPosition = Vector2.Zero;
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
