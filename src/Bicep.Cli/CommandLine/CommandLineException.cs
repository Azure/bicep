// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

