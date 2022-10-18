using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineDesktop;

public class DesktopWindow : PlatformAgnosticWindow
{
    protected override void LateSetup(WindowConfig config)
    {
        void OnResize(object? sender, EventArgs e)
        {
            var newWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            InvokeResized(newWindowSize);
        }

        Window.ClientSizeChanged += OnResize;
    }
}
