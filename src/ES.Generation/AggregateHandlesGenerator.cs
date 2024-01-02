using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generator adds <see cref="IAggregateHandles{TCommand}"/> marker interfaces to aggregates.
/// This interface is used to do compile time checks for usages of <see cref="IAggregateHandler{TAggregate, TCommand}"/>
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AggregateHandlesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateAggregateSyntaxProvider();

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider),
            (ctx, t) => GenerateHandles(ctx, t.Right, t.Left));
    }

    private void GenerateHandles(SourceProductionContext context, Compilation compilation, INamedTypeSymbol aggregate)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var symbols = new AggregateTypeSymbols(compilation, aggregate);
            var model = new AggregateHandlesModel(aggregate, symbols.CommandHandlers.Select(c => c.CommandType).ToImmutableArray());

            context.AddRenderedSource(model, Templates.RenderAggregateHandles);
        }
        catch (Exception ex)
        {
            context.AddSource($"{nameof(aggregate.Name)}.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}

public record AggregateHandlesModel : TemplateModelBase
{
    public AggregateHandlesModel(INamedTypeSymbol aggregate, ImmutableArray<INamedTypeSymbol> commands)
        : base(aggregate.ContainingNamespace, aggregate.Name, [aggregate, ..commands])
    {
        Aggregate = aggregate;
        Commands = commands;
    }

    public INamedTypeSymbol Aggregate { get; init; }
    public ImmutableArray<INamedTypeSymbol> Commands { get; init; }
}
