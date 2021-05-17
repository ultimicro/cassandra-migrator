namespace CassandraMigrator.Provider.Tests
{
    using System.Threading.Tasks;
    using Xunit;

    public sealed class FileSystemMigrationProviderTests : IClassFixture<FileSystemMigrationProviderFixture>
    {
        private readonly FileSystemMigrationProvider subject;

        public FileSystemMigrationProviderTests(FileSystemMigrationProviderFixture fixture)
        {
            this.subject = new FileSystemMigrationProvider(fixture.Directory.FullName);
        }

        [Fact]
        public async Task GetAsync_WithExistsMigration_ShouldReturnThatMigration()
        {
            var version = new MigrationVersion(1, 0);
            await using var result = await this.subject.GetAsync(version);

            Assert.NotNull(result);
            Assert.Equal(version, result!.Version);
        }

        [Fact]
        public Task GetAsync_WithMissingMigration_ShouldThrow()
        {
            return Assert.ThrowsAsync<MigrationNotFoundException>(async () => await this.subject.GetAsync(new MigrationVersion(2, 1)));
        }

        [Fact]
        public async Task GetAsync_WithUnavailableMigrationForTheSameMajor_ShouldReturnNull()
        {
            var result = await this.subject.GetAsync(new MigrationVersion(1, 2));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithUnavailableMigration_ShouldReturnNull()
        {
            var result = await this.subject.GetAsync(new MigrationVersion(2, 3));

            Assert.Null(result);
        }
    }
}
