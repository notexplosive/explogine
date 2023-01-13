using System;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

public class PlatformAgnosticWindow : IWindow
{
    private WindowConfig _currentConfig;
    private MouseCursor? _pendingCursor;
    private Rectangle _rememberedBounds;
    private Point? _specifiedRenderResolution;
    protected GameWindow Window = null!;

    public PlatformAgnosticWindow()
    {
        ClientCanvas = new ClientCanvas(this);
        RenderResolutionChanged += ClientCanvas.ResizeCanvas;
    }

    /// <summary>
    ///     The Canvas that renders the actual game content to the screen.
    /// </summary>
    public ClientCanvas ClientCanvas { get; }

    public Canvas Canvas => ClientCanvas.Internal;

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
    public Matrix ScreenToCanvas => ClientCanvas.ScreenToCanvas;
    public Matrix CanvasToScreen => ClientCanvas.CanvasToScreen;

    public Point Size => Client.Headless
        ? new Point(1600, 900)
        : new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

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

    /// <summary>
    /// Passthrough to the OS
    /// </summary>
    public bool IsInFocus => Client.IsInFocus;
    
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

    public void SetCursor(MouseCursor cursor)
    {
        _pendingCursor = cursor;
    }

    /// <summary>
    ///     Run at the end of frame so we're only setting the cursor once per frame
    /// </summary>
    public void ResolveSetCursor()
    {
        if (_pendingCursor != null)
        {
            Mouse.SetCursor(_pendingCursor);
        }

        _pendingCursor = MouseCursor.Arrow;
    }
}
