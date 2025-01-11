namespace ExplogineCore.Lua;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class LuaMemberAttribute : Attribute
{
    public LuaMemberAttribute(string luaVisibleName)
    {
        LuaVisibleName = luaVisibleName;
    }

    public string LuaVisibleName { get; }
}
