using System.Reflection;

namespace ExplogineCore;

public static class Reflection
{
    /// <summary>
    /// Gets static fields from type T that derive from TInterface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TInterface"></typeparam>
    /// <returns></returns>
    public static Dictionary<string, TInterface> GetStaticFieldsThatDeriveFromType<T, TInterface>()
    {
        return typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(fieldInfo => fieldInfo.FieldType.GetInterfaces().Contains(typeof(TInterface)))
            .ToDictionary(
                fieldInfo => fieldInfo.Name,
                fieldInfo => (TInterface) fieldInfo.GetValue(null)!
            );
    }

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

    public static IEnumerable<Tuple<MemberInfo, Type>> GetAllMembersInAssemblyWithAttribute<TAttribute>(Assembly assembly) where TAttribute : Attribute
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
}
