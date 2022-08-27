using System;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public abstract class AbstractWindow
{
    protected GameWindow _window = null!;
    private WindowConfig _currentConfig;
    protected Rectangle _rememberedBounds;

    public Point RenderResolution { get; protected set; }
    public event Action<Point>? Resized;
    public event Action? ConfigChanged;

    public string Title
    {
        get => _window.Title;
        set => _window.Title = value;
    }

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

    public bool IsFullscreen { get; private set; }
    public Point Size => new(_window.ClientBounds.Width, _window.ClientBounds.Height);

    public WindowConfig Config
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            // Always allow window resizing because MonoGame handles heterogeneous DPIs poorly
            // also just generally QoL. Window Resizing should always be legal.
            AllowResizing = true;

            ChangeRenderResolution(_currentConfig.WindowSize);

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

    protected void ChangeRenderResolution(Point windowSize)
    {
        // We use _currentConfig instead of Config because this is happening during Config._set
        if (_currentConfig.RenderResolution.HasValue)
        {
            RenderResolution = _currentConfig.RenderResolution.Value;
        }
        else
        {
            RenderResolution = windowSize;
        }
    }

    public void Setup(GameWindow window, WindowConfig config)
    {
        _window = window;
        _rememberedBounds = new Rectangle(Position, Size);

        LateSetup();
        
        Config = config;
    }

    protected abstract void LateSetup();

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

    public void SetSize(Point windowSize)
    {
        Client.Graphics.DeviceManager.PreferredBackBufferWidth = windowSize.X;
        Client.Graphics.DeviceManager.PreferredBackBufferHeight = windowSize.Y;
        Resized?.Invoke(windowSize);
        Client.Graphics.DeviceManager.ApplyChanges();
    }
    
    protected void InvokeResized(Point windowSize)
    {
        Resized?.Invoke(windowSize);
    }

    
}
