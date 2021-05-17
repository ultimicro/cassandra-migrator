namespace CassandraMigrator
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMigrator
    {
        Task<IEnumerable<MigrationHistory>> ExecuteAsync(IMigrationProvider provider, CancellationToken cancellationToken = default);
    }
}
