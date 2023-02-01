using System;
using ExplogineCore.Data;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui.Window;

public class InternalWindow : IUpdateInputHook, IDisposable
{
    public delegate void WindowEvent(InternalWindow window);

    private readonly InternalWindowBody _body;
    private readonly InternalWindowChrome _chrome;

    public InternalWindow(RectangleF rectangle, Settings settings, IWindowContent content,
        Depth depth, IRuntime parentRuntime)
    {
        CurrentSettings = settings;
        Widget = new WindowWidget(rectangle, depth - 1);
        _chrome = new InternalWindowChrome(this, 32, settings.SizeSettings);
        _body = new InternalWindowBody(this, content);

        ParentRuntime = parentRuntime;
        Title = CurrentSettings.Title;
    }

    public WindowWidget Widget { get; }

    public Settings CurrentSettings { get; }
    public Canvas Canvas => Widget.Canvas;
    public RectangleF CanvasOutputRectangle => Widget.OutputRectangle;

    public RectangleF WholeRectangle
    {
        get => _chrome.WholeWindowRectangle;
        set => _chrome.WholeWindowRectangle = value;
    }

    public RectangleF TitleBarRectangle => _chrome.TitleBarRectangle;

    public Depth StartingDepth
    {
        // off-by-one because we want the widget to be the source of truth, but we also want to draw chrome below the widget
        get => Widget.Depth + 1;
        set => Widget.Depth = value - 1;
    }

    public Vector2 Position
    {
        get => WholeRectangle.Location;
        set => Widget.Position = value + new Vector2(0, TitleBarRectangle.Height);
    }

    public ImageAsset? Icon => CurrentSettings.Icon;
    public string Title { get; }
    public IRuntime ParentRuntime { get; }

    public void Dispose()
    {
        Widget.Dispose();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        Widget.UpdateHovered(hitTestStack);
        _chrome.UpdateInput(input, hitTestStack);
        _body.UpdateInput(input, hitTestStack);
    }

    public event WindowEvent? RequestedFocus;
    public event WindowEvent? RequestedClose;
    public event WindowEvent? RequestedMinimize;
    public event WindowEvent? RequestedFullScreen;
    public event WindowEvent? RequestedConstrainToBounds;

    public void Draw(Painter painter, IGuiTheme theme, bool isInFocus)
    {
        _chrome.Draw(painter, theme, isInFocus);
        Widget.Draw(painter);
    }

    public void DrawContent(Painter painter)
    {
        _body.Content.DrawWindowContent(painter);
    }

    public void SetRectangle(RectangleF resizedRect)
    {
        Widget.OutputRectangle = resizedRect;
    }

    public void RequestClose()
    {
        RequestedClose?.Invoke(this);
    }

    public void RequestMinimize()
    {
        RequestedMinimize?.Invoke(this);
    }

    public void RequestFullScreen()
    {
        RequestedFullScreen?.Invoke(this);
    }

    public void RequestFocus()
    {
        RequestedFocus?.Invoke(this);
    }

    public void ValidateBounds()
    {
        RequestedConstrainToBounds?.Invoke(this);
    }

    public record Settings(string Title, ISizeSettings SizeSettings, ImageAsset? Icon = null,
        bool AllowMinimize = false, bool AllowClose = true);

    public interface ISizeSettings
    {
        Point StartingSize { get; }
        bool AllowFullScreen { get; }
    }

    public readonly record struct NonResizableSizeSettings(Point StartingSize) : ISizeSettings
    {
        public bool AllowFullScreen => false;

        public static NonResizableSizeSettings Create(Point startingSize)
        {
            return new NonResizableSizeSettings(startingSize);
        }
    }

    public readonly record struct ResizableSizeSettings(Point StartingSize, Point MinimumSize,
        bool AllowFullScreen = false) : ISizeSettings
    {
        public static ResizableSizeSettings Create(Point startingSize, Point? minimumSize = null,
            bool allowFullScreen = true)
        {
            return new ResizableSizeSettings(startingSize, minimumSize ?? new Point(200, 200), allowFullScreen);
        }
    }
}
