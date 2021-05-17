namespace CassandraMigrator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public sealed class Migrator : IMigrator
    {
        private readonly IConnection connection;
        private readonly ILogger<Migrator> logger;

        public Migrator(IConnection connection, ILogger<Migrator> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public async Task<IEnumerable<MigrationHistory>> ExecuteAsync(IMigrationProvider provider, CancellationToken cancellationToken = default)
        {
            // Find starting point.
            await using var latest = await provider.GetLatestAsync(cancellationToken);

            if (latest == null)
            {
                this.logger.LogWarning("No any available migrations to apply.");
                return Enumerable.Empty<MigrationHistory>();
            }

            var histories = await this.connection.ListHistoryAsync(latest.Version.Major, cancellationToken);
            var current = histories.LastOrDefault();

            if (current != null)
            {
                // Make sure the latest migration to apply is really latest.
                if ((await this.connection.ListHistoryAsync(latest.Version.Major + 1, cancellationToken)).Any() || current.Version > latest.Version)
                {
                    this.logger.LogWarning("Cannot apply the latest migration {Latest} due to the database is already applied by newer version.", latest);
                    return Enumerable.Empty<MigrationHistory>();
                }

                if (current.Version == latest.Version)
                {
                    this.logger.LogInformation("The database is already in the latest version {Current}.", current.Version);
                    return Enumerable.Empty<MigrationHistory>();
                }
            }
            else
            {
                // Find the latest major version of the database.
                for (var i = latest.Version.Major - 1; i >= 0; i--)
                {
                    histories = await this.connection.ListHistoryAsync(i, cancellationToken);

                    if (histories.Any())
                    {
                        break;
                    }
                }

                current = histories.LastOrDefault();
            }

            // Execute migrations.
            var result = new List<MigrationHistory>();

            if (current == null)
            {
                this.logger.LogInformation("No any migrations have been applied to the database yet.");
            }
            else
            {
                this.logger.LogInformation("The current database version is {Current}.", current.Version);
            }

            for (; ;)
            {
                // Find next migration.
                IMigration? migration;

                if (current == null)
                {
                    // The database is a brand new.
                    migration = (await provider.GetAsync(new(0, 0), cancellationToken))!;
                }
                else if (current.Version.Minor == 0)
                {
                    // The database is in the first (0) minor version. We are safe to jump to next major version to speed up.
                    migration = await provider.GetAsync(current.Version.IncreaseMajor(), cancellationToken);

                    if (migration == null)
                    {
                        // No next major version is available, switch to next minor version instead.
                        migration = await provider.GetAsync(current.Version.IncreaseMinor(), cancellationToken);

                        if (migration == null)
                        {
                            // No more migrations.
                            break;
                        }
                    }
                }
                else
                {
                    // The database is in the middle of major version. Apply all remaining minor versions.
                    migration = await provider.GetAsync(current.Version.IncreaseMinor(), cancellationToken);

                    if (migration == null)
                    {
                        // No more minor versions. Try to skip the next major version if it is possible to speed up.
                        migration = await provider.GetAsync(current.Version.IncreaseMajor().IncreaseMajor(), cancellationToken);

                        if (migration == null)
                        {
                            // The next major version is the latest one. Apply all its minor versions.
                            migration = await provider.GetAsync(new(current.Version.Major + 1, 1), cancellationToken);

                            if (migration == null)
                            {
                                // No more migrations.
                                break;
                            }
                        }
                    }
                }

                // Execute migration.
                await using (migration)
                {
                    this.logger.LogInformation("Applying migration: {Migration}", migration);

                    foreach (var statement in await migration.ListStatementAsync(cancellationToken))
                    {
                        await this.connection.ExecuteMigrationAsync(statement);
                    }

                    current = new MigrationHistory(migration.Version, DateTime.Now);
                }

                await this.connection.CreateHistoryAsync(current);
                result.Add(current);
            }

            this.logger.LogInformation("{Count} migration(s) has been applied successfully.", result.Count);

            return result;
        }
    }
}
