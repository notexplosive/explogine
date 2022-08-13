﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NotCore.AssetManagement;

namespace NotCore;

public class Graphics
{
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    private readonly Stack<RenderTarget2D> _renderTargetStack = new();
    private RenderTarget2D? _currentRenderTarget;

    public Graphics(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice)
    {
        _graphicsDeviceManager = graphicsDeviceManager;
        Device = graphicsDevice;
        Painter = new Painter(graphicsDevice);

        Device.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};
    }

    public GraphicsDevice Device { get; }
    public Painter Painter { get; }
    public Point ScreenSize => Device.Viewport.Bounds.Size;

    public Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture)
    {
        if (rect.Width * rect.Height == 0)
        {
            throw new Exception("Can't crop a texture without any area");
        }

        var cropTexture = new Texture2D(Device, rect.Width, rect.Height);
        var data = new Color[rect.Width * rect.Height];
        sourceTexture.GetData(0, rect, data, 0, data.Length);
        cropTexture.SetData(data);
        return cropTexture;
    }

    public void PushCanvas(Canvas canvas)
    {
        if (_currentRenderTarget != null)
        {
            _renderTargetStack.Push(_currentRenderTarget);
        }

        _currentRenderTarget = canvas.RenderTarget;

        Device.SetRenderTarget(canvas.RenderTarget);
        // We must clear after setting a render target, otherwise it's black
        Device.Clear(Color.Transparent);
    }

    public void PopCanvas()
    {
        if (_renderTargetStack.Count > 0)
        {
            _currentRenderTarget = _renderTargetStack.Pop();
        }
        else
        {
            _currentRenderTarget = null;
        }

        Device.SetRenderTarget(_currentRenderTarget);
    }
}
