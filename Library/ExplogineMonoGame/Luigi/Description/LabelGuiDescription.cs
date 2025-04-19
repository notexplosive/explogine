using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class LabelGuiDescription : IGuiDescription
{
    private readonly int? _fontSize;
    private readonly string _text;

    public LabelGuiDescription(string text, int? fontSize = null)
    {
        _text = text;
        _fontSize = fontSize;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        gui.Label(rectangle, Depth.Middle, _text, Alignment.BottomLeft, _fontSize);
    }
}
