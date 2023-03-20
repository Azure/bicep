// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    /// <summary>
    /// Visitor responsible for collecting all the parse diagnostics from the parse tree.
    /// </summary>
    public class ParseDiagnosticsVisitor : CstVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;

        public ParseDiagnosticsVisitor(IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
        }

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            base.VisitSkippedTriviaSyntax(syntax);

            this.diagnosticWriter.WriteMultiple(syntax.Diagnostics);
        }
    }
}
