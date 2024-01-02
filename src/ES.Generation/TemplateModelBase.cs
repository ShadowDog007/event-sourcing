using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

public record TemplateModelBase
{
    public string Namespace { get; init; }
    public string Name { get; init; }
    public string FileName => $"{Name}.g.cs";
    public ImmutableHashSet<string> Namespaces { get; init; }
    public IEnumerable<string> SortedNamespaces => Namespaces.OrderBy(n => n, NamespaceComparer.Default);

    public TemplateModelBase(string fileNamespace, string name, ITypeSymbol?[] symbols, string[]? additionalNamespaces = null)
    {
        Namespace = fileNamespace;
        Name = name;
        Namespaces = symbols
            .Where(s => s is not null)
            .Select(s => s!.ContainingNamespace)
            .Select(n => n.ToString())
            .Concat(additionalNamespaces ?? [])
            .Where(n => n != Namespace)
            .ToImmutableHashSet();
    }

    public TemplateModelBase(INamespaceSymbol fileNamespace, string name, ITypeSymbol?[] symbols, string[]? additionalNamespaces = null)
        : this(fileNamespace.ToString(), name, symbols, additionalNamespaces)
    {
    }
}
