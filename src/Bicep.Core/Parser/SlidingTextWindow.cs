using Bicep.Core.Parser;

namespace Bicep.Core.Parser
{
    public class SlidingTextWindow
    {
        public const char InvalidCharacter = char.MaxValue;

        private readonly string text;

        public SlidingTextWindow(string text)
        {
            this.text = text;
            this.position = 0;
            this.offset = 0;
        }

        public TextSpan GetSpan()
            => new TextSpan(position, offset);

        public string GetText()
            => text.Substring(position, offset);

        private int position;

        private int offset;

        public char Peek(int numChars = 0)
        {
            if (position + offset + numChars >= text.Length)
            {
                return InvalidCharacter;
            }

            return text[position + offset + numChars];
        }

        public bool IsAtEnd()
            => position + offset >= text.Length;

        public char Next()
        {
            var nextChar = Peek(0);
            if (nextChar != InvalidCharacter)
            {
                Advance(nextChar);
            }

            return nextChar;
        }

        public void Advance(int numChars = 1)
        {
            offset += numChars;
        }

        public void Reset()
        {
            position += offset;
            offset = 0;
        }
    }
}
