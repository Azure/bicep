// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Exceptions
{
    /// <summary>
    /// Exception used to signal common error conditions.
    /// </summary>
    public class BicepException : Exception
    {
        public BicepException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
