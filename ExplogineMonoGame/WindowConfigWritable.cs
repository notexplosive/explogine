using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public struct WindowConfigWritable
{
    public bool Fullscreen { get; set; }
    public Point WindowSize { get; set; }
    public string Title { get; set; }
}

public readonly struct WindowConfig
{
    public WindowConfig(WindowConfigWritable configWritable)
    {
        WindowSize = configWritable.WindowSize;
        Title = configWritable.Title;
        Fullscreen = configWritable.Fullscreen;
    }

    public Point WindowSize { get; }
    public string Title { get; }
    public bool Fullscreen { get; }
}
