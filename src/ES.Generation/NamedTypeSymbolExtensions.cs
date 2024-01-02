using Microsoft.CodeAnalysis;

namespace ES.Generation;

public static class NamedTypeSymbolExtensions
{
    public static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(this ITypeSymbol type)
    {
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }
}


