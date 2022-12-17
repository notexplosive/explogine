using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Gui;

public class ImmediateGui : IUpdateInput
{
    private readonly List<IGuiWidget> _elements = new();

    public void Button(RectangleF rectangle, string text, Action? onPress, Depth depth)
    {
        _elements.Add(new Button(rectangle, text, onPress, depth));
    }

    public void Draw(Painter painter, IGuiTheme uiTheme)
    {
        foreach (var element in _elements)
        {
            switch (element)
            {
                case Button buttonElement:
                    uiTheme.DrawButton(painter, buttonElement);
                    break;
            }
        }
    }

    public void UpdateInput(InputFrameState input, HitTestStack hitTestStack)
    {
        foreach (var element in _elements)
        {
            element.UpdateInput(input, hitTestStack);
        }
    }
}
