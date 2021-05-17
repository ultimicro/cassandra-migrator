namespace CassandraMigrator.Provider
{
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using GCore.Antlr.Grammers.Cql3;

    public static class MigrationText
    {
        public static IEnumerable<string> Parse(string cql)
        {
            // https://github.com/antlr/grammars-v4/blob/master/cql3/CqlParser.g4
            var input = new AntlrInputStream(cql);
            var lexer = new CqlLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CqlParser(tokens) { ErrorHandler = new BailErrorStrategy() };
            var walker = new ParseTreeWalker();
            var result = new List<string>();
            var extractor = new CqlExtractor(result);

            walker.Walk(extractor, parser.root());

            return result;
        }
    }
}
