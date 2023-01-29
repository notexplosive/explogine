using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui.Window;

public class WindowManager : IUpdateHook, IUpdateInputHook, IDrawHook, IEarlyDrawHook
{
    private readonly DeferredActions _deferredActions = new();
    private readonly int _depthPerWindow = 10;
    private readonly RectangleF _desktopBoundingRect;
    private readonly IGuiTheme _uiTheme;
    private readonly IRuntime _parentRuntime;
    private readonly List<InternalWindow> _windows = new();
    private readonly Dictionary<InternalWindow, WindowState> _windowStates = new();

    public WindowManager(RectangleF desktopBoundingRect, IGuiTheme uiTheme, IRuntime parentRuntime)
    {
        _desktopBoundingRect = desktopBoundingRect;
        _uiTheme = uiTheme;
        _parentRuntime = parentRuntime;
    }

    /// <summary>
    ///     WindowManager manages its own SpriteBatch calls, so its starting depth does not matter
    /// </summary>
    public Depth BottomDepth => Depth.Middle;

    public Depth TopDepth => BottomDepth - _windows.Count * _depthPerWindow;

    public void Draw(Painter painter)
    {
        // !!!
        // NOTE!! This used to call BeginSpriteBatch but doesn't anymore, this is now the caller's responsability
        // !!!
        
        for (var i = 0; i < _windows.Count; i++)
        {
            var window = _windows[i];

            if (!_windowStates[window].IsMinimized)
            {
                window.Draw(painter, _uiTheme, i == _windows.Count - 1);
            }
        }
    }

    public void EarlyDraw(Painter painter)
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
    }

    public void Update(float dt)
    {
        _deferredActions.RunAllAndClear();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var inputWithoutKeyboard = input.WithoutKeyboard();
        for (var i = _windows.Count - 1; i >= 0; i--)
        {
            var isTopWindow = i == _windows.Count - 1;
            var window = _windows[i];

            if (!_windowStates[window].IsMinimized)
            {
                if (isTopWindow)
                {
                    window.UpdateInput(input, hitTestStack);
                }
                else
                {
                    window.UpdateInput(inputWithoutKeyboard, hitTestStack);
                }

            }
        }
    }

    public event InternalWindow.WindowEvent? ClosedWindow;
    public event InternalWindow.WindowEvent? CreatedWindow;
    public event InternalWindow.WindowEvent? MinimizedWindow;
    public event InternalWindow.WindowEvent? UnMinimizedWindow;

    public InternalWindow AddWindow(Vector2 position, InternalWindow.Settings settings, IWindowContent content)
    {
        var window = new InternalWindow(new RectangleF(position, settings.SizeSettings.StartingSize.ToVector2()),
            settings, content, TopDepth, _parentRuntime);
        _windowStates.Add(window, new WindowState());
        _windows.Add(window);
        SetupOrTeardown(window);

        CreatedWindow?.Invoke(window);
        return window;
    }

    private void SetupOrTeardown(InternalWindow window, bool isSetup = true)
    {
        if (isSetup)
        {
            // Setup
            window.RequestedFocus += BringWindowToFrontDeferred;
            window.RequestedConstrainToBounds += ConstrainWindowToBoundsDeferred;
            window.RequestedClose += CloseWindowDeferred;
            window.RequestedMinimize += MinimizeWindow;
            window.RequestedFullScreen += FullScreenWindow;
        }
        else
        {
            // Teardown
            window.RequestedFocus -= BringWindowToFrontDeferred;
            window.RequestedConstrainToBounds -= ConstrainWindowToBoundsDeferred;
            window.RequestedClose -= CloseWindowDeferred;
            window.RequestedMinimize -= MinimizeWindow;
            window.RequestedFullScreen -= FullScreenWindow;
        }
    }

    private void FullScreenWindow(InternalWindow window)
    {
        _windowStates[window].ToggleFullScreen(window, _desktopBoundingRect);
    }

    private void CloseWindowDeferred(InternalWindow window)
    {
        _deferredActions.Add(() =>
        {
            _windows.Remove(window);
            _windowStates.Remove(window);
            ClosedWindow?.Invoke(window);
            window.Dispose();
        });
    }

    public void MinimizeWindow(InternalWindow window)
    {
        _windowStates[window].IsMinimized = true;
        MinimizedWindow?.Invoke(window);
    }

    public void UnMinimizeWindow(InternalWindow window)
    {
        _windowStates[window].IsMinimized = false;
        UnMinimizedWindow?.Invoke(window);
    }

    private void ConstrainWindowToBoundsDeferred(InternalWindow window)
    {
        // This needs to be deferred because we might set the position later that frame
        _deferredActions.Add(() =>
        {
            var constrainedRect = window.TitleBarRectangle.ConstrainedTo(_desktopBoundingRect);
            window.Position = constrainedRect.Location;
        });
    }

    private void BringWindowToFrontDeferred(InternalWindow targetWindow)
    {
        _deferredActions.Add(() =>
        {
            _windows.Remove(targetWindow);
            _windows.Add(targetWindow);
        });
    }

    /// <summary>
    ///     Stores per-window state that doesn't make sense to live on the Window itself because they're only relevant to the
    ///     Window Manager.
    /// </summary>
    private class WindowState
    {
        private RectangleF _rectangleBeforeFullScreen;
        public bool IsMinimized { get; set; }
        public bool IsFullScreen { get; set; }

        public void ToggleFullScreen(InternalWindow window, RectangleF screenRect)
        {
            if (!IsFullScreen)
            {
                _rectangleBeforeFullScreen = window.WholeRectangle;
                window.WholeRectangle = screenRect;
            }
            else
            {
                window.WholeRectangle = _rectangleBeforeFullScreen;
            }
            
            IsFullScreen = !IsFullScreen;
        }
    }
}
