using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class TextFieldGuiDescription : IGuiDescription
{
    private readonly LuaGuiCommand? _initializeCommand;
    private readonly LuaGuiCommand _submitCommand;

    public TextFieldGuiDescription(LuaGuiCommand submitCommand, LuaGuiCommand? initializeCommand)
    {
        _submitCommand = submitCommand;
        _initializeCommand = initializeCommand;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        var textInputWidget = gui.TextInputWidget(rectangle, block.Theme.Font,
            new TextInputWidget.Settings {IsSingleLine = true, Selector = new AlwaysSelected()});

        context.LuaRegisterTextFieldModifyCommand(_submitCommand);
        textInputWidget.Submitted += () =>
        {
            context.LuaRunTextFieldModified(_submitCommand, textInputWidget.Text, true);
        };

        textInputWidget.TextChanged += text => { context.LuaRunTextFieldModified(_submitCommand, text, false); };

        if (_initializeCommand.HasValue)
        {
            context.LuaRegisterTextFieldInitializeCommand(_initializeCommand.Value);
            context.OnFinalize += () =>
            {
                context.LuaRunTextFieldInitialize(_initializeCommand.Value, textInputWidget);
            };
        }
    }
}
