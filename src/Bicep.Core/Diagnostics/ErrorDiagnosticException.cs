// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Exceptions;

namespace Bicep.Core.Diagnostics
{
    /// <summary>
    /// Exception with error diagnostic information attached.
    /// </summary>
    public class ErrorDiagnosticException(ErrorDiagnostic diagnostic, Exception? inner = null) : BicepException(diagnostic.Message, inner)
    {
        public ErrorDiagnostic Diagnostic { get; } = diagnostic;
    }
}
