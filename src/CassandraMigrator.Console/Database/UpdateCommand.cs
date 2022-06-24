namespace CassandraMigrator.Console.Database
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;
    using CassandraMigrator.DataStaxClient;
    using CassandraMigrator.Provider;
    using Microsoft.Extensions.Logging;

    internal sealed class UpdateCommand : Command, ICommandHandler
    {
        public static readonly Option<DirectoryInfo> DirectoryOption = new(
            new[] { "-d", "--directory" },
            "Path to directory contains migrations");

        public static readonly Option<bool> NoCreateKeyspace = new(
            new[] { "--no-create-keyspace" },
            "Don't attempt to create keyspace");

        public UpdateCommand()
            : base("update", "Apply migrations to the database")
        {
            this.Add(DirectoryOption);
            this.Add(NoCreateKeyspace);
            this.Handler = this;
        }

        public int Invoke(InvocationContext context) => this.InvokeAsync(context).Result;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            // Get options.
            var address = context.ParseResult.GetValueForOption(DatabaseCommand.AddressOption);
            var keyspace = context.ParseResult.GetValueForOption(DatabaseCommand.KeyspaceOption);
            var username = context.ParseResult.GetValueForOption(DatabaseCommand.UsernameOption);
            var password = context.ParseResult.GetValueForOption(DatabaseCommand.PasswordOption);
            var directory = context.ParseResult.GetValueForOption(DirectoryOption);
            var createKeyspace = context.ParseResult.GetValueForOption(NoCreateKeyspace) == false;

            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("No server address is specified.");
            }

            if (string.IsNullOrEmpty(keyspace))
            {
                throw new InvalidOperationException("No keyspace is specified.");
            }

            if (string.IsNullOrEmpty(username))
            {
                username = null;
            }

            if (string.IsNullOrEmpty(password))
            {
                password = null;
            }

            if (directory == null)
            {
                directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            // Setup migrator.
            using var connection = await Connection.ConnectAsync(address, keyspace, username, password, createKeyspace);
            using var logger = LoggerFactory.Create(b => b.AddConsole());

            var migrator = new Migrator(connection, logger.CreateLogger<Migrator>());
            var provider = new FileSystemMigrationProvider(directory.FullName);

            // Execute migrations.
            await migrator.ExecuteAsync(provider);

            return 0;
        }
    }
}
