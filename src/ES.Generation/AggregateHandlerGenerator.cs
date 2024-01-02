using ES.Generation.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generates <see cref="IAggregateHandler{TAggregate, TCommand}"/> implementations for all of the commands supported by the aggregate
/// Also generates ServiceCollectionExtensions to register the handlers.
/// 
/// Automatically includes validation logic if required, and resolution of required services.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class AggregateHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var aggregates = context.SyntaxProvider.CreateAggregateSyntaxProvider();
        var validators = context.SyntaxProvider.CreateAggregateValidatorProvider();

        var provider = aggregates.Collect()
            .Combine(validators.Collect())
            .SelectMany((items, _) =>
                items.Left.GroupJoin(items.Right, a => a,
                    v => v.AllInterfaces.First(i => i is { Name: "IAggregateValidator" })
                        .TypeArguments[0],
                    (a, validators) => (Aggregate: a, Validators: validators.ToImmutableArray()),
                    SymbolEqualityComparer.Default)
                .ToImmutableArray()
            );

        context.RegisterSourceOutput(provider.Combine(context.CompilationProvider),
            (ctx, t) => GenerateHandlers(ctx, t.Right, t.Left.Aggregate, t.Left.Validators));
    }

    private void GenerateHandlers(SourceProductionContext context, Compilation compilation, INamedTypeSymbol aggregate,
        ImmutableArray<INamedTypeSymbol> validators)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var symbols = new AggregateTypeSymbols(compilation, aggregate);

            var services = new ServiceCollectionExtensionBuilder(compilation, $"AggregateCommandHandlers");

            foreach (var (handler, commandType) in symbols.CommandHandlers)
            {
                var model = new AggregateHandlerModel(aggregate, commandType, handler,
                    validators.FirstOrDefault(v => SymbolEqualityComparer.Default.Equals(
                        v.AllInterfaces.First(i => i is { Name: "IAggregateValidator" }).TypeArguments[1], commandType)));
                var serviceType = symbols.MakeCommandHandlerType(commandType);

                // If there are no constructor parameters then we can register this as singleton
                if (!model.ConstructorParameters.Any())
                    services.AddSingleton(serviceType, model.Name);
                else
                    services.AddScoped(serviceType, model.Name);


                context.AddRenderedSource(model, Templates.RenderAggregateHandler);
            }

            context.AddServiceCollectionExtensionsSource(services);
        }
        catch (Exception ex)
        {
            context.AddSource($"{nameof(AggregateHandlerGenerator)}.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}

public record AggregateHandlerModel(
        INamedTypeSymbol Aggregate,
        INamedTypeSymbol Command,
        IMethodSymbol Handler,
        INamedTypeSymbol? CommandValidator
    ) : TemplateModelBase(Aggregate.ContainingNamespace, $"{Aggregate.Name}{Command.Name}Handler",
        [Aggregate, Command, ..Handler.Parameters.Select(p => p.Type), CommandValidator],
        (CommandValidator is not null) ? ["FluentValidation","ES.Core.Aggregates.Validation"] : [])
{
    /// <summary>
    /// If the HandleAsync method must use the async modifier
    /// </summary>
    /// <remarks>If we have to await validation</remarks>
    public bool IsHandlerAwaitable => Handler.ReturnType.Name is "Task" or "ValueTask";
    public bool HasValidator => CommandValidator is not null;

    public string AwaitOrReturn => HasValidator && IsHandlerAwaitable ? "await" : IsHandlerAwaitable ? "return" : "";

    public string MethodCall => $"aggregate.{Handler.Name}({string.Join(", ", HandlerParameters)})";

    public IEnumerable<(INamedTypeSymbol Type, string Name)> ConstructorParameters
        => Handler.Parameters.Where(IsConstructorParameter)
            .Where(p => p.Type is INamedTypeSymbol)
            .Select(p => ((INamedTypeSymbol)p.Type, p.Name))
            .Concat(CommandValidator is not null ? new[] { (ValidatorInterfaceType!, "validator") } : []);

    public INamedTypeSymbol? ValidatorInterfaceType
        => CommandValidator?.AllInterfaces.First(i => i is { IsGenericType: true, Name: "IAggregateValidator", TypeParameters.Length: 2 });

    public IEnumerable<string> HandlerParameters => Handler.Parameters
        .Select(parameter => TryGetKnownParameterName(parameter, out var parameterName) ? parameterName : parameter.Name);

    public bool TryGetKnownParameterName(IParameterSymbol parameter, out string parameterName)
    {
        var name = parameter.Type.Name switch
        {
            "IServiceProvider" => "serviceProvider",
            "CancellationToken" => "cancellationToken",
            _ => SymbolEqualityComparer.Default.Equals(Command, parameter.Type) ? "command" : null!
        };

        parameterName = name!;
        return name != null;
    }


    private bool IsConstructorParameter(IParameterSymbol parameter) => !TryGetKnownParameterName(parameter, out _);
}
