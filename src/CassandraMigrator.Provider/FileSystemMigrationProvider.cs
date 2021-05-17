namespace CassandraMigrator.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class FileSystemMigrationProvider : IMigrationProvider
    {
        private readonly MigrationInfo[] migrations;

        public FileSystemMigrationProvider(string directory)
        {
            // List all migration files.
            var migrations = new SortedSet<MigrationInfo>();
            var path = new DirectoryInfo(directory);

            foreach (var file in path.EnumerateFiles("*.cql"))
            {
                // Get file version.
                var name = file.Name;
                var sep = name.IndexOf('.', StringComparison.Ordinal);
                var ext = name.LastIndexOf('.');

                if (sep == ext || sep == 0)
                {
                    throw new ArgumentException($"File {name} has invalid name.", nameof(directory));
                }

                int major, minor;

                try
                {
                    major = short.Parse(name.Substring(0, sep), NumberStyles.None, CultureInfo.InvariantCulture);
                    minor = short.Parse(name.Substring(sep + 1, ext - sep - 1), NumberStyles.None, CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    throw new ArgumentException($"File {name} has invalid name.", nameof(directory), ex);
                }

                // Store metadata.
                MigrationVersion version;

                try
                {
                    version = new MigrationVersion(major, minor);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    throw new ArgumentException($"File {name} has invalid name.", nameof(directory), ex);
                }

                if (!migrations.Add(new(version, file.FullName)))
                {
                    throw new ArgumentException($"File {name} has duplicated version with other file.", nameof(directory));
                }
            }

            // Create lookup table.
            var i = 0;

            this.migrations = new MigrationInfo[migrations.Count];

            foreach (var info in migrations)
            {
                this.migrations[i++] = info;
            }
        }

        public ValueTask<IMigration?> GetAsync(MigrationVersion version, CancellationToken cancellationToken = default)
        {
            MigrationInfo? info;

            if (this.migrations.Length == 0)
            {
                info = null;
            }
            else
            {
                var index = Array.BinarySearch(this.migrations, new(version, string.Empty));

                if (index >= 0)
                {
                    info = this.migrations[index];
                }
                else
                {
                    index = ~index;

                    if (index == this.migrations.Length)
                    {
                        // The target version is larger than any available versions.
                        info = null;
                    }
                    else if (this.migrations[index].Version.Major != version.Major)
                    {
                        // No more migrations for the target major version.
                        info = null;
                    }
                    else
                    {
                        // Missing migration in the target major version.
                        throw new MigrationNotFoundException();
                    }
                }
            }

            return new ValueTask<IMigration?>(info != null ? new MigrationFile(info.Version, info.File) : null);
        }

        public ValueTask<IMigration?> GetLatestAsync(CancellationToken cancellationToken = default)
        {
            var index = this.migrations.Length - 1;
            var migration = (index == -1) ? null : new MigrationFile(this.migrations[index].Version, this.migrations[index].File);

            return new ValueTask<IMigration?>(migration);
        }

        private sealed class MigrationInfo : IComparable<MigrationInfo>
        {
            public MigrationInfo(MigrationVersion version, string file)
            {
                this.Version = version;
                this.File = file;
            }

            public MigrationVersion Version { get; }

            public string File { get; }

            public int CompareTo(MigrationInfo? other) => other == null ? 1 : this.Version.CompareTo(other.Version);
        }
    }
}
