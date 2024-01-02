using Microsoft.CodeAnalysis;

namespace ES.Generation.Symbols;

public record AggregateSymbols
{
    public INamedTypeSymbol CommandType { get; init; }
    public INamedTypeSymbol AggregateCommandHandlerType { get; init; }
    public INamedTypeSymbol ConsumerType { get; init; }

    public AggregateSymbols(Compilation compilation)
    {
        CommandType = compilation.GetTypeByMetadataName("ES.Core.Messages.Command")!;
        AggregateCommandHandlerType = compilation.GetTypeByMetadataName("ES.Core.Aggregates.IAggregateHandler`2")!;
        ConsumerType = compilation.GetTypeByMetadataName("ES.Core.Aggregates.AggregateCommandConsumer`3")!;
    }

    public INamedTypeSymbol GetAggregateValidatorInterface(INamedTypeSymbol implementation)
    {
        return implementation.AllInterfaces.First(i => i is { IsGenericType: true, Name: "IAggregateValidator", TypeParameters.Length: 2 });
    }
}
