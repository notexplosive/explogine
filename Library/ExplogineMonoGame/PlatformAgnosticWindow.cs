using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

public class PlatformAgnosticWindow
{
    private WindowConfig _currentConfig;
    private Rectangle _rememberedBounds;
    private Point? _specifiedRenderResolution;
    protected GameWindow Window = null!;

    public Point RenderResolution => _specifiedRenderResolution ?? Size;

    public string Title
    {
        get => Window.Title;
        set => Window.Title = value;
    }

    public Point Position
    {
        get => Window.Position;
        set => Window.Position = value;
    }

    public bool AllowResizing
    {
        get => Window.AllowUserResizing;
        set => Window.AllowUserResizing = value;
    }

    public bool IsFullscreen { get; private set; }

    public Point Size => Client.Headless
        ? new Point(1600, 900)
        : new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

    public WindowConfig Config
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            // Always allow window resizing because MonoGame handles heterogeneous DPIs poorly
            // also just generally QoL. Window Resizing should always be legal.
            AllowResizing = true;

            SetSize(Config.WindowSize);

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

            Client.Graphics.DeviceManager.ApplyChanges();
            ConfigChanged?.Invoke();
        }
    }

    public TextEnteredBuffer TextEnteredBuffer { get; set; }

    public void SetRenderResolution(Point? optionalSize)
    {
        if (optionalSize.HasValue)
        {
            _specifiedRenderResolution = optionalSize.Value;
            RenderResolutionChanged?.Invoke(optionalSize.Value);
        }
        else
        {
            _specifiedRenderResolution = null;
            RenderResolutionChanged?.Invoke(Size);
        }
    }

    public event Action<Point>? Resized;
    public event Action<Point>? RenderResolutionChanged;
    public event Action? ConfigChanged;

    public void Setup(GameWindow window, WindowConfig config)
    {
        Window = window;
        _rememberedBounds = new Rectangle(Position, Size);
        LateSetup(config);

        Config = config;
    }

    protected virtual void LateSetup(WindowConfig config)
    {
    }

    public void SetFullscreen(bool state)
    {
        if (IsFullscreen == state)
        {
            return;
        }

        if (state)
        {
            _rememberedBounds = new Rectangle(Position, Size);
            SetSize(Client.Graphics.DisplaySize);
            Window.IsBorderless = true;
            Window.Position = Point.Zero;
        }
        else
        {
            Window.Position = _rememberedBounds.Location;
            SetSize(_rememberedBounds.Size);
            Window.IsBorderless = false;
        }

        Client.Graphics.DeviceManager.ApplyChanges();
        IsFullscreen = state;
    }

    public void SetSize(Point windowSize)
    {
        Client.Graphics.DeviceManager.PreferredBackBufferWidth = windowSize.X;
        Client.Graphics.DeviceManager.PreferredBackBufferHeight = windowSize.Y;

        InvokeResized(windowSize);
        Client.Graphics.DeviceManager.ApplyChanges();
    }

    protected void InvokeTextEntered(char text)
    {
        TextEnteredBuffer = TextEnteredBuffer.WithAddedCharacter(text);
    }

    protected void InvokeResized(Point windowSize)
    {
        if (!_specifiedRenderResolution.HasValue)
        {
            RenderResolutionChanged?.Invoke(windowSize);
        }

        Resized?.Invoke(windowSize);
    }

    public void SetCursor(MouseCursor cursor)
    {
        Mouse.SetCursor(cursor);
    }
}