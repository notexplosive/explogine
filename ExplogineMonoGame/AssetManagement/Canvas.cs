using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public sealed class Canvas : IDisposable
{
    public Canvas(Point size) : this(size.X, size.Y)
    {
        // See other constructor
    }

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
    public Point Size => new(RenderTarget.Width, RenderTarget.Height);

    public void Dispose()
    {
        RenderTarget.Dispose();
    }

    public TextureAsset AsAsset(string key)
    {
        return new TextureAsset(Texture);
    }
}
