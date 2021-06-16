namespace CassandraMigrator.CqlParser
{
    using System.Collections.Generic;
    using Antlr4.Runtime.Misc;
    using CassandraMigrator.CqlParser.Parsing;

    internal sealed class StatementExtractor : CqlParserBaseListener
    {
        private readonly List<string> statements;

        public StatementExtractor()
        {
            this.statements = new();
        }

        public IEnumerable<string> Statements => this.statements;

        public override void EnterCql(Parsing.CqlParser.CqlContext context)
        {
            var start = context.Start.StartIndex;
            var stop = context.Stop.StopIndex;
            var cql = context.Start.InputStream.GetText(new Interval(start, stop));

            this.statements.Add(cql);
        }
    }
}
