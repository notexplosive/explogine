using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class WindowManager : IUpdateHook, IUpdateInputHook, IDrawHook
{
    private readonly DeferredActions _deferredActions = new();
    private readonly int _depthPerWindow = 10;
    private readonly RectangleF _desktopBoundingRect;
    private readonly SimpleGuiTheme _uiTheme;
    private readonly List<VirtualWindow> _windows = new();
    private readonly Dictionary<VirtualWindow, WindowState> _windowStates = new();

    public WindowManager(RectangleF desktopBoundingRect, SimpleGuiTheme uiTheme)
    {
        _desktopBoundingRect = desktopBoundingRect;
        _uiTheme = uiTheme;
    }

    /// <summary>
    ///     WindowManager manages its own SpriteBatch calls, so its starting depth does not matter
    /// </summary>
    public Depth BottomDepth => Depth.Middle;

    public Depth TopDepth => BottomDepth - _windows.Count * _depthPerWindow;

    public void Draw(Painter painter)
    {
        // Put all the windows at the proper relative depth
        for (var i = 0; i < _windows.Count; i++)
        {
            _windows[i].StartingDepth = BottomDepth - _depthPerWindow * i;
        }

        // Prepare the contents of the windows
        foreach (var window in _windows)
        {
            Client.Graphics.PushCanvas(window.Canvas);
            painter.Clear(_uiTheme.BackgroundColor);
            window.DrawContent(painter);
            Client.Graphics.PopCanvas();
        }

        // Draw the windows to the screen
        painter.BeginSpriteBatch();
        for (var i = 0; i < _windows.Count; i++)
        {
            var window = _windows[i];

            if (!_windowStates[window].IsMinimized)
            {
                window.Draw(painter, _uiTheme, i == _windows.Count - 1);
            }
        }

        painter.EndSpriteBatch();
    }

    public void Update(float dt)
    {
        _deferredActions.RunAllAndClear();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        for (var i = _windows.Count - 1; i >= 0; i--)
        {
            var window = _windows[i];
            if (!_windowStates[window].IsMinimized)
            {
                window.UpdateInput(input, hitTestStack);
            }
        }
    }

    public VirtualWindow AddWindow(Vector2 position, VirtualWindow.Settings settings, IWindowContent content)
    {
        var window = new VirtualWindow(new RectangleF(position, settings.SizeSettings.StartingSize.ToVector2()),
            settings, content, TopDepth);
        _windowStates.Add(window, new WindowState());
        _windows.Add(window);
        SetupOrTeardown(window);
        return window;
    }

    private void SetupOrTeardown(VirtualWindow window, bool isSetup = true)
    {
        if (isSetup)
        {
            // Setup
            window.RequestedFocus += BringWindowToFrontDeferred;
            window.RequestedConstrainToBounds += ConstrainWindowToBoundsDeferred;
            window.RequestedClose += CloseWindowDeferred;
            window.RequestedMinimize += MinimizeWindow;
        }
        else
        {
            // Teardown
            window.RequestedFocus -= BringWindowToFrontDeferred;
            window.RequestedConstrainToBounds -= ConstrainWindowToBoundsDeferred;
            window.RequestedClose -= CloseWindowDeferred;
            window.RequestedMinimize -= MinimizeWindow;
        }
    }

    private void CloseWindowDeferred(VirtualWindow window)
    {
        _deferredActions.Add(() =>
        {
            _windows.Remove(window);
            _windowStates.Remove(window);
        });
    }

    public void MinimizeWindow(VirtualWindow window)
    {
        _windowStates[window].IsMinimized = true;
    }

    private void ConstrainWindowToBoundsDeferred(VirtualWindow window)
    {
        // This needs to be deferred because we might set the position later that frame
        _deferredActions.Add(() =>
        {
            var constrainedRect = window.TitleBarRectangle.ConstrainedTo(_desktopBoundingRect);
            window.Position = constrainedRect.Location;
        });
    }

    private void BringWindowToFrontDeferred(VirtualWindow targetWindow)
    {
        _deferredActions.Add(() =>
        {
            _windows.Remove(targetWindow);
            _windows.Add(targetWindow);
        });
    }

    /// <summary>
    /// Stores per-window state that doesn't make sense to live on the Window itself because they're only relevant to the Window Manager.
    /// </summary>
    private class WindowState
    {
        public bool IsMinimized { get; set; }
    }
}
