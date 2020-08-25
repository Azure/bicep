// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Parser
{
    class TokenReader
    {
        private Token[] Tokens { get; }

        public TokenReader(IEnumerable<Token> tokens)
        {
            this.Tokens = tokens.ToArray();
            this.Position = 0;
        }

        public int Position { get; private set; }

        public int Count => this.Tokens.Length;

        public bool IsAtEnd() => this.Position >= this.Tokens.Length;

        public Token Prev()
        {
            return this.Tokens[Position - 1];
        }

        public Token Peek()
        {
            return Tokens[Position];
        }

        public Token Read()
        {
            var output = Peek();
            this.Position++;
            return output;
        }

        public void StepBack()
        {
            if (this.Position == 0)
            {
                throw new InvalidOperationException("Reader is already at the beginning.");
            }

            this.Position -= 1;
        }

        public IEnumerable<Token> Slice(int start, int length)
        {
            return this.Tokens.Skip(start).Take(length);
        }
    }
}

