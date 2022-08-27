using System;
using System.Collections.Generic;
using ExplogineMonoGame.AssetManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame;

public class Graphics
{
    public GraphicsDeviceManager DeviceManager { get; }
    private readonly Stack<RenderTarget2D> _renderTargetStack = new();
    private RenderTarget2D? _currentRenderTarget;

    public Graphics(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice)
    {
        DeviceManager = graphicsDeviceManager;
        Device = graphicsDevice;
        Painter = new Painter(graphicsDevice);

        Device.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};
    }

    public GraphicsDevice Device { get; }
    public Painter Painter { get; }
    public Point DisplaySize => new(Client.Graphics.Device.DisplayMode.Width, Client.Graphics.Device.DisplayMode.Height);

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

    public bool IsAtTopCanvas()
    {
        return _currentRenderTarget == null;
    }
}
