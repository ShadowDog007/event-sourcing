using Microsoft.CodeAnalysis;

namespace ES.Generation;

public static class TemplateModelExtensions
{
    public static TModel WithNamespaces<TModel>(this TModel model, params ITypeSymbol[] types)
        where TModel : TemplateModelBase
    {
        return model.WithNamespaces<TModel>(types.Select(t => t.ContainingNamespace.ToString()).ToArray());
    }

    public static TModel WithNamespaces<TModel>(this TModel model, params string[] types)
        where TModel : TemplateModelBase
    {
        var builder = model.Namespaces.ToBuilder();
        builder.UnionWith(types);

        return model with
        {
            Namespaces = builder.ToImmutable(),
        };
    }
}


