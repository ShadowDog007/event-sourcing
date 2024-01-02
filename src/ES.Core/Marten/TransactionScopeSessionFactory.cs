using Marten;
using Marten.Services;

namespace ES.Core.Marten;

public class TransactionScopeSessionFactory(IDocumentStore store) : SessionFactoryBase(store)
{
    public override SessionOptions BuildOptions() => SessionOptions.ForCurrentTransaction().WithTracking(DocumentTracking.None);
}
