// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Exceptions;

namespace Bicep.Core.Diagnostics
{
    /// <summary>
    /// Exception with error diagnostic information attached.
    /// </summary>
    public class DiagnosticException : BicepException
    {
        public DiagnosticException(Diagnostic diagnostic, Exception? inner = null)
            : base(diagnostic.Message, inner)
        {
            Diagnostic = diagnostic;
        }

        public Diagnostic Diagnostic { get; }
    }
}
