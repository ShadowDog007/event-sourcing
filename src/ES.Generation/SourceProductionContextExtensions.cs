using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace ES.Generation;

public static class SourceProductionContextExtensions
{
    public static void AddRenderedSource<TModel>(this SourceProductionContext context, TModel model, Action<TModel, TextWriter, string?> renderer)
        where TModel : TemplateModelBase
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        Templates.RenderAutoGenMessage("", writer, null);
        renderer(model, writer, null);

        context.AddSource($"{model.Name}.g.cs", SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true));
    }

    public static void AddServiceCollectionExtensionsSource(this SourceProductionContext context,
        ServiceCollectionExtensionsModel model)
    {
        context.AddRenderedSource(model, Templates.RenderServiceCollectionExtensions);
    }

    public static void AddServiceCollectionExtensionsSource(this SourceProductionContext context,
        ServiceCollectionExtensionBuilder builder)
    {
        context.AddRenderedSource(builder.ToModel(), Templates.RenderServiceCollectionExtensions);
    }
}


