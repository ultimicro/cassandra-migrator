namespace CassandraMigrator.DataStaxClient
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cassandra;

    public sealed class Connection : IConnection, IDisposable
    {
        private const int RetryCount = 3; // Number to retry, not including the first attempt.

        private readonly ISession session;
        private readonly bool sessionOwnership;
        private readonly Cluster? cluster;

        [Obsolete("Use CreateAsync instead.")]
        public Connection(ISession session, bool leaveOpen)
        {
            this.session = session;
            this.sessionOwnership = !leaveOpen;
        }

        private Connection(ISession session)
        {
            this.session = session;
            this.sessionOwnership = false;
        }

        private Connection(ISession session, Cluster cluster)
        {
            this.session = session;
            this.cluster = cluster;
            this.sessionOwnership = true;
        }

        public static async Task<Connection> ConnectAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var cluster = Cluster.Builder().WithConnectionString(connectionString).Build();

            try
            {
                return await ConnectAsync(cluster, true, cancellationToken);
            }
            catch
            {
                cluster.Dispose();
                throw;
            }
        }

        public static Task<Connection> ConnectAsync(string address, string keyspace, CancellationToken cancellationToken = default)
        {
            return ConnectAsync(address, keyspace, null, null, cancellationToken);
        }

        public static Task<Connection> ConnectAsync(
            string address,
            string keyspace,
            string? username,
            string? password,
            CancellationToken cancellationToken = default)
        {
            return ConnectAsync(address, keyspace, username, password, true, cancellationToken);
        }

        public static async Task<Connection> ConnectAsync(
            string address,
            string keyspace,
            string? username,
            string? password,
            bool createKeyspace,
            CancellationToken cancellationToken = default)
        {
            var builder = Cluster.Builder().AddContactPoint(address).WithDefaultKeyspace(keyspace);

            if (username != null && password != null)
            {
                builder.WithCredentials(username, password);
            }

            var cluster = builder.Build();

            try
            {
                return await ConnectAsync(cluster, createKeyspace, cancellationToken);
            }
            catch
            {
                cluster.Dispose();
                throw;
            }
        }

        public static async Task<Connection> CreateAsync(ISession session, CancellationToken cancellationToken = default)
        {
            await CreateMigrationTableAsync(session, cancellationToken);

            return new Connection(session);
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

        private static async Task<Connection> ConnectAsync(Cluster cluster, bool createKeyspace, CancellationToken cancellationToken = default)
        {
            ISession session;

            for (var i = 0; ; i++)
            {
                try
                {
                    if (createKeyspace)
                    {
                        var replication = ReplicationStrategies.CreateSimpleStrategyReplicationProperty(1);
                        session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists(replication);
                    }
                    else
                    {
                        session = await cluster.ConnectAsync();
                    }
                }
                catch (NoHostAvailableException)
                {
                    if (i < RetryCount)
                    {
                        await Task.Delay(1000 * 30 * (i + 1), cancellationToken);
                        continue;
                    }

                    throw;
                }

                break;
            }

            try
            {
                await CreateMigrationTableAsync(session, cancellationToken);

                return new Connection(session, cluster);
            }
            catch
            {
                session.Dispose();
                throw;
            }
        }

        private static Task CreateMigrationTableAsync(ISession session, CancellationToken cancellationToken = default)
        {
            var cql = "CREATE TABLE IF NOT EXISTS migrations (major SMALLINT, minor SMALLINT, time TIMESTAMP, PRIMARY KEY (major, minor))";

            return session.ExecuteAsync(new SimpleStatement(cql));
        }
    }
}
