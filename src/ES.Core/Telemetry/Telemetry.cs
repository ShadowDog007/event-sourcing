using System.Diagnostics;

namespace ES.Core.Telemetry;

public static class Telemetry
{
    public static ActivitySource Source { get; } = new ActivitySource("ES.Core");
}
