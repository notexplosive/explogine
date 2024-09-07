using System.Collections.Generic;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Layout;
using ExplogineMonoGame.Luigi.Description;
using JetBrains.Annotations;
using MoonSharp.Interpreter;

namespace ExplogineMonoGame.Luigi;

/// <summary>
///     The "layout" object passed to lua scripts
/// </summary>
public class LuaLayoutBuilder
{
    private readonly IGuiTheme _rootTheme;
    private readonly Dictionary<string, IGuiTheme> _themes;
    private readonly Stack<LuaLayoutBuilderBlock> _groupStack = new();
    private readonly LuaLayoutBuilderBlock _rootBuilder;
    private int _idPool;

    public LuaLayoutBuilder(Style rootStyle, IGuiTheme rootTheme, Dictionary<string, IGuiTheme> themes)
    {
        _rootTheme = rootTheme;
        _themes = themes;
        _rootBuilder = new LuaLayoutBuilderBlock(new LayoutBuilder(rootStyle), _rootTheme);
    }

    private LuaLayoutBuilderBlock CurrentGroup()
    {
        return _groupStack.Count == 0 ? _rootBuilder : _groupStack.Peek();
    }

    [UsedImplicitly]
    [LuaMember("add")]
    public LuaLayoutElement Add(LayoutElement element)
    {
        var elementName = AddElementToCurrentGroup(element);
        return new LuaLayoutElement(CurrentGroup(), elementName);
    }

    [MoonSharpHidden]
    private IGuiTheme GetStyleFromName(string? themeName)
    {
        if (themeName != null && _themes.TryGetValue(themeName, out var theme))
        {
            return theme;
        }
        return _rootTheme;
    }
    
    [UsedImplicitly]
    [LuaMember("beginGroup")]
    public LuaLayoutBuilderBlock BeginGroup(LayoutElement element, string? styleName = null)
    {
        var group = CurrentGroup().AddGroupWithSameInstructions(new Style(), element, GetStyleFromName(styleName));
        _groupStack.Push(group);
        return group;
    }

    [UsedImplicitly]
    [LuaMember("beginScrollableGroup")]
    public LuaLayoutBuilderBlock BeginScrollableGroup(LayoutElement element, string? scrollPositionId, string? styleName = null)
    {
        // The pane of the scrollable area in the parent group
        var elementId = AddElementToCurrentGroup(element);

        // The content of the scrollable area, it's not a "child" of the main layout, it exists in a vacuum
        var childGroup = new LuaLayoutBuilderBlock(new LayoutBuilder(new Style()), GetStyleFromName(styleName));

        // Bind the pane and its content together and add an instruction to create it to the CURRENT group
        CurrentGroup().AddInstruction(new GuiLayoutInstruction(elementId, new ScrollAreaDescription(childGroup, scrollPositionId)));

        // The child group becomes the new current group to build on
        _groupStack.Push(childGroup);

        return childGroup;
    }

    [UsedImplicitly]
    [LuaMember("endGroup")]
    public void EndGroup()
    {
        var popSuccessful = _groupStack.TryPop(out var result);

        if (!popSuccessful)
        {
            Client.Debug.LogWarning("Gui popped an empty stack, it looks like you called endGroup one time too many");
        }
    }

    [MoonSharpHidden]
    public void Clear()
    {
        _groupStack.Clear();
        _rootBuilder.Clear();
        _idPool = 0;
    }

    [MoonSharpHidden]
    public Gui.Gui CreateGui(RectangleF rootArea, LuaGuiBindingContext context)
    {
        var gui = new Gui.Gui();

        var builtLayout = _rootBuilder.Bake(rootArea);
        ApplyInstructions(_rootBuilder, builtLayout, context, gui);

        return gui;
    }

    [MoonSharpHidden]
    public void ApplyInstructions(LuaLayoutBuilderBlock block, LayoutArrangement builtLayout,
        LuaGuiBindingContext context, Gui.Gui gui)
    {
        foreach (var instruction in block.LocalInstructions())
        {
            var builtElement = builtLayout.FindElement(instruction.ElementId).Rectangle;
            instruction.GuiDescription.Apply(gui, builtElement, context, block, this);
        }
    }

    [MoonSharpHidden]
    private int NextId()
    {
        return _idPool++;
    }

    [MoonSharpHidden]
    private string AddElementToCurrentGroup(LayoutElement element)
    {
        var id = NextId();
        var elementName = "element:" + id;
        var pendingElement = element with {Name = new ElementName(elementName)};

        CurrentGroup().AddElement(pendingElement);
        return elementName;
    }
}
