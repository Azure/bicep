// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1080 : LinterRule
    {
        internal BCPL1080() : base(
            code: "BCPL1080",
            ruleName: "String interpolation can be simplified.",
            description: "String interpolation can be simplified. String variables can be directly assigned to string properties and variables.",
            docUri: "https://bicep/linter/rules/BCPL1080")
        { }


        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new BCPL1080Visitor(spanFixes);
            visitor.Visit(model.SyntaxTree.ProgramSyntax);

            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(kvp.Key, kvp.Value));
        }

        private sealed class BCPL1080Visitor : SyntaxVisitor
        {
            private readonly Dictionary<TextSpan, CodeFix> spanFixes;

            public BCPL1080Visitor(Dictionary<TextSpan, CodeFix> spanFixes)
            {
                this.spanFixes = spanFixes;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                AddCodeFixIfSingleInterpolatedString(syntax.Value);
                base.VisitObjectPropertySyntax(syntax);
            }

            public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
            {
                AddCodeFixIfSingleInterpolatedString(syntax.Value);
                base.VisitVariableDeclarationSyntax(syntax);
            }

            public override void VisitLocalVariableSyntax(LocalVariableSyntax syntax)
            {
                base.VisitLocalVariableSyntax(syntax);
            }

            public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            {
                AddCodeFixIfSingleInterpolatedString(syntax.Value);
                base.VisitOutputDeclarationSyntax(syntax);
            }

            public override void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
            {
                AddCodeFixIfSingleInterpolatedString(syntax.DefaultValue);
                base.VisitParameterDefaultValueSyntax(syntax);
            }

            private VariableAccessSyntax? AddCodeFixIfSingleInterpolatedString(SyntaxBase valueSyntax)
            {
                if (valueSyntax is StringSyntax strSyntax
                    && strSyntax.Expressions.Length == 1
                    && strSyntax.SegmentValues.All(s => string.IsNullOrEmpty(s))
                    && strSyntax.Expressions.First() is VariableAccessSyntax variableAccessSyntax)
                {
                    AddCodeFix(valueSyntax.Span, variableAccessSyntax.Name.IdentifierName);
                }
                return null;
            }

            private void AddCodeFix(TextSpan span, string name)
            {
                var codeReplacement = new CodeReplacement(span, name);
                var fix = new CodeFix($"Use string variable assignment: {codeReplacement.Text}", true, codeReplacement);
                spanFixes[span] = fix;
            }
        }
    }
}
