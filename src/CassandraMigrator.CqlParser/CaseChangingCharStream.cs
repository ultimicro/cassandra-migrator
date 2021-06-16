/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace Antlr4.Runtime
{
    using Antlr4.Runtime.Misc;

    /// <summary>
    /// This class supports case-insensitive lexing by wrapping an existing
    /// <see cref="ICharStream"/> and forcing the lexer to see either upper or
    /// lowercase characters. Grammar literals should then be either upper or
    /// lower case such as 'BEGIN' or 'begin'. The text of the character
    /// stream is unaffected. Example: input 'BeGiN' would match lexer rule
    /// 'BEGIN' if constructor parameter upper=true but getText() would return
    /// 'BeGiN'.
    /// </summary>
    public class CaseChangingCharStream : ICharStream
    {
        private ICharStream stream;
        private bool upper;

        public CaseChangingCharStream(ICharStream stream, bool upper)
        {
            this.stream = stream;
            this.upper = upper;
        }

        public int Index
        {
            get
            {
                return this.stream.Index;
            }
        }

        public int Size
        {
            get
            {
                return this.stream.Size;
            }
        }

        public string SourceName
        {
            get
            {
                return this.stream.SourceName;
            }
        }

        public void Consume()
        {
            this.stream.Consume();
        }

        [return: NotNull]
        public string GetText(Interval interval)
        {
            return this.stream.GetText(interval);
        }

        public int La(int i)
        {
            int c = this.stream.La(i);

            if (c <= 0)
            {
                return c;
            }

            char o = (char)c;

            if (this.upper)
            {
                return char.ToUpperInvariant(o);
            }

            return char.ToLowerInvariant(o);
        }

        public int Mark()
        {
            return this.stream.Mark();
        }

        public void Release(int marker)
        {
            this.stream.Release(marker);
        }

        public void Seek(int index)
        {
            this.stream.Seek(index);
        }
    }
}
