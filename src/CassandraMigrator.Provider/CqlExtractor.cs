namespace CassandraMigrator.Provider
{
    using System.Collections.Generic;
    using Antlr4.Runtime.Misc;
    using GCore.Antlr.Grammers.Cql3;

    internal sealed class CqlExtractor : CqlParserBaseListener
    {
        private readonly List<string> result;

        public CqlExtractor(List<string> result)
        {
            this.result = result;
        }

        public override void EnterCql([NotNull] CqlParser.CqlContext context)
        {
            var start = context.Start.StartIndex;
            var stop = context.Stop.StopIndex;
            var cql = context.Start.InputStream.GetText(new Interval(start, stop));

            this.result.Add(cql);
        }
    }
}
