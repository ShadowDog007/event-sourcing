using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ES.Core;

public static class ConfigurationManagerExtensions
{
    public static IConfigurationManager AddEventSourcingConfig(this IConfigurationManager configuration, IWebHostEnvironment environment)
    {
        var original = configuration.GetFileProvider();
        // core json files will always be in the bin directory
        configuration.SetBasePath(AppContext.BaseDirectory);

        // Add core configuration
        configuration.AddJsonFile("appsettings.core.json")
            .MoveSourceBeforeJson("appsettings.json");
        configuration.AddJsonFile($"appsettings.core.{environment.EnvironmentName}.json", true)
            .MoveSourceBeforeJson($"appsettings.{environment.EnvironmentName}.json");

        // Restore original file provider
        configuration.SetFileProvider(original);


        return configuration;
    }

    private static IConfigurationBuilder MoveSourceBeforeJson(this IConfigurationBuilder builder, string filePath)
    {
        return builder.MoveSource(builder.FindJsonIndex(filePath));
    }

    private static int FindJsonIndex(this IConfigurationBuilder builder, string fileName)
    {
        for (var i = 0; i < builder.Sources.Count; ++i)
        {
            if (builder.Sources[i] is JsonConfigurationSource source
                && string.Equals(source.Path, fileName, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static IConfigurationBuilder MoveSource(this IConfigurationBuilder builder, int toIndex)
    {
        if (toIndex == -1)
            return builder;

        var fromIndex = builder.Sources.Count - 1;
        var source = builder.Sources[fromIndex];
        builder.Sources.RemoveAt(fromIndex);
        builder.Sources.Insert(toIndex, source);
        return builder;
    }
}
