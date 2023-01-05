using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Data;

public interface IWindowSizeSettings
{
    Point StartingSize { get; }
}

public readonly record struct NonResizableWindowSizeSettings(Point StartingSize) : IWindowSizeSettings;

public readonly record struct ResizableWindowSizeSettings(Point StartingSize, Point MinimumSize, Point? MaximumSize = default,
    bool AllowFullScreen = false) : IWindowSizeSettings;