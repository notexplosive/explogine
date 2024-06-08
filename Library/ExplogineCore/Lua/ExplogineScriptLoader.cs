using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;

namespace ExplogineCore.Lua;

public class ExplogineScriptLoader : ScriptLoaderBase
{
    private readonly IFileSystem _files;

    public ExplogineScriptLoader(IFileSystem files)
    {
        // we don't support loading from anywhere but the mod folder
        ModulePaths = new[] {"?.lua"};
        _files = files;
    }

    public override bool ScriptFileExists(string name)
    {
        return _files.HasFile(name);
    }

    public override object LoadFile(string file, Table globalContext)
    {
        return _files.ReadFile(file);
    }
}
