using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generates the entrypoint for the application,
/// and registers all the required services (generated ones and manually written ones)
/// </summary>
[Generator]
public class ProgramGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var serviceCollectionExtensions = context.SyntaxProvider.CreateServiceCollectionExtensionsProvider().Collect();
        var aggregates = context.SyntaxProvider.CreateAggregateSyntaxProvider().Collect();

        var provider = serviceCollectionExtensions.Combine(aggregates).Select((t, _) => (ServiceCollectionExtensions: t.Left, Aggregates: t.Right));

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider),
            (ctx, t) => GenerateProgram(ctx, t.Right, t.Left.ServiceCollectionExtensions, t.Left.Aggregates));
    }

    private void GenerateProgram(SourceProductionContext context, Compilation compilation,
        ImmutableArray<IMethodSymbol> addServicesMethods,
        ImmutableArray<INamedTypeSymbol> aggregates)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        context.AddRenderedSource(new ProgramModel(compilation, addServicesMethods, aggregates), Templates.RenderProgram);
    }
}

public record ProgramModel : TemplateModelBase
{
    public string ShortenedName { get; init; }
    public ImmutableArray<IMethodSymbol> AddServicesMethods { get; init; }
    public IEnumerable<string> EndpointMappings { get; init; }

    public ProgramModel(Compilation compilation, ImmutableArray<IMethodSymbol> addServicesMethods,
        ImmutableArray<INamedTypeSymbol> aggregates)
        : base(compilation.AssemblyName!, "Program", [], [compilation.AssemblyName!])
    {
        ShortenedName = compilation.AssemblyName!.Replace("ES.", "").Replace(".", "");
        AddServicesMethods = addServicesMethods;
        EndpointMappings = aggregates.Select(a => AggregateEndpointsModel.GetMapName(compilation, a));
    }
}

