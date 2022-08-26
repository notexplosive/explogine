using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public interface IWindow
{
    public Point RenderResolution { get; }
    public string Title { get; set; }
    public Point Position { get; set; }
    bool AllowResizing { get; set; }
    bool IsFullscreen { get; }
    public Point Size { get; }
    public WindowConfig Config { get; set; }
    void Setup(GameWindow window, WindowConfig config);
    public void SetFullscreen(bool state);
    public event Action<Point> Resized;
    void SetSize(Point windowSize);
    public event Action? ConfigChanged;
}
