﻿using System.Linq;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class WindowManager : IUpdateHook, IUpdateInputHook, IDrawHook
{
    private readonly Rail _rail = new();
    private readonly RectangleF _desktopBoundingRect;
    private readonly SimpleGuiTheme _uiTheme;
    private readonly DeferredActions _deferredActions = new();
    private readonly int _depthPerWindow = 10;

    public WindowManager(RectangleF desktopBoundingRect, SimpleGuiTheme uiTheme)
    {
        _desktopBoundingRect = desktopBoundingRect;
        _uiTheme = uiTheme;
    }

    /// <summary>
    /// WindowManager manages its own SpriteBatch calls, so its starting depth does not matter
    /// </summary>
    public Depth BottomDepth => Depth.Middle;
    public Depth TopDepth => BottomDepth - _rail.Count * _depthPerWindow;

    public void Draw(Painter painter)
    {
        var windows = _rail.GetMatching<VirtualWindow>().ToArray();
        for (var i = 0; i < windows.Length; i++)
        {
            windows[i].StartingDepth = BottomDepth - _depthPerWindow * i;
        }

        painter.BeginSpriteBatch();
        foreach (var window in windows)
        {
            window.Draw(painter, _uiTheme);
        }

        painter.EndSpriteBatch();
    }

    public void Update(float dt)
    {
        _rail.Update(dt);
        _deferredActions.RunAllAndClear();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _rail.UpdateInput(input, hitTestStack);
    }

    public VirtualWindow AddWindow(Vector2 position, IWindowSizeSettings windowSizeSettings)
    {
        var window = new VirtualWindow(new RectangleF(position, windowSizeSettings.StartingSize.ToVector2()), windowSizeSettings, TopDepth);
        _rail.Add(window);
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
        }
        else
        {
            // Teardown
            window.RequestedFocus -= BringWindowToFrontDeferred;
            window.RequestedConstrainToBounds -= ConstrainWindowToBoundsDeferred;
        }
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
            _rail.Remove(targetWindow);
            _rail.Add(targetWindow);
        });
    }
}