// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class PreferUnquotedPropertyNamesRule : LinterRuleBase
    {
        public new const string Code = "prefer-unquoted-property-names";

        public PreferUnquotedPropertyNamesRule() : base(
            code: Code,
            description: CoreResources.PreferUnquotedPropertyNamesRule_Description,
            LinterRuleCategory.Style)
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new Visitor(spanFixes);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(diagnosticLevel, kvp.Key, kvp.Value));
        }

        private sealed class Visitor : AstVisitor
        {
            private readonly Dictionary<TextSpan, CodeFix> spanFixes;

            public Visitor(Dictionary<TextSpan, CodeFix> spanFixes)
            {
                this.spanFixes = spanFixes;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                if (TryGetValidIdentifierToken(syntax.Key, out string? literal))
                {
                    AddCodeFix(syntax.Key.Span, literal, string.Format(CoreResources.PreferUnquotedPropertyNames_DeclarationFixTitle, literal));
                }

                base.VisitObjectPropertySyntax(syntax);
            }

            public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            {
                if (TryGetValidIdentifierToken(syntax.IndexExpression, out string? literal))
                {
                    var replacement = $".{literal}";
                    var message = string.Format(CoreResources.PreferUnquotedPropertyNames_DereferenceFixTitle, $"{syntax.BaseExpression}{replacement}");
                    AddCodeFix(TextSpan.Between(syntax.OpenSquare, syntax.CloseSquare), replacement, message);
                }

                base.VisitArrayAccessSyntax(syntax);
            }

            private void AddCodeFix(TextSpan span, string replacement, string description)
            {
                var codeReplacement = new CodeReplacement(span, replacement);
                var fix = new CodeFix(description, true, CodeFixKind.QuickFix, codeReplacement);
                spanFixes[span] = fix;
            }

            private static bool TryGetValidIdentifierToken(SyntaxBase syntax, [NotNullWhen(true)] out string? validToken)
            {
                if (syntax is StringSyntax stringSyntax && stringSyntax.TryGetLiteralValue() is { } literalValue)
                {
                    if (!StringUtils.IsPropertyNameEscapingRequired(literalValue))
                    {
                        validToken = literalValue;
                        return true;
                    }
                }

                validToken = null;
                return false;
            }
        }
    }
}
