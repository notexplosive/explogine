using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class GraphicGuiDescription : IGuiDescription
{
    private readonly LuaGuiCommand _command;

    public GraphicGuiDescription(LuaGuiCommand command)
    {
        _command = command;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        context.LuaRegisterGraphicCommand(_command);

        gui.DynamicLabel(rectangle, Depth.Middle,
            (painter, theme, rectangle2, depth) => { context.LuaDrawDynamicLabel(_command, painter, rectangle); });
    }
}
