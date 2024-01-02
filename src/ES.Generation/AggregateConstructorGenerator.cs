using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;

namespace ES.Generation;

/// <summary>
/// Generates the static abstract <see cref="IAggregateConstructor{TAggregate, TState}.Create"/> method
/// for aggregates, so they can be used by <see cref="IAggregateRepository{TAggregate, TState}"/>
/// </summary>
[Generator]
public class AggregateConstructorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var aggregates = context.SyntaxProvider.CreateAggregateSyntaxProvider();

        context.RegisterSourceOutput(aggregates.Combine(context.CompilationProvider),
            (ctx, t) => GenerateHandlers(ctx, t.Right, t.Left));
    }

    private void GenerateHandlers(SourceProductionContext context, Compilation compilation, INamedTypeSymbol aggregate)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var symbols = new AggregateTypeSymbols(compilation, aggregate);
            var model = new AggregateConstructorModel(aggregate, symbols.AggregateStateType);

            context.AddRenderedSource(model, Templates.RenderAggregateConstructor);
        }
        catch (Exception ex)
        {
            context.AddSource($"{aggregate.Name}.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}

public record AggregateConstructorModel : TemplateModelBase
{
    public INamedTypeSymbol Aggregate { get; init; }
    public ITypeSymbol StateType { get; init; }

    public AggregateConstructorModel(INamedTypeSymbol aggregate, ITypeSymbol stateType)
        : base(aggregate.ContainingNamespace, aggregate.Name, [aggregate, stateType], ["ES.Core.Aggregates", "Marten.Events"])
    {
        Aggregate = aggregate;
        StateType = stateType;
    }
}
