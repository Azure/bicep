// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    /// <summary>
    /// Visitor responsible for collecting all the parse diagnostics from the parse tree.
    /// </summary>
    public class ParseDiagnosticsVisitor : SyntaxVisitor
    {
        private readonly IList<Diagnostic> diagnostics;
        
        public ParseDiagnosticsVisitor(IList<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            foreach (var diagnostic in syntax.LexerDiagnostics)
            {
                this.diagnostics.Add(diagnostic);
            }
        }

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            base.VisitSkippedTriviaSyntax(syntax);

            foreach (var diagnostic in syntax.Diagnostics)
            {
                this.diagnostics.Add(diagnostic);
            }
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            if (syntax.IdentifierName.Length > LanguageConstants.MaxIdentifierLength)
            {
                this.diagnostics.Add(DiagnosticBuilder.ForPosition(syntax.Identifier).IdentifierNameExceedsLimit());
            }

            base.VisitIdentifierSyntax(syntax);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            base.VisitObjectSyntax(syntax);

            var duplicatedProperties = syntax.Properties
                .GroupByExcludingNull(prop => prop.TryGetKeyText(), LanguageConstants.IdentifierComparer)
                .Where(group => group.Count() > 1);

            foreach (var group in duplicatedProperties)
            {
                foreach (ObjectPropertySyntax duplicatedProperty in group)
                {
                    this.diagnostics.Add(DiagnosticBuilder.ForPosition(duplicatedProperty.Key).PropertyMultipleDeclarations(group.Key));
                }
            }
        }
    }
}
