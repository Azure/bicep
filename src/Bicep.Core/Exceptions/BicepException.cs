// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Exceptions
{
    /// <summary>
    /// Exception used to signal common error conditions.
    /// </summary>
    public class BicepException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
    }
}
