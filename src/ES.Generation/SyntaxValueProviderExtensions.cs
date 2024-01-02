using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace ES.Generation;

public static class SyntaxValueProviderExtensions
{
    public static IncrementalValuesProvider<IMethodSymbol> CreateServiceCollectionExtensionsProvider(this SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(
            static (s, _) => s is MethodDeclarationSyntax,
            static (ctx, token) =>
            {
                var node = (MethodDeclarationSyntax)ctx.Node;
                var method = ctx.SemanticModel.GetDeclaredSymbol(node, token);

                if (method is not { IsStatic: true, Parameters.Length: 1 or 2 })
                {
                    return null!;
                }

                return method.Name.StartsWith("Add")
                    && node.ParameterList.Parameters[0].ChildNodesAndTokens()
                        .Any(t => t.IsKind(SyntaxKind.ThisKeyword))
                    && method.Parameters[0] is { Type.Name: "IServiceCollection" }
                    && (method.Parameters.Length == 1 || method.Parameters[1] is { Type.Name: "IWebHostEnvironment" })
                    ? method
                    : null!;
            }
        )
            .Where(m => m is not null);
    }

    public static IncrementalValuesProvider<INamedTypeSymbol> CreateAggregateSyntaxProvider(this SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(
            static (s, _) => s is ClassDeclarationSyntax,
            static (ctx, token) =>
            {
                var node = (ClassDeclarationSyntax)ctx.Node;
                var syntax = ctx.SemanticModel.GetDeclaredSymbol(node, token);

                if (syntax is { BaseType.Name: "AggregateBase" })
                    return syntax!;
                return null!;
            }
        )
            .Where(s => s is not null);
    }

    public static IncrementalValuesProvider<INamedTypeSymbol> CreateAggregateValidatorProvider(this SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(
            static (s, _) => s is ClassDeclarationSyntax,
            static (ctx, token) =>
            {
                var node = (ClassDeclarationSyntax)ctx.Node;
                var syntax = ctx.SemanticModel.GetDeclaredSymbol(node, token);

                if (syntax is not null && syntax.AllInterfaces.Any(i => i is { Name: "IAggregateValidator" }))
                {
                    return syntax;
                }
                return null!;
            }
        )
            .Where(t => t is not null);
    }

    public static IncrementalValuesProvider<INamedTypeSymbol> CreateProjectionSyntaxProvider(this SyntaxValueProvider provider)
    {
        return provider.CreateSyntaxProvider(
            static (s, _) => s is ClassDeclarationSyntax,
            static (ctx, token) =>
            {
                var node = (ClassDeclarationSyntax)ctx.Node;
                var syntax = ctx.SemanticModel.GetDeclaredSymbol(node, token);

                if (syntax?.BaseType is { Name: "AggregateProjection", TypeArguments.Length: 1 }
                    && syntax.BaseType.TypeArguments[0] is INamedTypeSymbol)
                    return syntax;
                return null!;
            }
        )
            .Where(s => s is not null);
    }
}
