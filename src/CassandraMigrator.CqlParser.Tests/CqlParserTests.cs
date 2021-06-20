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
)";
            var insert = "INSERT INTO foo (pk, ck, val) VALUES (fd1d5bea-ac6b-4af0-a475-c310afc491df, 0X00000000, 'ABC')";
            var cql = @$"
-- comment 1
{table1};
// comment 2
{table2};
/*
comment 3
*/
{insert};
";
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
    }
}
