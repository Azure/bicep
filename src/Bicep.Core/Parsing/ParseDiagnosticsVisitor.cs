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
    public class ParseDiagnosticsVisitor : AstVisitor
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

            // find duplicate target scope declarations
            var targetScopeSyntaxes = syntax.Children.OfType<TargetScopeSyntax>().ToImmutableArray();
            if (targetScopeSyntaxes.Length > 1)
            {
                foreach (var targetScope in targetScopeSyntaxes)
                {
                    this.diagnosticWriter.Write(targetScope.Keyword, x => x.TargetScopeMultipleDeclarations());
                }
            }

            // find duplicate using declarations
            var usingSyntaxes = syntax.Children.OfType<UsingDeclarationSyntax>().ToImmutableArray();
            if (usingSyntaxes.Length > 1)
            {
                foreach (var declaration in usingSyntaxes)
                {
                    this.diagnosticWriter.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
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
