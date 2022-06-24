namespace CassandraMigrator.CqlParser
{
    using System;

    public sealed class CqlException : FormatException
    {
        public CqlException(int line, int column, string token)
            : this(line, column, token, null)
        {
        }

        public CqlException(int line, int column, string token, Exception? innerException)
            : base($"Unrecognized CQL token {token} at line {line}:{column}.", innerException)
        {
            this.Line = line;
            this.Column = column;
            this.Token = token;
        }

        public int Line { get; }

        public int Column { get; }

        public string Token { get; }
    }
}
