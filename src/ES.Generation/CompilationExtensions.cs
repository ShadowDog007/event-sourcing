using Microsoft.CodeAnalysis;

namespace ES.Generation;

public static class CompilationExtensions
{
    public static string GetSimpleAssemblyName(this Compilation compilation)
        => compilation.AssemblyName!.Replace("ES.", "").Replace(".", "");
}
