// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SimplifyInterpolationRule : LinterRuleBase
    {
        public new const string Code = "simplify-interpolation";

        public SimplifyInterpolationRule() : base(
            code: Code,
            description: CoreResources.SimplifyInterpolationRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new Visitor(spanFixes, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            var diagnosticLevel = GetDiagnosticLevel(model);
            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(diagnosticLevel, kvp.Key, kvp.Value));
        }

        private sealed class Visitor : SyntaxVisitor
        {
            private readonly Dictionary<TextSpan, CodeFix> spanFixes;
            private readonly SemanticModel model;

            public Visitor(Dictionary<TextSpan, CodeFix> spanFixes, SemanticModel model)
            {
                this.spanFixes = spanFixes;
                this.model = model;
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
                // We're looking for code with this pattern:
                //
                // resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                //   name: '${AutomationAccountName}'   <<= a string literal with a single interpolated value

                if (valueSyntax is StringSyntax strSyntax
                    && strSyntax.Expressions.Length == 1
                    && strSyntax.SegmentValues.All(s => string.IsNullOrEmpty(s))
                    && strSyntax.Expressions.First() is ExpressionSyntax expression)
                {
                    // We only want to trigger if the expression is of type string (because interpolation
                    // using non-string types can be a perfectly valid way to convert to string, e.g. '${intVar}')
                    var type = model.GetTypeInfo(expression);
                    if (type.IsString())
                    {
                        AddCodeFix(valueSyntax.Span, expression.ToText());
                    }
                }
                return null;
            }

            private void AddCodeFix(TextSpan span, string name)
            {
                var codeReplacement = new CodeReplacement(span, name);
                var fix = new CodeFix(CoreResources.SimplifyInterpolationFixTitle, true, CodeFixKind.QuickFix, codeReplacement);
                spanFixes[span] = fix;
            }
        }
    }
}
