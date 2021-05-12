// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class BCPL1040 : LinterRuleBase
    {
        public BCPL1040() : base(
            code: "BCPL1040",
            ruleName: "Location set by parameter",
            description: "Best practice dictates that Location be set via parameter.",
            docUri: "https://bicep/linter/rules/BCPL1040")// TODO: setup up doc pages
        { }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanDiagnostics = new List<TextSpan>();

            var visitor = new BCPL1040Visitor(spanDiagnostics);
            visitor.Visit(model.SyntaxTree.ProgramSyntax);

            return spanDiagnostics.Select(span => CreateDiagnosticForSpan(span));
        }

        private sealed class BCPL1040Visitor : SyntaxVisitor
        {
            private readonly List<TextSpan> diagnostics;

            public BCPL1040Visitor(List<TextSpan> diagnostics)
            {
                this.diagnostics = diagnostics;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                if (syntax.NameEquals("location"))
                {
                    if (syntax.Value is StringSyntax stringSyntax && stringSyntax.IsStringLiteral())
                    {
                        this.diagnostics.Add(stringSyntax.Span);
                    }
                }
                base.VisitObjectPropertySyntax(syntax);
            }
        }
    }
}
