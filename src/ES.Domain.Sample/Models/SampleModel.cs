using ES.Core.Models;

namespace ES.Domain.Sample.Models;

public record SampleModel : VersionedModel
{
    public int Value { get; init; }
}
