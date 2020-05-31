using System;

namespace Bicep.Cli.CommandLine
{
    public class CommandLineException : Exception
    {
        public CommandLineException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}
