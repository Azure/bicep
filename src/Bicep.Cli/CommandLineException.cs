// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Exceptions;

namespace Bicep.Cli
{
    public class CommandLineException : BicepException
    {
        public CommandLineException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}

