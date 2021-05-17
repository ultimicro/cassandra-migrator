namespace CassandraMigrator
{
    using System;

    public sealed class MigrationHistory
    {
        public MigrationHistory(MigrationVersion version, DateTime appliedTime)
        {
            this.Version = version;
            this.AppliedTime = appliedTime;
        }

        public MigrationVersion Version { get; }

        public DateTime AppliedTime { get; }
    }
}
