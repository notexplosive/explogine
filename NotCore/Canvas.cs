using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NotCore;

public sealed class Canvas : IDisposable
{
    public Canvas(int width, int height)
    {
        RenderTarget = new RenderTarget2D(
            Client.Graphics.Device,
            width,
            height,
            false,
            Client.Graphics.Device.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    internal RenderTarget2D RenderTarget { get; }
    public Texture2D Texture => RenderTarget;

    public void Dispose()
    {
        RenderTarget.Dispose();
    }

    public DynamicAsset AsAsset(string key)
    {
        return new DynamicAsset(key, this);
    }
}