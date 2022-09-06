namespace CassandraMigrator.CqlParser;

using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

public static class CqlParser
{
    /// <summary>
    /// Parse the specified CQL to get all statements that are inside it.
    /// </summary>
    /// <param name="cql">
    /// The CQL contains multiple statements separated with semi-colon.
    /// </param>
    /// <returns>
    /// List of statements.
    /// </returns>
    /// <exception cref="CqlException">
    /// <paramref name="cql"/> contain invalid CQL.
    /// </exception>
    /// <remarks>
    /// No semi-colon in each result statement.
    /// </remarks>
    public static IEnumerable<string> ParseStatements(string cql)
    {
        var input = new AntlrInputStream(cql);
        var lexer = new CqlLexer(input);
        var tokens = new CommonTokenStream(lexer);
        var parser = new Parsing.CqlParser(tokens) { ErrorHandler = new BailErrorStrategy() };
        var walker = new ParseTreeWalker();
        var extractor = new StatementExtractor();

        try
        {
            walker.Walk(extractor, parser.root());
        }
        catch (ParseCanceledException ex)
        {
            var token = (ex.InnerException as RecognitionException)?.OffendingToken;

            if (token == null)
            {
                throw;
            }

            throw new CqlException(token.Line, token.Column, token.Text, ex);
        }

        return extractor.Statements;
    }
}
