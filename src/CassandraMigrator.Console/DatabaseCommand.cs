namespace CassandraMigrator.Console
{
    using System.CommandLine;
    using CassandraMigrator.Console.Database;

    internal sealed class DatabaseCommand : Command
    {
        public static readonly Option<string> AddressOption = new(
            new[] { "-a", "--address" },
            () => "127.0.0.1",
            "Address of the target database");

        public static readonly Option<string> KeyspaceOption = new(
            new[] { "-k", "--keyspace" },
            "Target keyspace");

        public DatabaseCommand()
            : base("database", "Manage database")
        {
            this.Add(AddressOption);
            this.Add(KeyspaceOption);
            this.Add(new UpdateCommand());
        }
    }
}
