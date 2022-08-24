using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineDesktop;

public class DesktopWindow : IWindow
{
    private static WindowConfig _currentConfig;
    private Rectangle _rememberedBounds;
    private GameWindow _window = null!;

    public Point Position
    {
        get => _window.Position;
        set => _window.Position = value;
    }

    public bool AllowResizing
    {
        get => _window.AllowUserResizing;
        set => _window.AllowUserResizing = value;
    }

    public event Action<Point>? Resized;

    public void SetSize(Point windowSize)
    {
        Client.Graphics.DeviceManager.PreferredBackBufferWidth = windowSize.X;
        Client.Graphics.DeviceManager.PreferredBackBufferHeight = windowSize.Y;
        Client.Graphics.DeviceManager.ApplyChanges();
    }

    public void Setup(GameWindow window, WindowConfig config)
    {
        void OnResize(object? sender, EventArgs e)
        {
            var windowSize = new Point(_window.ClientBounds.Width, _window.ClientBounds.Height);
            Resized?.Invoke(windowSize);
        }

        _window = window;
        _window.ClientSizeChanged += OnResize;
        _rememberedBounds = new Rectangle(Position, Size);

        Config = config;
    }

    public string Title
    {
        get => _window.Title;
        set => _window.Title = value;
    }

    public bool IsFullscreen { get; private set; }

    public Point Size => new(_window.ClientBounds.Width, _window.ClientBounds.Height);

    public void SetFullscreen(bool state)
    {
        if (IsFullscreen == state)
        {
            return;
        }
        
        if (state)
        {
            _rememberedBounds = new Rectangle(Position, Size);
            SetSize(Client.Graphics.ScreenSize);
            _window.IsBorderless = true;
            _window.Position = Point.Zero;
        }
        else
        {
            _window.Position = _rememberedBounds.Location;
            SetSize(_rememberedBounds.Size);
            _window.IsBorderless = false;
        }

        Client.Graphics.DeviceManager.ApplyChanges();
        IsFullscreen = state;
    }

    public WindowConfig Config
    {
        get => DesktopWindow._currentConfig;
        set
        {
            DesktopWindow._currentConfig = value;
            // Always allow window resizing because MonoGame handles heterogeneous DPIs poorly
            // also just generally QoL. Window Resizing should always be legal.
            AllowResizing = true;

            if (Config.Fullscreen)
            {
                SetFullscreen(true);
            }
            else
            {
                SetFullscreen(false);
                SetSize(Config.WindowSize);                
            }

            Title = Config.Title;
        }
    }
}
