namespace CassandraMigrator.CqlParser.Tests
{
    using System.Linq;
    using Xunit;

    public sealed class CqlParserTests
    {
        [Fact]
        public void ParseStatements_WithValidCQL_ShouldReturnAllNonCommentStatements()
        {
            var table1 = @"CREATE TABLE foo (
    pk uuid,
    ck blob,
    val text,
    PRIMARY KEY (pk, ck)
)";
            var table2 = @"CREATE TABLE bar (
    pk uuid,
    ck blob,
    val text,
    PRIMARY KEY (pk, ck)
) WITH CLUSTERING ORDER BY (ck DESC) AND compaction = {'class': 'LeveledCompactionStrategy'}";
            var insert = "INSERT INTO foo (pk, ck, val) VALUES (fd1d5bea-ac6b-4af0-a475-c310afc491df, 0X00000000, 'ABC')";
            var cql = @$"
-- comment 1
{table1};
// comment 2
{table2};
/*
comment 3
*/
{insert};";
            var result = CqlParser.ParseStatements(cql).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(table1, result[0]);
            Assert.Equal(table2, result[1]);
            Assert.Equal(insert, result[2]);
        }

        [Fact]
        public void ParseStatements_WithInvalidCQL_ShouldThrow()
        {
            var invalid = "INSERT INTO foo (pk) VALUES 0x00000000;";
            var ex = Assert.Throws<CqlException>(() => CqlParser.ParseStatements(invalid));

            Assert.Equal(1, ex.Line);
            Assert.Equal(28, ex.Column);
            Assert.Equal("0x00000000", ex.Token);
        }

        [Fact]
        public void ParseStatements_WithAllowedReservedWordInColumnName_ShouldSuccess()
        {
            var type = @"CREATE TYPE foo (
    type TINYINT
)";
            var table = @"CREATE TABLE bar (
    language ASCII,
    level TINYINT,
    PRIMARY KEY (language)
) WITH compaction = {'class': 'LeveledCompactionStrategy'}";
            var cql = @$"
{type};

{table};";

            var result = CqlParser.ParseStatements(cql).ToArray();

            Assert.Equal(2, result.Length);
            Assert.Equal(type, result[0]);
            Assert.Equal(table, result[1]);
        }

        [Fact]
        public void ParseStatements_WithMaterializedView_ShouldSuccess()
        {
            var cql = "CREATE MATERIALIZED VIEW foo AS SELECT * FROM bar WHERE b IS NOT NULL AND a IS NOT NULL PRIMARY KEY (b, a) WITH CLUSTERING ORDER BY (a DESC)";
            var result = CqlParser.ParseStatements(cql).ToArray();

            var statement = Assert.Single(result);
            Assert.Equal(cql, statement);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        [InlineData("-- comment")]
        [InlineData("-- comment\n")]
        [InlineData("// comment")]
        [InlineData("// comment\n")]
        public void ParseStatements_WithNoStatements_ShouldReturnEmptyList(string cql)
        {
            var result = CqlParser.ParseStatements(cql);

            Assert.Empty(result);
        }
    }
}
