using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

public interface IGuiDescription
{
    void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder);
}
