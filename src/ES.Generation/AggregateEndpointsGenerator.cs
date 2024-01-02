using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;

namespace ES.Generation;

/// <summary>
/// Generates EndpointRouteBuilderExtensions to register aggregate read/command endpoints
/// </summary>
[Generator]
public class AggregateEndpointsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var aggregates = context.SyntaxProvider.CreateAggregateSyntaxProvider();

        context.RegisterSourceOutput(aggregates.Combine(context.CompilationProvider),
            (ctx, t) => GenerateEndpoints(ctx, t.Right, t.Left));
    }

    private void GenerateEndpoints(SourceProductionContext context, Compilation compilation, INamedTypeSymbol aggregate)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var symbols = new AggregateTypeSymbols(compilation, aggregate);
            var model = new AggregateEndpointsModel(compilation, symbols);

            context.AddRenderedSource(model, Templates.RenderAggregateEndpoints);
        }
        catch (Exception ex)
        {
            context.AddSource($"{nameof(AggregateHandlerGenerator)}.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}

public record AggregateEndpointsModel : TemplateModelBase
{
    public string MapName { get; init; }
    public INamedTypeSymbol Aggregate { get; init; }
    public ITypeSymbol AggregateState { get; init; }
    public IEnumerable<INamedTypeSymbol> Commands { get; init; }

    public AggregateEndpointsModel(Compilation compilation, AggregateTypeSymbols symbols)
        : base(compilation.AssemblyName!, $"EndpointRouteBuilderExtensions.{GetMapName(compilation, symbols.AggregateType)}",
            [symbols.AggregateType, symbols.AggregateStateType, ..symbols.CommandHandlers.Select(h => h.CommandType)])
    {
        MapName = GetMapName(compilation, symbols.AggregateType);
        Aggregate = symbols.AggregateType;
        AggregateState = symbols.AggregateStateType;
        Commands = symbols.CommandHandlers.Select(h => h.CommandType);
    }

    public static string GetMapName(Compilation compilation, INamedTypeSymbol aggregateType)
    {
        var domainName = compilation.AssemblyName!.Replace("ES.", "").Replace(".", "");

        return $"Map{domainName}{aggregateType.Name}Routes";
    }
}
