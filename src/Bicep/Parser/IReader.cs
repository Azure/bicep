using System.Collections.Generic;

namespace Bicep.Parser
{
    public interface IReader<T>
    {
        int Position { get; }

        T Prev();

        T Peek();

        T Read();

        bool IsAtEnd();

        IEnumerable<T> Slice(int start, int length);
    }
}