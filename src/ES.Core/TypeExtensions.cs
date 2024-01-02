namespace ES.Core;

public static class TypeExtensions
{
    public static bool ClosesType(this Type type, Type generic, out Type[] arguments)
    {
        var types = generic switch
        {
            { IsClass: true } => type.EnumerateSelfAndBaseTypes(),
            { IsInterface: true } => type.GetInterfaces(),
            _ => [],
        };

        foreach (var candidate in types)
        {
            if (generic == candidate
                || generic.IsGenericTypeDefinition && candidate.IsConstructedGenericType
                    && candidate.GetGenericTypeDefinition() == generic)
            {
                arguments = generic.IsGenericTypeDefinition ? candidate.GetGenericArguments() : [];
                return true;
            }
        }

        arguments = [];
        return false;
    }

    public static IEnumerable<Type> EnumerateBaseTypes(this Type type)
    {
        var baseType = type.BaseType;

        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    public static IEnumerable<Type> EnumerateSelfAndBaseTypes(this Type type)
    {
        return type.EnumerateBaseTypes().Prepend(type);
    }
}
