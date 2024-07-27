using System;
using System.Collections.Generic;
using System.Reflection;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui;
using ExplogineMonoGame.Layout;
using ExplogineMonoGame.Rails;
using MoonSharp.Interpreter;

namespace ExplogineMonoGame.Luigi;

public class LuaGui : IUpdateInputHook, IDrawHook, IEarlyDrawHook
{
    private readonly string _fileName;
    private readonly LuaLayoutBuilder _layoutBuilder;
    private readonly IGuiTheme _theme;
    private Gui.Gui? _gui;

    public LuaGui(IFileSystem files,string fileName, Style style, IGuiTheme theme, Dictionary<string, IGuiTheme> otherThemes)
    {
        _fileName = fileName;
        _theme = theme;
        LuaRuntime = new LuaRuntime(files);
        LuaRuntime.MessageLogged += ClientLuaUtilities.LogMessage;
        LuaRuntime.RegisterAssembly(Assembly.GetExecutingAssembly());
        LuaRuntime.RegisterType(typeof(LayoutElement));
        LuaRuntime.RegisterType(typeof(Alignment));
        LuaRuntime.RegisterType(typeof(Orientation));

        _layoutBuilder = new LuaLayoutBuilder(style, theme, otherThemes);
        LuaRuntime.SetGlobal("layout", _layoutBuilder);
        LuaRuntime.SetGlobal("fill", LuaGuiStatic.Fill);
        LuaRuntime.SetGlobal("gui", LuaGuiStatic.Gui);
        LuaRuntime.SetGlobal("orientation", LuaGuiStatic.Orientation);
        LuaRuntime.SetGlobal("alignment", LuaGuiStatic.Alignment);
        LuaRuntime.SetGlobal("command", (Func<string, DynValue[], LuaGuiCommand>) GenerateCommand);
    }

    public LuaRuntime LuaRuntime { get; }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch();
        _gui?.Draw(painter, _theme);
        painter.EndSpriteBatch();
    }

    public void EarlyDraw(Painter painter)
    {
        _gui?.PrepareCanvases(painter, _theme);
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _gui?.UpdateInput(input, hitTestStack);
    }

    public LuaGuiBindingContext RunLua(RectangleF rootArea, Table args, LuigiRememberedState? rememberedState)
    {
        LuaRuntime.SetGlobal("args", args);

        // Clear the state of the current layout
        _layoutBuilder.Clear();

        // Consume instructions
        LuaRuntime.DoFile(_fileName);

        var context = new LuaGuiBindingContext(rememberedState);

        // Run instructions
        _gui = _layoutBuilder.CreateGui(rootArea, context);

        return context;
    }

    private LuaGuiCommand GenerateCommand(string commandName, params DynValue[] arguments)
    {
        return new LuaGuiCommand(commandName, arguments);
    }

    /// <summary>
    ///     You MUST call this when you're done with the LuaGui. It creates widgets that need to be disposed.
    /// </summary>
    public void Clear()
    {
        // Disposes of Widgets
        _gui?.Clear();
    }

    public void HandleErrors(LuaGuiBindingContext context)
    {
        foreach (var command in context.UnboundCommands())
        {
            Client.Debug.LogWarning("Missing Binding:", command);
        }

        if (LuaRuntime.CurrentError != null)
        {
            var exception = LuaRuntime.CurrentError.Exception;
            if (exception is ScriptRuntimeException)
            {
                Client.Debug.LogError(exception.Message);
                Client.Debug.LogError(LuaRuntime.Callstack());
            }
            else
            {
                Client.Debug.LogError(exception);
            }
        }
    }
}
