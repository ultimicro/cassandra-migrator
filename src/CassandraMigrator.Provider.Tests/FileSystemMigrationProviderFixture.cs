namespace CassandraMigrator.Provider.Tests
{
    using System;
    using System.IO;

    public sealed class FileSystemMigrationProviderFixture : IDisposable
    {
        public FileSystemMigrationProviderFixture()
        {
            this.Directory = new DirectoryInfo(Path.Join(Path.GetTempPath(), Path.GetRandomFileName()));
            this.Directory.Create();

            try
            {
                CreateEmptyMigration("0.0.cql");
                CreateEmptyMigration("1.0.cql");
                CreateEmptyMigration("1.1.cql");
                CreateEmptyMigration("2.0.cql");
                CreateEmptyMigration("2.2.cql");
            }
            catch
            {
                this.Directory.Delete(true);
                throw;
            }

            string GetPath(string name) => Path.Join(this.Directory.FullName, name);
            void CreateEmptyMigration(string name) => File.Create(GetPath(name)).Dispose();
        }

        public DirectoryInfo Directory { get; }

        public void Dispose()
        {
            this.Directory.Delete(true);
        }
    }
}
