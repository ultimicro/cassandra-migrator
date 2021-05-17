namespace CassandraMigrator
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class Migration : IMigration
    {
        protected Migration(MigrationVersion version)
        {
            this.Version = version;
        }

        public MigrationVersion Version { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await this.DisposeAsyncCore();
            this.Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets CQL statements for this migration.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// CQL statements to execute.
        /// </returns>
        public abstract ValueTask<IEnumerable<string>> ListStatementAsync(CancellationToken cancellationToken = default);

        public override string ToString() => this.Version.ToString();

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual ValueTask DisposeAsyncCore() => default;
    }
}
