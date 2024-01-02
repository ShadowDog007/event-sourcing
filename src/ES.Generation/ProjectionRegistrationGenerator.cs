using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ES.Generation;

/// <summary>
/// Generates <see cref="IConfigureMarten"/> implementation to register all of the aggregate projections
/// Also generates ServiceCollectionExtensions to register this implementation
/// </summary>
[Generator(LanguageNames.CSharp)]
public class ProjectionRegistrationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateProjectionSyntaxProvider();

        context.RegisterSourceOutput(provider.Collect().Combine(context.CompilationProvider),
            (ctx, t) => GenerateProjectionRegistrations(ctx, t.Right, t.Left));
    }

    private void GenerateProjectionRegistrations(SourceProductionContext context, Compilation compilation, ImmutableArray<INamedTypeSymbol> projections)
    {
        if (compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            return;

        try
        {
            var model = new ProjectionRegistrationModel(compilation, projections);
            context.AddRenderedSource(model, Templates.RenderProjectionRegistrations);
        }
        catch (Exception ex)
        {
            context.AddSource($"{compilation.GetSimpleAssemblyName()}ProjectionRegistrations.g.cs", $"/* Exception raised during source generation:\n\n {ex} */");
        }


        var services = new ServiceCollectionExtensionBuilder(compilation, $"ProjectionRegistrations");
        try
        {

            services.AddNamespaces(["Marten"]);
            services.AddTransient("IConfigureMarten", $"{compilation.GetSimpleAssemblyName()}ProjectionRegistrations");

            context.AddServiceCollectionExtensionsSource(services);
        }
        catch (Exception ex)
        {
            context.AddSource(services.FileName, $"/* Exception raised during source generation:\n\n {ex} */");
        }
    }
}

public record ProjectionRegistrationModel : TemplateModelBase
{
    public IEnumerable<INamedTypeSymbol> Projections { get; init; }

    public ProjectionRegistrationModel(Compilation compilation, IEnumerable<INamedTypeSymbol> projections)
        : base(compilation.AssemblyName!, $"{compilation.GetSimpleAssemblyName()}ProjectionRegistrations",
            [..projections])
    {
        Projections = projections;
    }
}
