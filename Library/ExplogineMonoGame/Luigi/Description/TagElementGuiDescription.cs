using ExplogineCore.Lua;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class TagElementGuiDescription : IGuiDescription
{
    private readonly string _tag;

    public TagElementGuiDescription(string tag)
    {
        _tag = tag;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        context.AddTag(_tag, rectangle);
    }
}
