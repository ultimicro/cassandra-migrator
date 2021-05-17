namespace CassandraMigrator.Console
{
    using System.CommandLine;
    using System.Threading.Tasks;

    public static class Program
    {
        public static Task<int> Main(string[] args)
        {
            var root = new RootCommand("Cassandra schema migration tool")
            {
                new DatabaseCommand(),
            };

            return root.InvokeAsync(args);
        }
    }
}
