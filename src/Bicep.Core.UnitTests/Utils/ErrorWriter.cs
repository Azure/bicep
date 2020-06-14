using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parser;

namespace Bicep.Core.UnitTests.Utils
{
    public class ErrorWriter
    {
        private readonly StringBuilder buffer;

        public ErrorWriter(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        public void WriteErrors(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
            {
                this.WriteError(error);
            }
        }

        public void WriteError(Error error)
        {
            this.buffer.Append(error.Message);
            this.buffer.Append(' ');
            this.buffer.AppendLine(error.Span.ToString());
        }
    }
}
