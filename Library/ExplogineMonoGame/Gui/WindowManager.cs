using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Gui;

public class WindowManager : IUpdateHook, IUpdateInputHook, IDrawHook
{
    private readonly SimpleGuiTheme _uiTheme;
    private readonly Rail _rail = new();

    public WindowManager(SimpleGuiTheme uiTheme)
    {
        _uiTheme = uiTheme;
    }

    public void Draw(Painter painter)
    {
        foreach (var window in _rail.GetMatching<VirtualWindow>())
        {
            
        }
        
        painter.BeginSpriteBatch();
        foreach (var window in _rail.GetMatching<VirtualWindow>())
        {
            window.Draw(painter, _uiTheme);
        }
        painter.EndSpriteBatch();
    }

    public void Update(float dt)
    {
        _rail.Update(dt);
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _rail.UpdateInput(input, hitTestStack);
    }

    public VirtualWindow AddWindow(RectangleF rectangle)
    {
        var window = new VirtualWindow(rectangle, TopDepth);
        _rail.Add(window);
        return window;
    }

    public Depth TopDepth
    {
        get => Depth.Middle - _rail.Count * 10;
    }
}
