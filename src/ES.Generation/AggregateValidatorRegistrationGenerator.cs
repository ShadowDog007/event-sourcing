using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generates ServiceCollectionExtensions to register all of the defined aggregate command validators
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AggregateValidatorRegistrationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateAggregateValidatorProvider();

        context.RegisterSourceOutput(provider.Collect().Combine(context.CompilationProvider),
            (ctx, t) => GenerateValidatorRegistrations(ctx, t.Right, t.Left));
    }

    private void GenerateValidatorRegistrations(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol> validators)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var services = new ServiceCollectionExtensionBuilder(compilation, "AggregateCommandValidators");
            var symbols = new AggregateSymbols(compilation);

            foreach (var grouping in validators
                .Select(validator => (Service: symbols.GetAggregateValidatorInterface(validator), Implementation: validator))
                .GroupBy<(INamedTypeSymbol Service, INamedTypeSymbol Implementation), ITypeSymbol >(r => r.Service.TypeParameters[0]! /* Aggregate Type */, SymbolEqualityComparer.Default)
                .OrderBy(g => g.Key!.ToString()))
            {
                services.AddComment($"Validators for {grouping.Key.Name}");
                foreach (var (service, implementation) in grouping.OrderBy(r => r.Service.TypeParameters[1].Name /* Command type */))
                {
                    services.AddScoped(service, implementation);
                }
                services.AddEmptyLine();
            }

            context.AddServiceCollectionExtensionsSource(services);
        }
        catch (Exception ex)
        {
            context.AddSource($"ServiceCollectionExtensions.AddAggregateCommandValidators.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}
