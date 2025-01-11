using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

public class RealWindow : IWindow
{
    private WindowConfig _currentConfig;
    private Rectangle _rememberedBounds;
    private Point? _specifiedRenderResolution;
    protected GameWindow Window = null!;

    public RealWindow()
    {
        ClientCanvas = new ClientCanvas(this);
        RenderResolutionChanged += ClientCanvas.ResizeCanvas;
    }

    /// <summary>
    ///     The Canvas that renders the actual game content to the screen.
    /// </summary>
    public ClientCanvas ClientCanvas { get; }

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

    private WindowConfig Config
    {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            // Always allow window resizing because MonoGame handles heterogeneous DPIs poorly
            // also just generally QoL. Window Resizing should always be legal.
            AllowResizing = true;

            SetSize(value.WindowSize);

            if (value.Fullscreen)
            {
                SetFullscreen(true);
            }
            else
            {
                SetFullscreen(false);
                SetSize(value.WindowSize);
            }

            Title = value.Title;

            Client.Graphics.DeviceManager.ApplyChanges();
            ConfigChanged?.Invoke();
        }
    }

    public TextEnteredBuffer TextEnteredBuffer { get; set; }

    public Canvas Canvas => ClientCanvas.Internal;

    public Point RenderResolution => _specifiedRenderResolution ?? Size;

    public bool IsFullscreen { get; private set; }
    public Matrix ScreenToCanvas => ClientCanvas.ScreenToCanvas;
    public Matrix CanvasToScreen => ClientCanvas.CanvasToScreen;

    public Point Size
    {
        get =>
            Client.Headless
                ? new Point(1600, 900)
                : new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
        set => SetSize(value);
    }

    /// <summary>
    ///     Passthrough to the OS
    /// </summary>
    public bool IsInFocus => Client.IsInFocus;

    public void SetRenderResolution(CartridgeConfig config)
    {
        var renderResolution = config.RenderResolution;
        Client.Graphics.SamplerState = config.SamplerState ?? Client.Graphics.SamplerState;
        if (renderResolution.HasValue)
        {
            _specifiedRenderResolution = renderResolution.Value;
            RenderResolutionChanged?.Invoke(renderResolution.Value);
        }
        else
        {
            _specifiedRenderResolution = null;
            RenderResolutionChanged?.Invoke(Size);
        }
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

    public event Action<Point>? Resized;
    public event Action<Point>? RenderResolutionChanged;
    public event Action<string>? FileDropped;
    public event Action? ConfigChanged;

    public void Setup(GameWindow window, WindowConfig config)
    {
        ClientCanvas.Setup();
        Window = window;
        _rememberedBounds = new Rectangle(Position, Size);
        LateSetup(config);

        Config = config;
    }

    protected virtual void LateSetup(WindowConfig config)
    {
        // Intentionally left blank, would be abstract except we want to be able to instantiate this type
    }

    public void SetSize(Point windowSize)
    {
        Client.Graphics.DeviceManager.PreferredBackBufferWidth = windowSize.X;
        Client.Graphics.DeviceManager.PreferredBackBufferHeight = windowSize.Y;

        InvokeResized(windowSize);
        Client.Graphics.DeviceManager.ApplyChanges();
    }

    protected void InvokeFileDrop(string[] files)
    {
        foreach (var file in files)
        {
            FileDropped?.Invoke(file);
        }
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
}
