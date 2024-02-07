// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class TokenReader(IEnumerable<Token> tokens)
    {
        private Token[] Tokens { get; } = tokens.ToArray();

        public int Position { get; private set; } = 0;

        public int Count => this.Tokens.Length;

        public bool IsAtEnd() => this.Position >= this.Tokens.Length;

        public Token Prev()
        {
            return this.Tokens[Position - 1];
        }

        public Token Peek(bool skipNewlines = false)
        {
            var peekPosition = this.Position;

            if (skipNewlines)
            {
                while (this.AtPosition(peekPosition).IsOf(TokenType.NewLine))
                {
                    peekPosition++;
                }
            }

            return Tokens[peekPosition];
        }

        public Token? PeekAhead(int charCount = 1, bool skipNewlines = false)
        {
            var effectivePosition = this.Position + charCount;

            if (skipNewlines)
            {
                while (this.AtPosition(effectivePosition).IsOf(TokenType.NewLine))
                {
                    effectivePosition++;
                }
            }

            return effectivePosition < this.Tokens.Length ? this.AtPosition(effectivePosition) : null;
        }

        public Token Read()
        {
            var output = Peek();
            this.Position++;
            return output;
        }

        public IEnumerable<Token> Slice(int start, int length)
        {
            return this.Tokens.Skip(start).Take(length);
        }

        public Token AtPosition(int position)
        {
            return this.Tokens[position];
        }
    }
}

