#if NET
using Marten.Schema;
#endif

namespace ES.Core.Models;

public record VersionedModel
{
#if NET
    [Identity]
#endif
    public Guid StreamId { get; init; }
    public DateTimeOffset CreatedTime { get; init; }
    public DateTimeOffset ModifiedTime { get; init; }
    public long Version { get; init; }
}
