namespace CassandraMigrator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMigration : IDisposable, IAsyncDisposable
    {
        MigrationVersion Version { get; }

        /// <summary>
        /// Gets CQL statements for this migration.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// CQL statements to execute.
        /// </returns>
        ValueTask<IEnumerable<string>> ListStatementAsync(CancellationToken cancellationToken = default);
    }
}
