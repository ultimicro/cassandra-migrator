namespace CassandraMigrator
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMigrationProvider
    {
        ValueTask<IMigration?> GetLatestAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets migration for the specified version.
        /// </summary>
        /// <param name="version">
        /// The version of the migration to retrieve.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The migration for the version specified by <paramref name="version"/> or <c>null</c>if the specified version is not available.
        /// </returns>
        /// <exception cref="MigrationNotFoundException">
        /// <paramref name="version"/> is less than or equal to the latest minor version but it is not available.
        /// </exception>
        ValueTask<IMigration?> GetAsync(MigrationVersion version, CancellationToken cancellationToken = default);
    }
}
