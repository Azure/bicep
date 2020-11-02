// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public static class IDiagnosticWriterExtensions
    {
        public static void WriteMultiple(this IDiagnosticWriter diagnosticWriter, IEnumerable<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                diagnosticWriter.Write(diagnostic);
            }
        }

        public static void WriteMultiple(this IDiagnosticWriter diagnosticWriter, params Diagnostic[] diagnostics)
            => WriteMultiple(diagnosticWriter, (IEnumerable<Diagnostic>) diagnostics);
    }
}