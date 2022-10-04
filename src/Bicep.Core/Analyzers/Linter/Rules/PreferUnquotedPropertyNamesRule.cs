// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class PreferUnquotedPropertyNamesRule : LinterRuleBase
    {
        public new const string Code = "prefer-unquoted-property-names";

        public PreferUnquotedPropertyNamesRule() : base(
            code: Code,
            description: CoreResources.PreferUnquotedPropertyNamesRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new Visitor(spanFixes);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            var diagnosticLevel = GetDiagnosticLevel(model);
            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(diagnosticLevel, kvp.Key, kvp.Value));
        }

        private sealed class Visitor : SyntaxVisitor
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
                    var message = string.Format(CoreResources.PreferUnquotedPropertyNames_DereferenceFixTitle, $"{syntax.BaseExpression.ToText()}{replacement}");
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
                if (syntax is StringSyntax @string)
                {
                    string? literalValue = @string.TryGetLiteralValue();
                    if (literalValue is not null && Lexer.IsValidIdentifier(literalValue))
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
