// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Cli
{
    /// <summary>
    /// Exception used to signal common error conditions. Message will logged to StdErr as-is.
    /// </summary>
    public class BicepException : Exception
    {
        public BicepException(string message) : base(message)
        {
        }

        public BicepException(string message, Exception? inner) : base(message, inner)
        {
        }
    }
}
