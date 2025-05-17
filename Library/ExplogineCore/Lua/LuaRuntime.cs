using System.Reflection;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Debugging;

#pragma warning disable CS8974

namespace ExplogineCore.Lua;

public class LuaRuntime
{
    private const string ErrorKeyName = "__CURRENT_ERROR";
    private static readonly HashSet<Assembly> KnownAssemblies = new();
    private readonly Dictionary<string, DynValue> _loadedModules = new();
    private readonly Script _lua;
    private readonly IFileSystem _root;

    private LuaError? _errorFromClr;

    public LuaRuntime(IFileSystem root)
    {
        _root = root;
        _lua = new Script(CoreModules.Preset_HardSandbox | CoreModules.Json | CoreModules.Dynamic |
                          CoreModules.OS_Time | CoreModules.Coroutine | CoreModules.ErrorHandling |
                          CoreModules.Metatables | CoreModules.LoadMethods | CoreModules.Debug)
        {
            Options =
            {
                ScriptLoader = new ExplogineScriptLoader(root)
            }
        };

        RegisterAssembly(Assembly.GetExecutingAssembly());
        RegisterAssembly(Assembly.GetCallingAssembly());

        SetGlobal("SOKO_DEBUG_LOG", DebugLog);
        SetGlobal("require", Require);
        SetGlobal("requireIfExists", RequireIfExists);

        _lua.DoString(@"
function print(...)
    SOKO_DEBUG_LOG({...})
end
", _lua.Globals, "SETUP");
    }

    public LuaError? CurrentError
    {
        get
        {
            var errorFromLua = _lua.Globals.Get(ErrorKeyName).ToObject<LuaError>();
            return _errorFromClr ?? errorFromLua ?? null;
        }
        private set => _errorFromClr = value;
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        if (KnownAssemblies.Contains(assembly))
        {
            // Already registered, do nothing
            return;
        }

        var types = Reflection.GetAllMembersInAssemblyWithAttribute<LuaMemberAttribute>(assembly)
            .Select(a => a.Item2).ToList();

        // For types that dont have any [LuaMember] members but want to be picked up anyway. They can be [LuaBoundType]s
        foreach (var type in Reflection.GetAllTypesWithAttribute<LuaBoundTypeAttribute>(assembly))
        {
            types.Add(type);
        }

        foreach (var type in types.Distinct())
        {
            RegisterType(type);
        }

        KnownAssemblies.Add(assembly);
    }

    /// <summary>
    ///     Manually register a type that does not have the [LuaBoundType] attribute
    /// </summary>
    public static void RegisterType(Type type)
    {
        if (!type.IsAbstract)
        {
#if DEBUG
            // Theoretically this useful for hot reload. But it doesn't seem to work because Hot Reload doesn't pick up attribute changes
            UserData.UnregisterType(type);
#endif
            UserData.RegisterType(new ExplogineUserDataDescriptor(type, InteropAccessMode.Default, type.Name));
        }
    }

    public DynValue RequireIfExists(string moduleName)
    {
        return RequireInternal(moduleName, true);
    }

    public DynValue Require(string moduleName)
    {
        return RequireInternal(moduleName, false);
    }

    private DynValue RequireInternal(string moduleName, bool canFail)
    {
        if (_loadedModules.TryGetValue(moduleName, out var module))
        {
            return module;
        }

        try
        {
            // MoonSharp's "RequireModule" wraps the file in a function and returns that.
            // We call the function, cache it's value and then return that value any subsequent time it's required.
            var moonSharpRequired = _lua.RequireModule(moduleName);
            _loadedModules[moduleName] = _lua.Call(moonSharpRequired);
            return _loadedModules[moduleName];
        }
        catch (ScriptRuntimeException runtimeException)
        {
            if (!canFail)
            {
                SetErrorIfUnset(runtimeException);
            }

            return DynValue.Nil;
        }
    }

    public void ClearCurrentError()
    {
        CurrentError = null;
    }

    public DynValue SafeWrapCall(Func<DynValue> wrapped)
    {
        try
        {
            return wrapped();
        }
        catch (Exception runtimeException)
        {
            SetErrorIfUnset(runtimeException);
        }

        return DynValue.Nil;
    }

    public static DynValue SafeWrapCallStatic(Script script, Func<DynValue> wrapped)
    {
        try
        {
            return wrapped();
        }
        catch (Exception runtimeException)
        {
            script.Globals[ErrorKeyName] = new LuaError(runtimeException);
        }

        return DynValue.Nil;
    }

    public void SetErrorIfUnset(Exception exception)
    {
        CurrentError ??= new LuaError(exception);
    }

    private void DebugLog(Table table)
    {
        var valuesToPrint = new List<object>();

        foreach (var item in LuaUtilities.EnumerateArrayLike(table))
        {
            if (item.Type == DataType.UserData)
            {
                valuesToPrint.Add(item.UserData.Object?.ToString() ?? "nil");
            }
            else if (item == null! || item.IsNil())
            {
                valuesToPrint.Add("nil");
            }
            else if (item.Type == DataType.Boolean)
            {
                valuesToPrint.Add(item.Boolean ? "true" : "false");
            }
            else if (item.Type == DataType.Number || item.Type == DataType.String)
            {
                valuesToPrint.Add(item.CastToString());
            }
            else
            {
                // just print the type (ie: Table)
                valuesToPrint.Add(item.Type);
            }
        }

        MessageLogged?.Invoke(valuesToPrint.ToArray());
    }

    public event Action<object[]>? MessageLogged;

    public void SetGlobal(string globalName, object value)
    {
        _lua.Globals.Set(globalName, DynValue.FromObject(_lua, value));
    }

    public DynValue GetGlobal(string globalName)
    {
        return _lua.Globals.Get(globalName);
    }

    /// <summary>
    ///     Executes a chunk and returns the result. If the chunk errors, we place the error on CurrentError and return nil.
    /// </summary>
    /// <returns>DynValue.Nil if the script fails to run, otherwise returns the result of the expression</returns>
    public DynValue Run(string text, string chunkName)
    {
        return SafeWrapCall(() => _lua.DoString(text, _lua.Globals, chunkName));
    }

    /// <summary>
    ///     Same as Run but we prepend the string with the token "return "
    /// </summary>
    /// <returns></returns>
    public DynValue RunAndReturn(string text, string chunkName)
    {
        text = "return " + text;
        return Run(text, chunkName);
    }

    public DynValue SafeCallKeyOnTable(Table table, string key, params object?[] parameters)
    {
        return SafeWrapCall(() =>
        {
            DynValue? result = null;
            var value = table.Get(key);

            if (value.Type == DataType.Function)
            {
                result = _lua.Call(value.Function, parameters);
            }

            return result ?? DynValue.Nil;
        });
    }

    public DynValue SafeCallFunction(Closure luaFunction, params object?[] args)
    {
        return SafeWrapCall(() => _lua.Call(luaFunction, args));
    }

    public DynValue SafeCallFunctionSingleReturn(Closure luaFunction, params object?[] args)
    {
        var result = SafeWrapCall(() => _lua.Call(luaFunction, args));

        if (result.Type == DataType.Tuple)
        {
            return result.Tuple.First();
        }

        return result;
    }

    public void LoadSeveralFiles(string directoryPath, Action<string, DynValue, LuaRuntime> onModuleLoaded)
    {
        foreach (var fileName in _root.GetFilesAt(directoryPath, "lua"))
        {
            var fullName = new FileInfo(fileName).Name;
            var module = DoFile(fileName);

            onModuleLoaded(fullName.RemoveFileExtension(), module, this);
        }
    }

    public string Info()
    {
        var moonSharp = _lua.Globals.Get("_MOONSHARP").Table;
        return moonSharp.Get("banner").String;
    }

    public string FormatCallstackItem(WatchItem stackItem)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(stackItem.Name);

        if (stackItem.Location != null)
        {
            if (stackItem.Location.IsClrLocation)
            {
                return "[Built In Code]";
            }

            stringBuilder.Append(" ");
            stringBuilder.Append(stackItem.Location.FormatLocation(_lua, true));
        }

        return stringBuilder.ToString();
    }

    public DynValue DoFile(string fileName)
    {
        if (!_root.HasFile(fileName))
        {
            SetErrorIfUnset(new FileNotFoundException("DoFile could not find the file specified", fileName));
            return DynValue.Nil;
        }

        return SafeWrapCall(() => _lua.DoFile(fileName));
    }

    public DynValue FromObject(object item)
    {
        return DynValue.FromObject(_lua, item);
    }

    public Table NewTable()
    {
        return NewTableAsDynValue().Table;
    }

    public DynValue NewTableAsDynValue()
    {
        return DynValue.NewTable(_lua);
    }

    public string Callstack()
    {
        var stringBuilder = new StringBuilder();
        var error = CurrentError?.Exception;
        if (error is InterpreterException luaError)
        {
            if (luaError.CallStack != null)
            {
                var callstackBuilder = new StringBuilder();
                foreach (var stackItem in luaError.CallStack)
                {
                    var line = FormatCallstackItem(stackItem);
                    callstackBuilder.AppendLine(line);
                }

                stringBuilder.Append(callstackBuilder);
            }
        }
        else
        {
#if DEBUG
            stringBuilder.AppendLine("C# callstack");
            stringBuilder.AppendLine(CurrentError?.Exception.ToString());
            stringBuilder.AppendLine(CurrentError?.Exception.StackTrace);
#else
            stringBuilder.AppendLine("This type of error can't generate a callstack. :(");
#endif
        }

        return stringBuilder.ToString();
    }
}
