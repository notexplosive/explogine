using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Gui;

/// <summary>
/// Shared code between Buttons and Button-likes (checkbox, radial, etc)
/// </summary>
public class ButtonBehavior
{
    public bool UpdateInputAndReturnClicked(ConsumableInput input, HitTestStack hitTestStack, RectangleF rectangle, Depth depth)
    {
        var result = false;
        hitTestStack.AddZone(rectangle, depth, ClearHovered, BecomeHovered);

        var wasMouseReleased = input.Mouse.GetButton(MouseButton.Left).WasReleased;
        var wasMousePressed = input.Mouse.GetButton(MouseButton.Left).WasPressed;

        if (IsHovered)
        {
            if (IsEngaged && wasMouseReleased)
            {
                result = true;
            }

            if (wasMousePressed)
            {
                IsEngaged = true;
            }
            Client.Cursor.Set(MouseCursor.Hand);
        }

        if (wasMouseReleased)
        {
            IsEngaged = false;
        }

        return result;
    }
    
    private void BecomeHovered()
    {
        IsHovered = true;
    }

    private void ClearHovered()
    {
        IsHovered = false;
    }
    
    public bool IsHovered { get; private set; }
    public bool IsEngaged { get; private set; }
}
