using Bicep.Parser;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Bicep.Cli
{
    class TokenReader : IReader<Token>
    {
        private Token[] Tokens { get; }

        public TokenReader(IEnumerable<Token> tokens)
        {
            Tokens = tokens.ToArray();
            Position = 0;
        }

        public int Position { get; private set; }

        public bool IsAtEnd() => Position >= Tokens.Length;

        public Token Prev()
        {
            return Tokens[Position - 1];
        }

        public Token Peek()
        {
            return Tokens[Position];
        }

        public Token Read()
        {
            var output = Peek();
            Position++;
            return output;
        }

        public IEnumerable<Token> Slice(int start, int length)
        {
            return Tokens.Skip(start).Take(length);
        }
    }
}
