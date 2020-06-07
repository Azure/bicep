using System;

namespace Bicep.Core.Visitors
{
    public class BicepInternalException : Exception
    {
        public BicepInternalException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}