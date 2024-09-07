using ExplogineCore.Lua;
using MoonSharp.Interpreter;

namespace ExplogineMonoGame.Luigi;

[LuaBoundType]
public readonly record struct LuaGuiCommand(string CommandName, DynValue[] Arguments);
