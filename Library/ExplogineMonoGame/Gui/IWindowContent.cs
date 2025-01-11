using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui.Window;

namespace ExplogineMonoGame.Gui;

/// <summary>
///     This deliberately doesn't use RailHooks because they'll get called at the wrong times
/// </summary>
public interface IWindowContent
{
    /// <summary>
    ///     Called when the window is launched
    /// </summary>
    /// <param name="parentWindow"></param>
    void Initialize(InternalWindow parentWindow);

    /// <summary>
    ///     Called when the window is closed
    /// </summary>
    void TearDown();

    /// <summary>
    ///     Behaves like UpdateInput(), although if the window is not in focus it will be given an empty Keyboard component
    /// </summary>
    /// <param name="input"></param>
    /// <param name="hitTestStack"></param>
    void UpdateInputInWindow(ConsumableInput input, HitTestStack hitTestStack);

    /// <summary>
    ///     Behaves like Draw(), canvas is already setup, but spritebatch is not
    /// </summary>
    /// <param name="painter"></param>
    void DrawWindowContent(Painter painter);
}
