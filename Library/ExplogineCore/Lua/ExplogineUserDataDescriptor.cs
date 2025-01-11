using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace ExplogineCore.Lua;

public class ExplogineUserDataDescriptor : StandardUserDataDescriptor
{
    private readonly Dictionary<string, IMemberDescriptor> _memberNameToDescriptor = new();

    public ExplogineUserDataDescriptor(Type type, InteropAccessMode accessMode, string? friendlyName = null) : base(
        type, accessMode,
        friendlyName)
    {
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);

        foreach (var member in members)
        {
            if (Attribute.IsDefined(member, typeof(LuaMemberAttribute)))
            {
                var luaMemberAttribute = (LuaMemberAttribute) member.GetCustomAttributes(typeof(LuaMemberAttribute))
                    .First(a => a is LuaMemberAttribute);
                var name = luaMemberAttribute.LuaVisibleName;

                if (name == null)
                {
                    throw new Exception($"Could not find lua name for {type.Name}.{member.Name}");
                }

                if (_memberNameToDescriptor.TryGetValue(name, out var found))
                {
                    throw new Exception(
                        $"Duplicate name alias in {type.Name}. Both {member.Name} and {found.Name} are aliased as {name}");
                }

                if (member is MethodInfo methodInfo)
                {
                    _memberNameToDescriptor[name] = new MethodMemberDescriptor(methodInfo);
                }

                if (member is PropertyInfo propertyInfo)
                {
                    _memberNameToDescriptor[name] = new PropertyMemberDescriptor(propertyInfo, accessMode);
                }
            }
        }
    }

    public override DynValue? Index(Script script, object clrObject, DynValue index, bool isDirectIndexing)
    {
        // Client.Debug.Log("Index", clrObject, index, isDirectIndexing);
        if (!isDirectIndexing)
        {
            // fallback to default behavior (mostly because I don't understand it and I want LuaList to work)
            return base.Index(script, clrObject, index, isDirectIndexing);
        }

        index = index.ToScalar();

        if (index.Type != DataType.String)
        {
            return null;
        }

        if (_memberNameToDescriptor.TryGetValue(index.String, out var memberDescriptor))
        {
            return memberDescriptor.GetValue(script, clrObject);
        }

        // this was doing base.Index, now it just fails
        return null;
    }

    public override bool SetIndex(Script script, object clrObject, DynValue index, DynValue value,
        bool isDirectIndexing)
    {
        // Client.Debug.Log("SetIndex", clrObject, index, isDirectIndexing);
        if (!isDirectIndexing)
        {
            // fallback to default behavior (mostly because I don't understand it and I want LuaList to work)
            return base.SetIndex(script, clrObject, index, value, isDirectIndexing);
        }

        index = index.ToScalar();

        if (index.Type != DataType.String)
        {
            return false;
        }

        if (_memberNameToDescriptor.TryGetValue(index.String, out var foundDescriptor))
        {
            LuaRuntime.SafeWrapCallStatic(script, () =>
            {
                foundDescriptor.SetValue(script, clrObject, value);
                return DynValue.Nil;
            });
            return true;
        }

        return false;
    }
}
