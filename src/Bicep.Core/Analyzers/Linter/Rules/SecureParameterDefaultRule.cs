// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParameterDefaultRule : LinterRuleBase
    {
        public new const string Code = "secure-parameter-default";

        public SecureParameterDefaultRule() : base(
            code: Code,
            description: CoreResources.SecureParameterDefaultRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var defaultValueSyntaxes = model.Root.ParameterDeclarations.Where(p => p.IsSecure())
                .Select(p => p.DeclaringParameter.Modifier as ParameterDefaultValueSyntax)
                .OfType<ParameterDefaultValueSyntax>(); // this eliminates nulls (when there's no default value)

            foreach (var defaultValueSyntax in defaultValueSyntaxes)
            {
                var defaultValue = defaultValueSyntax.DefaultValue;
                if (defaultValue is StringSyntax defaultString
                    && !defaultString.IsInterpolated()
                    && defaultString.TryGetLiteralValue() == "")
                {
                    // Empty string - okay
                    continue;
                }
                else if (defaultValue is VariableAccessSyntax variableAccessSyntax)
                {
                    // Variable - okay
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

                yield return CreateFixableDiagnosticForSpan(diagnosticLevel,
                    defaultValueSyntax.Span,
                    new CodeFix(CoreResources.SecureParameterDefaultFixTitle, true, CodeFixKind.QuickFix,
                            new CodeReplacement(defaultValueSyntax.Span, string.Empty)));
            }
        }

        private class NewGuidVisitor : AstVisitor
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

        private static bool ExpressionContainsNewGuid(ExpressionSyntax expression)
        {
            var visitor = new NewGuidVisitor();
            expression.Accept(visitor);
            return visitor.hasNewGuid;
        }
    }
}
