namespace CassandraMigrator.CqlParser
{
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;

    public static class CqlParser
    {
        public static IEnumerable<string> ParseStatements(string cql)
        {
            var input = new AntlrInputStream(cql);
            var lexer = new CqlLexer(new CaseChangingCharStream(input, true));
            var tokens = new CommonTokenStream(lexer);
            var parser = new Parsing.CqlParser(tokens) { ErrorHandler = new BailErrorStrategy() };
            var walker = new ParseTreeWalker();
            var extractor = new StatementExtractor();

            walker.Walk(extractor, parser.root());

            return extractor.Statements;
        }
    }
}
