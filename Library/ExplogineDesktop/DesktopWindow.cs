using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineDesktop;

public class DesktopWindow : RealWindow
{
    protected override void LateSetup(WindowConfig config)
    {
        void OnResize(object? sender, EventArgs e)
        {
            var newWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            InvokeResized(newWindowSize);
        }
        
        void OnTextEntered(object? sender, TextInputEventArgs e)
        {
            InvokeTextEntered(e.Character);
        }

        void OnFileDrop(object? sender, FileDropEventArgs e)
        {
            InvokeFileDrop(e.Files);
        }
        
        
        Window.ClientSizeChanged += OnResize;
        Window.TextInput += OnTextEntered;
        Window.FileDrop += OnFileDrop;
    }
}
