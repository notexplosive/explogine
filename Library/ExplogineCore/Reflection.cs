using System.Diagnostics.Contracts;
using System.Reflection;

namespace ExplogineCore;

public static class Reflection
{
    /// <summary>
    ///     Gets static fields from type T that derive from TInterface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    [Pure]
    public static Dictionary<string, TInterface> GetStaticFieldsThatDeriveFromType<T, TInterface>()
    {
        return GetStaticFieldsThatDeriveFromType<TInterface>(typeof(T));
    }

    /// <summary>
    ///     Gets static fields from type T that derive from TInterface
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    [Pure]
    public static Dictionary<string, TInterface> GetStaticFieldsThatDeriveFromType<TInterface>(Type t)
    {
        return t
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(fieldInfo => fieldInfo.FieldType.GetInterfaces().Contains(typeof(TInterface)) ||
                                fieldInfo.FieldType == typeof(TInterface))
            .ToDictionary(
                fieldInfo => fieldInfo.Name,
                fieldInfo => (TInterface) fieldInfo.GetValue(null)!
            );
    }

    /// <summary>
    ///     Gets static fields from type T that derive from TInterface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    [Pure]
    public static IEnumerable<FieldInfo> GetStaticFieldInfosThatDeriveFromType<T, TInterface>()
    {
        return typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(fieldInfo => fieldInfo.FieldType.GetInterfaces().Contains(typeof(TInterface)));
    }

    [Pure]
    public static List<Type> GetAllTypesThatDeriveFrom<T>()
    {
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Find all types that implement ISpecificInterface
        var implementingTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

            implementingTypes.AddRange(types);
        }

        return implementingTypes;
    }

    [Pure]
    public static IEnumerable<Tuple<MemberInfo, Type>>
        GetAllMembersInAssemblyWithAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
    {
        var types = assembly.GetTypes();
        var attributeType = typeof(TAttribute);
        foreach (var type in types)
        {
            foreach (var member in type.GetMembers().Where(method => Attribute.IsDefined(method, attributeType)))
            {
                yield return new Tuple<MemberInfo, Type>(member, type);
            }
        }
    }

    [Pure]
    public static IEnumerable<MemberInfo> GetAllMembersInTypeWithAttribute<TAttribute>(Type type)
        where TAttribute : Attribute
    {
        var attributeType = typeof(TAttribute);
        foreach (var member in type
                     .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                 BindingFlags.Static).Where(method => Attribute.IsDefined(method, attributeType)))
        {
            yield return member;
        }
    }

    [Pure]
    public static IEnumerable<Type> GetAllTypesWithAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
    {
        var types = assembly.GetTypes();
        var attributeType = typeof(TAttribute);
        foreach (var type in types)
        {
            if (Attribute.IsDefined(type, attributeType))
            {
                yield return type;
            }
        }
    }

    [Pure]
    public static object? GetMemberValue(MemberInfo memberInfo, object instance)
    {
        if (memberInfo is FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue(instance);
        }

        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(instance);
        }

        if (memberInfo is MethodInfo methodInfo)
        {
            return methodInfo.Invoke(instance, []);
        }

        throw new Exception($"Could not invoke {memberInfo} on {instance}");
    }
}
