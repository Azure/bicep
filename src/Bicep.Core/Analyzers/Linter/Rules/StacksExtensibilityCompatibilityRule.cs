// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public class StacksExtensibilityCompatibilityRule : LinterRuleBase
    {
        public new const string Code = "stacks-extensibility-compat";

        public StacksExtensibilityCompatibilityRule() : base(
            code: Code,
            description: CoreResources.StacksExtensibilityCompatibilityRuleDescription,
            category: LinterRuleCategory.DeploymentStackIncompatibility)
        {
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (!model.Features.ModuleExtensionConfigsEnabled) // This rule is being released with this experimental flag.
            {
                return [];
            }

            var visitor = new RuleVisitor(this, model, diagnosticLevel);

            visitor.Visit(model.SourceFile.ProgramSyntax);

            return visitor.Diagnostics;
        }

        private sealed class RuleVisitor : AstVisitor
        {
            public List<IDiagnostic> Diagnostics { get; } = new();

            private StacksExtensibilityCompatibilityRule Rule { get; }

            private SemanticModel Model { get; }

            private DiagnosticLevel DiagnosticLevel { get; }

            private VisitorRecorder<VisitedElement> ElementRecorder { get; } = new();

            public RuleVisitor(StacksExtensibilityCompatibilityRule rule, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                Rule = rule;
                Model = model;
                DiagnosticLevel = diagnosticLevel;
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                if (Model.GetTypeInfo(syntax).IsError())
                {
                    return; // skip error elements
                }

                using var _ = ElementRecorder.Scope(VisitedElement.Module);

                base.VisitModuleDeclarationSyntax(syntax);
            }

            public override void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax)
            {
                if (Model.GetTypeInfo(syntax.Config).IsError())
                {
                    return; // skip error elements
                }

                using var _ = ElementRecorder.Scope(VisitedElement.ExtensionWithClause);

                base.VisitExtensionWithClauseSyntax(syntax);
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                VisitedElement? newVisitedElement = null;

                if (ElementRecorder.TryPeek(out var peek))
                {
                    if (peek is VisitedElement.ExtensionWithClause or VisitedElement.ModuleExtensionConfig)
                    {
                        if (Model.GetDeclaredType(syntax) is { } propertyType and not ErrorType)
                        {
                            ValidateConfigPropertyAssignment(propertyType, syntax.Value);
                        }

                        newVisitedElement = VisitedElement.ExtensionConfigRootProperty; // skip inner objects
                    }
                    else if (peek == VisitedElement.Module && LanguageConstants.IdentifierComparer.Equals(syntax.TryGetKeyText(), LanguageConstants.ModuleExtensionConfigsPropertyName))
                    {
                        newVisitedElement = VisitedElement.ModuleExtensionConfigs;
                    }
                    else if (peek == VisitedElement.ModuleExtensionConfigs && syntax.TryGetKeyText() is not null && Model.GetDeclaredType(syntax) is ObjectType)
                    {
                        newVisitedElement = VisitedElement.ModuleExtensionConfig;
                    }
                }

                using var _ = newVisitedElement is not null ? ElementRecorder.Scope(newVisitedElement.Value) : null;

                base.VisitObjectPropertySyntax(syntax);
            }

            private void ValidateConfigPropertyAssignment(TypeSymbol propertyType, SyntaxBase valueSyntax)
            {
                propertyType = TypeHelper.TryRemoveNullability(propertyType) ?? propertyType;

                if (propertyType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) && !IsKeyVaultReference(valueSyntax))
                {
                    Diagnostics.Add(Rule.CreateDiagnostic(valueSyntax.Span, CoreResources.StacksExtensibilityCompatibilityRule_SecurePropertyValueIsNotReference));
                }

                // NOTE(kylealbert): The non-secure key vault reference case is not flagged with this rule because this is handled by BCP180 already.
            }

            private bool IsKeyVaultReference(SyntaxBase valueSyntax) =>
                valueSyntax switch
                {
                    // NOTE(kylealbert): Due to current limitations with evaluation & emission, only direct instance function call expression is supported.
                    // ParenthesizedExpressionSyntax parenSyntax => IsKeyVaultReference(parenSyntax.Expression),
                    // TernaryOperationSyntax ternarySyntax => IsKeyVaultReference(ternarySyntax.TrueExpression) && IsKeyVaultReference(ternarySyntax.FalseExpression),
                    InstanceFunctionCallSyntax instCallSyntax when IsKeyVaultGetSecretCall(instCallSyntax) || IsAzGetSecretCall(instCallSyntax) => true,
                    _ => false
                };

            private bool IsKeyVaultGetSecretCall(InstanceFunctionCallSyntax instCallSyntax) =>
                Model.Binder.GetSymbolInfo(instCallSyntax.BaseExpression) is ResourceSymbol { Type: ResourceType resourceType }
                && LanguageConstants.ResourceTypeComparer.Equals(resourceType.TypeReference.Type, AzResourceTypeProvider.ResourceTypeKeyVault)
                && LanguageConstants.IdentifierComparer.Equals(AzResourceTypeProvider.GetSecretFunctionName, instCallSyntax.Name.IdentifierName);

            private bool IsAzGetSecretCall(InstanceFunctionCallSyntax instCallSyntax) =>
                Model.Binder.GetSymbolInfo(instCallSyntax.BaseExpression) is BuiltInNamespaceSymbol nsSymbol
                && LanguageConstants.ExtensionNameComparer.Equals(nsSymbol.Name, AzNamespaceType.BuiltInName)
                && LanguageConstants.IdentifierComparer.Equals(AzNamespaceType.GetSecretFunctionName, instCallSyntax.Name.IdentifierName);

            private enum VisitedElement
            {
                ExtensionWithClause,
                Module,
                ModuleExtensionConfig,
                ModuleExtensionConfigs,
                ExtensionConfigRootProperty
            }
        }
    }
}
