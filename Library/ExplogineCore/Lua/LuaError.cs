using MoonSharp.Interpreter;

namespace ExplogineCore.Lua;

#pragma warning disable CS8974
/// <summary>
///     Used internally to manage error propagation
/// </summary>
[LuaBoundType]
public class LuaError
{
    public LuaError(Exception runtimeException)
    {
        Exception = runtimeException;
    }

    [MoonSharpHidden]
    public Exception Exception { get; }
}
