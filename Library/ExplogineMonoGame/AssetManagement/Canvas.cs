using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public sealed class Canvas : IDisposable
{
    private RenderTarget2D? _renderTarget;

    private Point _size;

    public Canvas(Point size) : this(size.X, size.Y)
    {
        // See other constructor
    }

    public Canvas(int width, int height)
    {
        Size = new Point(width, height);
        _renderTarget = !Client.Headless ? CreateRenderTarget() : null;
    }

    internal RenderTarget2D RenderTarget => _renderTarget ??= CreateRenderTarget();
    public Texture2D Texture => RenderTarget;

    public Point Size
    {
        get => _size;
        set
        {
            if (_size == value)
            {
                return;
            }

            _renderTarget?.Dispose();
            _renderTarget = null;
            _size = value;
        }
    }

    public void Dispose()
    {
        _renderTarget?.Dispose();
    }

    private RenderTarget2D CreateRenderTarget()
    {
        if (Size.X < 1 || Size.Y < 1)
        {
            Size = new Point(Math.Max(Size.X, 1), Math.Max(Size.Y, 1));
        }

        return new RenderTarget2D(
            Client.Graphics.Device,
            Size.X,
            Size.Y,
            false,
            Client.Graphics.Device.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24);
    }

    public TextureAsset AsTextureAsset()
    {
        return new TextureAsset(Texture);
    }
}
