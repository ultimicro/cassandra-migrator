namespace CassandraMigrator
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConnection
    {
        Task CreateHistoryAsync(MigrationHistory history, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets migration histories for the specified major version.
        /// </summary>
        /// <param name="majorVersion">
        /// The major version to query for histories.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// Migration histories for the specified <paramref name="majorVersion"/>, ordered in ascending by minor version.
        /// </returns>
        Task<IEnumerable<MigrationHistory>> ListHistoryAsync(int majorVersion, CancellationToken cancellationToken = default);

        Task ExecuteMigrationAsync(string statement, CancellationToken cancellationToken = default);
    }
}
