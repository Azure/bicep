// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    /// <summary>
    /// Visitor responsible for collecting all the parse diagnostics from the parse tree.
    /// </summary>
    public class ParseDiagnosticsVisitor : SyntaxVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;
        
        public ParseDiagnosticsVisitor(IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);
            
            this.diagnosticWriter.WriteMultiple(syntax.LexerDiagnostics);

            var targetScopeSyntaxes = syntax.Children.OfType<TargetScopeSyntax>().ToList();
            if (targetScopeSyntaxes.Count > 1)
            {
                foreach (var targetScope in targetScopeSyntaxes)
                {
                    this.diagnosticWriter.Write(targetScope.Keyword, x => x.TargetScopeMultipleDeclarations());
                }
            }
        }

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            base.VisitSkippedTriviaSyntax(syntax);

            this.diagnosticWriter.WriteMultiple(syntax.Diagnostics);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            if (syntax.IdentifierName.Length > LanguageConstants.MaxIdentifierLength)
            {
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.Child).IdentifierNameExceedsLimit());
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
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(duplicatedProperty.Key).PropertyMultipleDeclarations(group.Key));
                }
            }
        }
    }
}
