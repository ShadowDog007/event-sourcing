using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

public record ServiceCollectionExtensionBuilder
{
    public ImmutableHashSet<string> Namespaces { get; private set; }
    public ImmutableArray<string> Lines { get; private set; }
    public Compilation Compilation { get; }
    public string ServiceTypes { get; }

    public string FileName => $"ServiceCollectionExtensions.{ServiceCollectionExtensionsModel.GetMethodName(Compilation, ServiceTypes)}.g.cs";

    public ServiceCollectionExtensionBuilder(Compilation compilation, string serviceTypes)
    {
        Namespaces = ImmutableHashSet<string>.Empty;
        Lines = ImmutableArray<string>.Empty;
        Compilation = compilation;
        ServiceTypes = serviceTypes;
    }

    public void AddComment(string comment)
    {
        Lines = Lines.Add($"// {comment}");
    }

    public void AddEmptyLine()
    {
        Lines = Lines.Add("");
    }

    public void AddNamespaces(IEnumerable<string> namespaces)
    {
        var builder = Namespaces.ToBuilder();
        builder.UnionWith(namespaces);

        Namespaces = builder.ToImmutableHashSet();
    }

    public void AddNamespaces(IEnumerable<INamedTypeSymbol> types)
    {
        AddNamespaces(types.Select(t => t.ContainingNamespace.ToString()));
    }

    private void Add(string method, INamedTypeSymbol service, INamedTypeSymbol implementation)
    {
        if (service.IsUnboundGenericType || implementation.IsUnboundGenericType)
        {
            // Generic registration
            if (!service.IsUnboundGenericType || !implementation.IsUnboundGenericType
                || service.TypeArguments.Length != implementation.TypeArguments.Length)
            {
                // Below we assume both types have the correct number of parameters
                // If they don't then at least this comment will explain why there is a compilation error
                Lines = Lines.Add($"// Registrations for <{service}, {implementation}> is invalid, open generics must have matching type argument lengths");
            }

            var commas = string.Join("", Enumerable.Repeat(',', service.TypeArguments.Length - 1));


            AddNamespaces([service, implementation]);
            Lines = Lines.Add($"{method}(typeof({service.Name}<{commas}>), typeof({implementation.Name}<{commas}>)");
        }
        else
        {
            var serviceName = GetShortTypeName(service, out var serviceTypes);
            var implementationName = GetShortTypeName(implementation, out var implementationTypes);

            AddNamespaces([..serviceTypes, ..implementationTypes]);
            Add(method, serviceName, implementationName);
        }
    }

    private void Add(string method, INamedTypeSymbol service, string implementation)
    {
        if (service.IsUnboundGenericType)
        {
            var commas = string.Join("", Enumerable.Repeat(',', service.TypeArguments.Length - 1));

            Lines = Lines.Add($"services.{method}(typeof({service.Name}<{commas}>),typeof({implementation}<{commas}>);");
        }
        else
        {
            var serviceName = GetShortTypeName(service, out var serviceTypes);

            AddNamespaces([..serviceTypes]);
            Add(method, serviceName, implementation);
        }
    }

    private void Add(string method, string service, string implementation)
    {
        Lines = Lines.Add($"services.{method}<{service}, {implementation}>();");
    }

    public void Add(string method, INamedTypeSymbol argument)
    {
        var name = GetShortTypeName(argument, out var serviceTypes);

        AddNamespaces([..serviceTypes]);
        Lines = Lines.Add($"services.{method}<{name}>();");
    }

    private string GetShortTypeName(INamedTypeSymbol type, out IEnumerable<INamedTypeSymbol> usedTypes)
    {
        if (!type.IsGenericType)
        {
            usedTypes = [type];
            return type.Name;
        }

        var typeParameters = string.Join(", ", type.TypeArguments.Select(t => t is INamedTypeSymbol n ? n.Name : t.ToString()));

        usedTypes = [type, .. type.TypeArguments.OfType<INamedTypeSymbol>()];
        return $"{type.Name}<{typeParameters}>";
    }

    public void AddSingleton(INamedTypeSymbol service, INamedTypeSymbol implementation)
        => Add("AddSingleton", service, implementation);
    public void AddScoped(INamedTypeSymbol service, INamedTypeSymbol implementation)
        => Add("AddScoped", service, implementation);
    public void AddTransient(INamedTypeSymbol service, INamedTypeSymbol implementation)
        => Add("AddTransient", service, implementation);

    public void AddSingleton(INamedTypeSymbol service, string implementation)
        => Add("AddSingleton", service, implementation);
    public void AddScoped(INamedTypeSymbol service, string implementation)
        => Add("AddScoped", service, implementation);
    public void AddTransient(INamedTypeSymbol service, string implementation)
        => Add("AddTransient", service, implementation);

    public void AddSingleton(string service, string implementation)
        => Add("AddSingleton", service, implementation);
    public void AddScoped(string service, string implementation)
        => Add("AddScoped", service, implementation);
    public void AddTransient(string service, string implementation)
        => Add("AddTransient", service, implementation);

    public ServiceCollectionExtensionsModel ToModel()
    {
        return new ServiceCollectionExtensionsModel(Compilation, ServiceTypes)
            .WithNamespaces(Namespaces.ToArray()) with
        {
            Lines = Lines,
        };
    }
}

public record ServiceCollectionExtensionsModel : TemplateModelBase
{
    public string MethodName { get; init; }
    public ImmutableArray<string> Lines { get; init; }

    public ServiceCollectionExtensionsModel(Compilation compilation, string serviceTypes)
        : base(compilation.AssemblyName!, $"ServiceCollectionExtensions.{GetMethodName(compilation, serviceTypes)}", [])
    {
        MethodName = GetMethodName(compilation, serviceTypes);
        Lines = ImmutableArray<string>.Empty;
    }

    public static string GetMethodName(Compilation compilation, string serviceTypes)
        => $"Add{compilation.GetSimpleAssemblyName()}{serviceTypes}";
}
