// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    /// <summary>
    /// Module extension configs must evaluate to the object structure expected by the deployments API. This includes an extra object layer
    /// to differentiate value and key vault references. There currently isn't a way to inject this extra layer when using non-object literal
    /// expressions, so those cases need to be blocked. There is one exception for the case where extension configurations are inherited because
    /// those expressions should return the expected object structure, but only if those expressions align correctly.
    /// </summary>
    public static class ModuleExtensionConfigsLimitations
    {
        public static void Validate(ObjectSyntax moduleBody, SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            if (!model.Features.ModuleExtensionConfigsEnabled)
            {
                return;
            }

            var extensionConfigsValue = moduleBody.TryGetPropertyByName(LanguageConstants.ModuleExtensionConfigsPropertyName)?.Value;

            if (extensionConfigsValue is null or SkippedTriviaSyntax)
            {
                return;
            }

            if (extensionConfigsValue is not ObjectSyntax extConfigsObject)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(extensionConfigsValue).PropertyRequiresObjectLiteral(LanguageConstants.ModuleExtensionConfigsPropertyName));

                return;
            }

            foreach (var extConfigAssignmentProperty in extConfigsObject.Properties)
            {
                ValidateExtensionConfigAssignment(extConfigAssignmentProperty, model, diagnosticWriter);
            }
        }

        /// <summary>
        /// Validates the property syntax for an extension's config assignment. This is the whole extension config.
        /// </summary>
        /// <returns>true if no diagnostics raised, false otherwise</returns>
        private static void ValidateExtensionConfigAssignment(ObjectPropertySyntax syntax, SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            if (syntax.Value is null or SkippedTriviaSyntax)
            {
                return; // other diagnostics will be raised for these cases
            }

            if (syntax.Value is not ObjectSyntax && !ResolvesToExpectedExtensionConfigExpression(syntax.Value, model))
            {
                var propertyName = syntax.TryGetKeyText() ?? "<unknown>";
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InvalidModuleExtensionConfigAssignmentExpression(propertyName));
            }
        }

        private static bool ResolvesToExpectedExtensionConfigExpression(SyntaxBase expression, SemanticModel model) =>
            expression switch
            {
                ParenthesizedExpressionSyntax parenSyntax => ResolvesToExpectedExtensionConfigExpression(parenSyntax.Expression, model),
                TernaryOperationSyntax ternarySyntax => ResolvesToExpectedExtensionConfigExpression(ternarySyntax.TrueExpression, model)
                    && ResolvesToExpectedExtensionConfigExpression(ternarySyntax.FalseExpression, model),
                AccessExpressionSyntax accessSyntax => IsExtConfigAccess(accessSyntax, model),
                _ => false
            };

        private static bool IsExtConfigAccess(AccessExpressionSyntax accessSyntax, SemanticModel model)
        {
            var baseExpressionChain = accessSyntax.GetBaseExpressionChain();

            if (baseExpressionChain.Count != 1 || model.Binder.GetSymbolInfo(baseExpressionChain[0]) is not ExtensionNamespaceSymbol)
            {
                return false;
            }

            return LanguageConstants.IdentifierComparer.Equals(accessSyntax.TryGetPropertyName(), LanguageConstants.ExtensionConfigPropertyName);
        }
    }
}
