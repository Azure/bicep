using System;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(string message)
            : base(message)
        {
        }
    }
}