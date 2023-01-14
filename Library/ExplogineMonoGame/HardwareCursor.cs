using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

public class HardwareCursor
{
    private MouseCursor? _pendingCursor;

    internal HardwareCursor()
    {
    }

    public void Set(MouseCursor cursor)
    {
        _pendingCursor = cursor;
    }

    /// <summary>
    ///     Run at the end of frame so we're only setting the cursor once per frame
    /// </summary>
    public void Resolve()
    {
        if (_pendingCursor != null)
        {
            Mouse.SetCursor(_pendingCursor);
        }

        _pendingCursor = MouseCursor.Arrow;
    }
}
