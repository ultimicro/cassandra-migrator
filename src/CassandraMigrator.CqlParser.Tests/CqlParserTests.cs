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
    date date,
    PRIMARY KEY (pk, ck)
)";
            var table2 = @"CREATE TABLE bar (
    pk uuid,
    ck blob,
    val FROZEN<udt1>,
    PRIMARY KEY (pk, ck)
) WITH CLUSTERING ORDER BY (ck DESC) AND compaction = {'class': 'LeveledCompactionStrategy'}";
            var table3 = @"CREATE TABLE baz (
    pk UUID,
    val COUNTER,
    PRIMARY KEY (pk)
)";
            var insert1 = "INSERT INTO foo (pk, ck, val) VALUES (fd1d5bea-ac6b-4af0-a475-c310afc491df, 0X00000000, 'ABC')";
            var insert2 = "INSERT INTO bar (pk, ck, val) VALUES (00000000-0000-0000-0000-000000000000, 0x00000001, { field1: { inner: 0 }, field2: func1() })";
            var alterTable1 = "ALTER TABLE baz DROP (baz1, baz2)";
            var alterTable2 = "ALTER TABLE baz ADD baz3 UUID";
            var alterTable3 = "ALTER TABLE foo ADD foo TEXT";
            var alterTable4 = "ALTER TABLE foo ADD bar TINYINT";
            var alterTable5 = "ALTER TABLE baz ADD foo MAP<ASCII, DECIMAL>";
            var alterTable6 = "ALTER TABLE foo ADD baz DATE";
            var cql = @$"
-- comment 1
{table1};
// comment 2
{table2};
{table3};
/*
comment 3
*/
{insert1};
{insert2};
{alterTable1};
{alterTable2};
{alterTable3};
{alterTable4};
{alterTable5};
{alterTable6};";
            var result = CqlParser.ParseStatements(cql).ToList();

            Assert.Equal(11, result.Count);
            Assert.Equal(table1, result[0]);
            Assert.Equal(table2, result[1]);
            Assert.Equal(table3, result[2]);
            Assert.Equal(insert1, result[3]);
            Assert.Equal(insert2, result[4]);
            Assert.Equal(alterTable1, result[5]);
            Assert.Equal(alterTable2, result[6]);
            Assert.Equal(alterTable3, result[7]);
            Assert.Equal(alterTable4, result[8]);
            Assert.Equal(alterTable5, result[9]);
            Assert.Equal(alterTable6, result[10]);
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
    user UUID,
    language ASCII,
    level TINYINT,
    PRIMARY KEY (user)
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
