namespace CassandraMigrator.DataStaxClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cassandra;

    public sealed class Connection : IConnection, IDisposable
    {
        private readonly ISession session;
        private readonly bool sessionOwnership;
        private readonly Cluster? cluster;

        public Connection(ISession session, bool leaveOpen)
        {
            this.session = session;
            this.sessionOwnership = !leaveOpen;
        }

        private Connection(ISession session, Cluster cluster)
        {
            this.session = session;
            this.cluster = cluster;
            this.sessionOwnership = true;
        }

        public static async Task<Connection> ConnectAsync(string address, string keyspace, CancellationToken cancellationToken = default)
        {
            var cluster = Cluster.Builder().AddContactPoint(address).Build();

            try
            {
                var session = await cluster.ConnectAsync();

                try
                {
                    session.CreateKeyspaceIfNotExists(keyspace);
                    session.ChangeKeyspace(keyspace);

                    await session.ExecuteAsync(new SimpleStatement("CREATE TABLE IF NOT EXISTS migrations (major SMALLINT, minor SMALLINT, time TIMESTAMP, PRIMARY KEY (major, minor))"));

                    return new Connection(session, cluster);
                }
                catch
                {
                    session.Dispose();
                    throw;
                }
            }
            catch
            {
                cluster.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (this.sessionOwnership)
            {
                this.session.Dispose();
            }

            this.cluster?.Dispose();
        }

        public async Task CreateHistoryAsync(MigrationHistory history, CancellationToken cancellationToken = default)
        {
            var prepare = await this.session.PrepareAsync("INSERT INTO migrations (major, minor, time) VALUES (?, ?, ?)");
            var statement = prepare.Bind(
                Convert.ToInt16(history.Version.Major),
                Convert.ToInt16(history.Version.Minor),
                history.AppliedTime.ToUniversalTime());

            await this.session.ExecuteAsync(statement);
        }

        public Task ExecuteMigrationAsync(string statement, CancellationToken cancellationToken = default)
        {
            return this.session.ExecuteAsync(new SimpleStatement(statement));
        }

        public async Task<IEnumerable<MigrationHistory>> ListHistoryAsync(int majorVersion, CancellationToken cancellationToken = default)
        {
            var prepare = await this.session.PrepareAsync("SELECT major, minor, time FROM migrations WHERE major = ? ORDER BY minor ASC");
            var statement = prepare.Bind(Convert.ToInt16(majorVersion));
            var histories = new List<MigrationHistory>();

            foreach (var row in await this.session.ExecuteAsync(statement))
            {
                var version = new MigrationVersion(row.GetValue<short>("major"), row.GetValue<short>("minor"));
                var time = row.GetValue<DateTimeOffset>("time");

                histories.Add(new MigrationHistory(version, time.UtcDateTime));
            }

            return histories;
        }
    }
}
