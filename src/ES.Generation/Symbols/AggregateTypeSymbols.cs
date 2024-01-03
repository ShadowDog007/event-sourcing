using Microsoft.CodeAnalysis;

namespace ES.Generation.Symbols;

public record AggregateTypeSymbols : AggregateSymbols
{
    public INamedTypeSymbol AggregateType { get; init; }
    public ITypeSymbol AggregateStateType { get; init; }
    public IReadOnlyList<(IMethodSymbol Handler, INamedTypeSymbol CommandType)> CommandHandlers { get; init; }

    public AggregateTypeSymbols(Compilation compilation, INamedTypeSymbol aggregateType)
        : this(new AggregateSymbols(compilation), aggregateType)
    {
    }

    public AggregateTypeSymbols(AggregateSymbols baseSymbols, INamedTypeSymbol aggregateType) : base(baseSymbols)
    {
        AggregateType = aggregateType;
        AggregateStateType = aggregateType.AllInterfaces.First(i => i is { Name: "IAggregate", IsGenericType: true, TypeArguments.Length: 1 })
            .TypeArguments[0];

        CommandHandlers =
            [
                .. aggregateType.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Select(m => TryGetHandlerCommandType(m, out var commandType) ? (m, commandType) : (null!, null!))
                    .Where(h => h is (not null, not null))
            ];
    }

    public INamedTypeSymbol MakeCommandHandlerType(INamedTypeSymbol commandType)
    {
        return AggregateCommandHandlerType.Construct(AggregateType, commandType);
    }

    public INamedTypeSymbol MakeCommandConsumerType(INamedTypeSymbol commandType)
    {
        return ConsumerType.Construct(AggregateType, AggregateStateType, commandType);
    }

    public bool IsCommandHandlerMethod(IMethodSymbol method) => TryGetHandlerCommandType(method, out _);

    public bool TryGetHandlerCommandType(IMethodSymbol method, out INamedTypeSymbol commandType)
    {
        foreach (var parameter in method.Parameters)
        {
            if (parameter.Type is INamedTypeSymbol type && type.EnumerateBaseTypes().Any(t => SymbolEqualityComparer.Default.Equals(CommandType, t)))
            {
                commandType = type;
                return true;
            }
        }

        commandType = null!;
        return false;
    }
}


