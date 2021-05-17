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

        public UpdateCommand()
            : base("update", "Apply migrations to the database")
        {
            this.Add(DirectoryOption);
            this.Handler = this;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            // Get options.
            var address = context.ParseResult.ValueForOption(DatabaseCommand.AddressOption);
            var keyspace = context.ParseResult.ValueForOption(DatabaseCommand.KeyspaceOption);
            var directory = context.ParseResult.ValueForOption(DirectoryOption);

            if (string.IsNullOrEmpty(address))
            {
                throw new InvalidOperationException("No server address is specified.");
            }

            if (string.IsNullOrEmpty(keyspace))
            {
                throw new InvalidOperationException("No keyspace is specified.");
            }

            if (directory == null)
            {
                directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            // Setup migrator.
            using var connection = await Connection.ConnectAsync(address, keyspace);
            using var logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var migrator = new Migrator(connection, logger.CreateLogger<Migrator>());
            var provider = new FileSystemMigrationProvider(directory.FullName);

            // Execute migrations.
            await migrator.ExecuteAsync(provider);

            return 0;
        }
    }
}
