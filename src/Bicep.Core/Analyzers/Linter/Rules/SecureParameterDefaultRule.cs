// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParameterDefaultRule : LinterRuleBase
    {
        public new const string Code = "secure-paramenter-default";

        public SecureParameterDefaultRule() : base(
            code: Code,
            description: CoreResources.SecureParameterDefaultRuleDescription,
            docUri: "https://aka.ms/bicep/linter/secure-paramenter-default")
        { }

        override public IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var defaultValueSyntaxes = model.Root.ParameterDeclarations.Where(p => p.IsSecure())
                .Select(p => p.Modifier as ParameterDefaultValueSyntax)
                .OfType<ParameterDefaultValueSyntax>(); // this eliminates nulls (when there's no default value)

            foreach (var defaultValueSyntax in defaultValueSyntaxes)
            {
                var defaultValue = defaultValueSyntax.DefaultValue;
                if (defaultValue is StringSyntax defaultString && defaultString.IsStringLiteral()
                    && defaultString.TryGetLiteralValue() == "")
                {
                    // Empty string - okay
                    continue;
                }
                else if (defaultValue is ObjectSyntax objectSyntax && !objectSyntax.Properties.Any())
                {
                    // Empty object - okay
                    continue;
                }
                else if (defaultValue is ExpressionSyntax expressionSyntax && ExpressionContainsNewGuid(expressionSyntax))
                {
                    // Contains a call to newGuid() - okay
                    continue;
                }

                yield return CreateFixableDiagnosticForSpan(defaultValueSyntax.Span,
                    new CodeFix("Remove default value of secure parameter", true,  // TODO: localize
                            new CodeReplacement(defaultValueSyntax.Span, string.Empty)));
            }
        }

        private class NewGuidVisitor : SyntaxVisitor
        {
            public bool hasNewGuid = false;

            public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            {
                if (syntax.NameEquals("newGuid"))
                {
                    hasNewGuid = true;
                    return;
                }

                base.VisitFunctionCallSyntax(syntax);
            }
        }

        private bool ExpressionContainsNewGuid(ExpressionSyntax expression)
        {
            var visitor = new NewGuidVisitor();
            expression.Accept(visitor);
            return visitor.hasNewGuid;
        }
    }
}
