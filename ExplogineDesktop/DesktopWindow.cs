using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineDesktop;

public class DesktopWindow : AbstractWindow
{
    protected override void LateSetup(WindowConfig config)
    {
        void OnResize(object? sender, EventArgs e)
        {
            var windowSize = new Point(_window.ClientBounds.Width, _window.ClientBounds.Height);
            ChangeRenderResolution(windowSize, config.RenderResolution);
            InvokeResized(windowSize);
        }

        _window.ClientSizeChanged += OnResize;
    }
}
