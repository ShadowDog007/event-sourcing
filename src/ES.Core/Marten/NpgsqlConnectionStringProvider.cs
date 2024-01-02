using Microsoft.Extensions.Options;
using Npgsql;

namespace ES.Core.Marten;

public class NpgsqlConnectionStringProvider : IConnectionStringProvider, IDisposable
{
    private readonly IDisposable? _optionsListener;
    public string ConnectionString { get; private set; }

    public NpgsqlConnectionStringProvider(IOptionsMonitor<NpgsqlConnectionStringBuilder> optionsMonitor)
    {
        ConnectionString = optionsMonitor.CurrentValue.ConnectionString;
        _optionsListener = optionsMonitor.OnChange(opt =>
        {
            ConnectionString = opt.ConnectionString;
        });
    }

    public void Dispose()
    {
        _optionsListener?.Dispose();
    }
}
