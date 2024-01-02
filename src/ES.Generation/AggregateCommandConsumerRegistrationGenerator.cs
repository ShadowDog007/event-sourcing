using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generates ServiceCollectionExtension method to register
/// implementations of <see cref="ES.Core.Aggregates.AggregateCommandConsumer{TAggregate, TState, TCommand}"/>
/// as MassTransit consumers
/// </summary>
[Generator]
public class AggregateCommandConsumerRegistrationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var aggregates = context.SyntaxProvider.CreateAggregateSyntaxProvider();

        var provider = aggregates.Collect();

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider),
            (ctx, t) => GenerateCommandConsumerRegistrations(ctx, t.Right, t.Left));
    }

    private void GenerateCommandConsumerRegistrations(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol> aggregates)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var baseSymbols = new AggregateSymbols(compilation);
            var aggregateSymbols = aggregates.Select(a => new AggregateTypeSymbols(baseSymbols, a)).ToList();

            var services = new ServiceCollectionExtensionBuilder(compilation, $"AggregateCommandConsumers");
            services.AddNamespaces(["MassTransit.Configuration"]);

            var first = true;

            foreach (var aggregate in aggregateSymbols)
            {
                services.AddComment($"Command consumers for {aggregate.AggregateType}");
                foreach (var (handler, commandType) in aggregate.CommandHandlers)
                {
                    var consumerType = aggregate.MakeCommandConsumerType(commandType);
                    services.Add("RegisterConsumer", consumerType);
                }

                if (first) first = false;
                else services.AddEmptyLine();
            }

            context.AddServiceCollectionExtensionsSource(services);
        }
        catch (Exception ex)
        {
            context.AddSource($"ServiceCollectionExtensions.AddAggregateCommandConsumers.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}
