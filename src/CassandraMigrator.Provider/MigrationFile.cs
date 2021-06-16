namespace CassandraMigrator.Provider
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CassandraMigrator.CqlParser;

    internal sealed class MigrationFile : Migration
    {
        private readonly string path;

        public MigrationFile(MigrationVersion version, string path)
            : base(version)
        {
            this.path = path;
        }

        public override async ValueTask<IEnumerable<string>> ListStatementAsync(CancellationToken cancellationToken = default)
        {
            var cql = await File.ReadAllTextAsync(this.path, Encoding.UTF8, cancellationToken);

            return CqlParser.ParseStatements(cql);
        }

        public override string ToString() => this.path;
    }
}
