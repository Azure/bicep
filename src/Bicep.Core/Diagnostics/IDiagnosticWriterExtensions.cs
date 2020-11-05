// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Parser;

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

        public static void Write(this IDiagnosticWriter diagnosticWriter, IPositionable positionable, DiagnosticBuilder.DiagnosticBuilderDelegate buildDiagnosticFunc)
            => diagnosticWriter.Write(buildDiagnosticFunc(DiagnosticBuilder.ForPosition(positionable)));
    }
}