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

        public static readonly Option<string> UsernameOption = new(
            new[] { "-u", "--username" },
            "Username to authenticate");

        public static readonly Option<string> PasswordOption = new(
            new[] { "-p", "--password" },
            "Password to authenticate");

        public DatabaseCommand()
            : base("database", "Manage database")
        {
            this.Add(AddressOption);
            this.Add(KeyspaceOption);
            this.Add(UsernameOption);
            this.Add(PasswordOption);
            this.Add(new UpdateCommand());
        }
    }
}
