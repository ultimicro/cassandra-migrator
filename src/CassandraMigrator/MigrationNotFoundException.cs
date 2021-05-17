namespace CassandraMigrator
{
    using System;

    public sealed class MigrationNotFoundException : Exception
    {
        public MigrationNotFoundException()
        {
        }

        public MigrationNotFoundException(string? message)
            : base(message)
        {
        }

        public MigrationNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
