// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParameterDefaultRule : LinterRuleBase
    {
        public new const string Code = "secure-parameter-default";

        public SecureParameterDefaultRule() : base(
            code: Code,
            description: CoreResources.SecureParameterDefaultRuleDescription,
            LinterRuleCategory.Security)
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
                else if (model.GetTypeInfo(defaultValue).ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                {
                    // has @secure attribute - okay
                    continue;
                }
                else if (defaultValue is ObjectSyntax objectSyntax && !objectSyntax.Properties.Any())
                {
                    // Empty object - okay
                    continue;
                }
                else if (defaultValue is ExpressionSyntax expressionSyntax && ExpressionContainsNewGuid(model, expressionSyntax))
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

        private static bool ExpressionContainsNewGuid(SemanticModel model, ExpressionSyntax expression)
            => SemanticModelHelper.GetFunctionsByName(model, SystemNamespaceType.BuiltInName, "newGuid", expression).Any();
    }
}
