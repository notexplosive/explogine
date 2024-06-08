using MoonSharp.Interpreter;

namespace ExplogineCore.Lua;

#pragma warning disable CS8974
/// <summary>
/// Used internally to manage error propagation
/// </summary>
[LuaBoundType]
public class LuaError
{
    [MoonSharpHidden]
    public Exception Exception { get; }

    public LuaError(Exception runtimeException)
    {
        Exception = runtimeException;
    }
}
