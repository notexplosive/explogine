using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public class ImmediateGui : IUpdateInput
{
    private readonly List<IGuiWidget> _widgets = new();

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        foreach (var element in _widgets)
        {
            element.UpdateInput(input, hitTestStack);
        }
    }

    public void Button(RectangleF rectangle, string label, Depth depth, Action? onPress)
    {
        _widgets.Add(new Button(rectangle, label, onPress, depth));
    }

    public Wrapped<bool> Checkbox(RectangleF totalRectangle, string label, Depth depth)
    {
        var state = new Wrapped<bool>();
        _widgets.Add(new Checkbox(totalRectangle, label, depth, state));
        return state;
    }

    public ImmediateGui Panel(RectangleF rectangle, Depth depth)
    {
        var panel = new Panel(rectangle, depth);
        _widgets.Add(panel);
        return panel.InnerGui;
    }

    public void Draw(Painter painter, IGuiTheme uiTheme, Matrix matrix)
    {
        PreDraw(painter, uiTheme);

        painter.BeginSpriteBatch(matrix);
        foreach (var widget in _widgets)
        {
            switch (widget)
            {
                case Button button:
                    uiTheme.DrawButton(painter, button);
                    break;
                case Panel panel:
                    uiTheme.DrawPanel(painter, panel);
                    break;
                case Checkbox checkbox:
                    uiTheme.DrawCheckbox(painter, checkbox);
                    break;
                default:
                    throw new Exception($"Unknown UI Widget type: {widget}");
            }
        }

        painter.EndSpriteBatch();
    }

    private void PreDraw(Painter painter, IGuiTheme uiTheme)
    {
        foreach (var widget in _widgets)
        {
            if (widget is IPreDrawWidget preDrawer)
            {
                preDrawer.PreDraw(painter, uiTheme);
            }
        }
    }
}
