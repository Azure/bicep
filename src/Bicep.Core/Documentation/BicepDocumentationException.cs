// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Core.Documentation;

/// <summary>
/// Represents a documentation model or rendering failure.
/// </summary>
public class BicepDocumentationException : BicepException
{
    public BicepDocumentationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
