// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Diagnostics
{
    /// <summary>
    /// Exception with error diagnostic information attached.
    /// </summary>
    public class ErrorDiagnosticException : Exception
    {
        public ErrorDiagnosticException(ErrorDiagnostic diagnostic, Exception? inner = null)
            : base(diagnostic.Message, inner)
        {
            Diagnostic = diagnostic;
        }

        public ErrorDiagnostic Diagnostic { get; }
    }
}