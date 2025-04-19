using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class ButtonGuiDescription : IGuiDescription
{
    private readonly LuaGuiCommand _command;
    private readonly string _label;

    public ButtonGuiDescription(LuaGuiCommand command, string label)
    {
        _command = command;
        _label = label;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        context.LuaRegisterButtonCommand(_command);
        gui.Button(rectangle, _label, Depth.Middle, () => context.LuaRunButtonCommand(_command));
    }
}
