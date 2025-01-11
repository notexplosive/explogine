using System;
using System.Collections.Generic;
using System.Linq;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using MoonSharp.Interpreter;

namespace ExplogineMonoGame.Luigi;

public class LuaGuiBindingContext
{
    public delegate void ButtonDelegate(DynValue[] args);

    public delegate void GraphicDelegate(DynValue[] args, Painter painter, RectangleF rectangle);

    public delegate void TextFieldInitializeDelegate(DynValue[] args, TextInputWidget textInputWidget);

    public delegate void TextFieldModifyDelegate(DynValue[] args, string currentText, bool isSubmit);

    private readonly Dictionary<string, ButtonDelegate?> _buttonCommands = new();
    private readonly Dictionary<string, GraphicDelegate?> _labelCommands = new();

    private readonly LuaRuntime _luaRuntime;
    private readonly Dictionary<string, RectangleF> _tagLookup = new();
    private readonly Dictionary<string, TextFieldInitializeDelegate?> _textFieldInitializeCommands = new();
    private readonly Dictionary<string, TextFieldModifyDelegate?> _textFieldModifyCommands = new();

    public LuaGuiBindingContext(LuaRuntime luaRuntime, LuigiRememberedState? rememberedState)
    {
        _luaRuntime = luaRuntime;
        RememberedState = rememberedState ?? new LuigiRememberedState();
    }

    public LuigiRememberedState RememberedState { get; }

    public event Action? OnFinalize;

    public void LuaRunButtonCommand(LuaGuiCommand command)
    {
        Client.Debug.LogVerbose("Run GUI Command", command.CommandName,
            string.Join(",", command.Arguments.Select(a => a.ToString()).ToList()));

        _buttonCommands[command.CommandName]?.Invoke(command.Arguments);
    }

    internal void LuaDrawDynamicLabel(LuaGuiCommand command, Painter painter, RectangleF rectangle)
    {
        _labelCommands[command.CommandName]?.Invoke(command.Arguments, painter, rectangle);
    }

    internal void LuaRunTextFieldModified(LuaGuiCommand command, string currentText, bool isSubmit)
    {
        _textFieldModifyCommands[command.CommandName]?.Invoke(command.Arguments, currentText, isSubmit);
    }

    internal void LuaRunTextFieldInitialize(LuaGuiCommand command, TextInputWidget widget)
    {
        _textFieldInitializeCommands[command.CommandName]?.Invoke(command.Arguments, widget);
    }

    public IEnumerable<string> UnboundCommands()
    {
        foreach (var command in _buttonCommands.Keys)
        {
            if (_buttonCommands[command] == null)
            {
                yield return command;
            }
        }

        foreach (var command in _labelCommands.Keys)
        {
            if (_labelCommands[command] == null)
            {
                yield return command;
            }
        }
    }

    public void BindButtonCommand(string commandName, ButtonDelegate action)
    {
        if (_buttonCommands.ContainsKey(commandName))
        {
            _buttonCommands[commandName] = action;
        }
    }

    public void BindGraphicCommand(string commandName, GraphicDelegate action)
    {
        if (_labelCommands.ContainsKey(commandName))
        {
            _labelCommands[commandName] = action;
        }
    }

    public void BindTextFieldModifiedCommand(string commandName, TextFieldModifyDelegate action)
    {
        if (_textFieldModifyCommands.ContainsKey(commandName))
        {
            _textFieldModifyCommands[commandName] = action;
        }
    }

    public void BindTextFieldInitializeCommand(string commandName, TextFieldInitializeDelegate action)
    {
        if (_textFieldInitializeCommands.ContainsKey(commandName))
        {
            _textFieldInitializeCommands[commandName] = action;
        }
    }

    internal void LuaRegisterButtonCommand(LuaGuiCommand command)
    {
        _buttonCommands.TryAdd(command.CommandName, null);
    }

    internal void LuaRegisterGraphicCommand(LuaGuiCommand command)
    {
        _labelCommands.TryAdd(command.CommandName, null);
    }

    internal void LuaRegisterTextFieldModifyCommand(LuaGuiCommand command)
    {
        _textFieldModifyCommands.TryAdd(command.CommandName, null);
    }

    internal void LuaRegisterTextFieldInitializeCommand(LuaGuiCommand command)
    {
        _textFieldInitializeCommands.TryAdd(command.CommandName, null);
    }

    public void FinishInitialize()
    {
        OnFinalize?.Invoke();
        OnFinalize = null;

        HandleErrors();
    }

    public void HandleErrors()
    {
        foreach (var command in UnboundCommands())
        {
            Client.Debug.LogWarning("Missing Binding:", command);
        }

        if (_luaRuntime.CurrentError != null)
        {
            var exception = _luaRuntime.CurrentError.Exception;
            if (exception is ScriptRuntimeException)
            {
                Client.Debug.LogError(exception.Message);
                Client.Debug.LogError(_luaRuntime.Callstack());
            }
            else
            {
                Client.Debug.LogError(exception);
            }
        }
    }

    public void AddTag(string tag, RectangleF rectangle)
    {
        _tagLookup[tag] = rectangle;
    }

    public RectangleF? GetRectangleAtTag(string tag)
    {
        if (_tagLookup.TryGetValue(tag, out var rectangle))
        {
            return rectangle;
        }

        return null;
    }
}
