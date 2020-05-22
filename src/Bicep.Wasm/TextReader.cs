using Bicep.Parser;
using System.Collections.Generic;

namespace Bicep.Wasm
{
    class TextReader : IReader<char>
    {
        private readonly string text;

        public TextReader(string text)
        {
            this.text = text;
            Position = 0;
        }

        public int Position { get; private set; }

        public bool IsAtEnd() => Position >= text.Length;

        public char Prev()
        {
            return text[Position - 1];
        }

        public char Peek()
        {
            return text[Position];
        }

        public char Read()
        {
            var output = Peek();
            Position++;
            return output;
        }

        public IEnumerable<char> Slice(int start, int length)
        {
            return text.Substring(start, length);
        }
    }
}
