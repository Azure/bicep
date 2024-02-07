// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Cli
{
    public class CommandLineException(string message, Exception? inner = null) : BicepException(message, inner)
    {
    }
}

