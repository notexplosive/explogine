using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Layout;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MoonSharp.Interpreter;

namespace ExplogineMonoGame.Luigi;

/// <summary>
///     Holds the LayoutBuilder and the Instructions for one "Block" of layout.
///     Most views are a single Block, but sometimes we have 1 or many scrollable Blocks.
/// </summary>
public class LuaLayoutBuilderBlock
{
    private readonly List<GuiLayoutInstruction> _instructions = new();

    private readonly LayoutBuilder _layout;

    public LuaLayoutBuilderBlock(LayoutBuilder layout, IGuiTheme theme)
    {
        _layout = layout;
        Theme = theme;
    }

    private LuaLayoutBuilderBlock(LayoutBuilder layout, List<GuiLayoutInstruction> instructions, IGuiTheme theme) :
        this(layout, theme)
    {
        _instructions = instructions;
    }

    public IGuiTheme Theme { get; }

    [MoonSharpHidden]
    public void AddInstruction(GuiLayoutInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    /// <summary>
    ///     Adds a subgroup that continues to write to the same instruction list. Basically just an indirect way of doing
    ///     Layout.AddGroup
    /// </summary>
    public LuaLayoutBuilderBlock AddGroupWithSameInstructions(Style style, LayoutElement element, IGuiTheme theme)
    {
        return new LuaLayoutBuilderBlock(_layout.AddGroup(style, element), _instructions, theme);
    }

    [MoonSharpHidden]
    public void AddElement(LayoutElement element)
    {
        _layout.Add(element);
    }

    [MoonSharpHidden]
    public LayoutArrangement Bake(RectangleF rootArea)
    {
        return _layout.Bake(rootArea);
    }

    [MoonSharpHidden]
    public void Clear()
    {
        _layout.Clear();
        _instructions.Clear();
    }

    /// <summary>
    ///     All instructions in this group, not including child groups
    /// </summary>
    [MoonSharpHidden]
    public IEnumerable<GuiLayoutInstruction> LocalInstructions()
    {
        return _instructions;
    }

    [UsedImplicitly]
    [LuaMember("horizontal")]
    public LuaLayoutBuilderBlock MakeHorizontal()
    {
        _layout.Style = _layout.Style with {Orientation = Orientation.Horizontal};
        return this;
    }

    [UsedImplicitly]
    [LuaMember("vertical")]
    public LuaLayoutBuilderBlock MakeVertical()
    {
        _layout.Style = _layout.Style with {Orientation = Orientation.Vertical};
        return this;
    }

    [UsedImplicitly]
    [LuaMember("align")]
    public LuaLayoutBuilderBlock Align(string horizontalAlignment, string verticalAlignment)
    {
        var newAlignment = _layout.Style.Alignment;
        switch (horizontalAlignment, verticalAlignment)
        {
            case ("left", "top"):
                newAlignment = Alignment.TopLeft;
                break;

            case ("left", "center"):
                newAlignment = Alignment.CenterLeft;
                break;

            case ("left", "bottom"):
                newAlignment = Alignment.BottomLeft;
                break;

            case ("center", "top"):
                newAlignment = Alignment.TopCenter;
                break;

            case ("center", "center"):
                newAlignment = Alignment.Center;
                break;

            case ("center", "bottom"):
                newAlignment = Alignment.BottomCenter;
                break;

            case ("right", "top"):
                newAlignment = Alignment.TopRight;
                break;

            case ("right", "center"):
                newAlignment = Alignment.CenterRight;
                break;

            case ("right", "bottom"):
                newAlignment = Alignment.BottomRight;
                break;

            default:
                Client.Debug.Log("Unknown alignment:", horizontalAlignment, verticalAlignment);
                break;
        }

        _layout.Style = _layout.Style with {Alignment = newAlignment};
        return this;
    }

    [UsedImplicitly]
    [LuaMember("padding")]
    public LuaLayoutBuilderBlock Padding(int padding)
    {
        _layout.Style = _layout.Style with {PaddingBetweenElements = padding};
        return this;
    }

    [UsedImplicitly]
    [LuaMember("margin")]
    public LuaLayoutBuilderBlock Margin(float horizontal, float vertical)
    {
        _layout.Style = _layout.Style with {Margin = new Vector2(horizontal, vertical)};
        return this;
    }
}
