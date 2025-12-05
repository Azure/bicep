// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    public abstract class DeployTimeConstantViolationVisitor : AstVisitor
    {
        public DeployTimeConstantViolationVisitor(
            SyntaxBase deployTimeConstantContainer,
            SemanticModel semanticModel,
            IDiagnosticWriter diagnosticWriter,
            ResourceTypeResolver resourceTypeResolver)
        {
            this.DeployTimeConstantContainer = deployTimeConstantContainer;
            this.SemanticModel = semanticModel;
            this.DiagnosticWriter = diagnosticWriter;
            this.ResourceTypeResolver = resourceTypeResolver;
        }

        protected SyntaxBase DeployTimeConstantContainer { get; }

        protected SemanticModel SemanticModel { get; }

        protected IDiagnosticWriter DiagnosticWriter { get; }

        protected ResourceTypeResolver ResourceTypeResolver { get; }

        protected void FlagDeployTimeConstantViolation(SyntaxBase errorSyntax, DeclaredSymbol? accessedSymbol = null, ObjectType? accessedObjectType = null, IEnumerable<string>? variableDependencyChain = null, string? violatingPropertyName = null)
        {
            var accessedSymbolName = accessedSymbol?.Name;
            var accessiblePropertyNames = GetAccessiblePropertyNames(accessedSymbol, accessedObjectType);

            var containerObjectSyntax = this.DeployTimeConstantContainer is ObjectPropertySyntax
                ? this.SemanticModel.Binder.GetParent(this.DeployTimeConstantContainer) as ObjectSyntax
                : null;
            var containerObjectType = containerObjectSyntax is not null
                ? this.SemanticModel.TypeManager.GetDeclaredType(containerObjectSyntax)
                : null;

            var diagnosticBuilder = DiagnosticBuilder.ForPosition(errorSyntax);
            var diagnostic = this.DeployTimeConstantContainer switch
            {
                ObjectPropertySyntax propertySyntax when propertySyntax.TryGetKeyText() is { } propertyName =>
                    diagnosticBuilder.RuntimeValueNotAllowedInProperty(propertyName, containerObjectType?.Name, accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                IfConditionSyntax => diagnosticBuilder.RuntimeValueNotAllowedInIfConditionExpression(accessedSymbolName, accessiblePropertyNames, variableDependencyChain),

                // Corner case: the runtime value is in the for-body of a variable declaration.
                ForSyntax forSyntax when ErrorSyntaxInForBodyOfVariable(forSyntax, errorSyntax) is string variableName =>
                    diagnosticBuilder.RuntimeValueNotAllowedInVariableForBody(variableName, accessedSymbolName, accessiblePropertyNames, variableDependencyChain, violatingPropertyName),

                ForSyntax => diagnosticBuilder.RuntimeValueNotAllowedInForExpression(accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                FunctionCallSyntaxBase functionCallSyntaxBase => diagnosticBuilder.RuntimeValueNotAllowedInRunTimeFunctionArguments(functionCallSyntaxBase.Name.IdentifierName, accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                FunctionDeclarationSyntax => diagnosticBuilder.RuntimeValueNotAllowedInFunctionDeclaration(accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                FunctionArgumentSyntax arg when GetFunctionAndParameterName(arg) is { } parameterMetadata
                    => diagnosticBuilder.RuntimeValueNotAllowedInFunctionArgument(
                        parameterMetadata.functionName,
                        parameterMetadata.parameterName,
                        accessedSymbolName,
                        accessiblePropertyNames,
                        variableDependencyChain),
                ExtensionWithClauseSyntax when this.SemanticModel.Binder.GetParent(this.DeployTimeConstantContainer) is ExtensionDeclarationSyntax
                    => diagnosticBuilder.RuntimeValueNotAllowedInExtensionDeclarationWithClause(accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                _ => throw new ArgumentOutOfRangeException(nameof(this.DeployTimeConstantContainer), $@"Unexpected syntax type '{this.DeployTimeConstantContainer.GetType().Name}'."),
            };

            this.DiagnosticWriter.Write(diagnostic);
        }

        protected (SyntaxBase? parent, SyntaxBase immediateChild) GetParentAndChildIgnoringNonNullAssertions(SyntaxBase syntax)
            => SemanticModel.Binder.GetParent(syntax) switch
            {
                NonNullAssertionSyntax nonNullAssertion => GetParentAndChildIgnoringNonNullAssertions(nonNullAssertion),
                var parent => (parent, syntax),
            };

        private string? ErrorSyntaxInForBodyOfVariable(ForSyntax forSyntax, SyntaxBase errorSyntax) =>
            this.SemanticModel.Binder.GetParent(forSyntax) is VariableDeclarationSyntax variableDeclarationSyntax &&
            TextSpan.AreOverlapping(errorSyntax, forSyntax.Body)
                ? variableDeclarationSyntax.Name.IdentifierName
                : null;

        private (string? functionName, string? parameterName) GetFunctionAndParameterName(FunctionArgumentSyntax arg)
        {
            if (SemanticModel.Binder.GetParent(arg) is not FunctionCallSyntaxBase func)
            {
                return (null, null);
            }

            var argumentIndex = func.Arguments.IndexOf(arg);
            var parameterName = SemanticModel.TypeManager.GetMatchedFunctionOverload(func) is FunctionOverload overload
                ? (-1 < argumentIndex && argumentIndex < overload.FixedParameters.Length
                    ? overload.FixedParameters[argumentIndex].Name
                    : overload.VariableParameter?.GetNamedSignature(argumentIndex))
                : null;

            return (func.Name.IdentifierName, parameterName ?? argumentIndex.ToString());
        }

        private static IEnumerable<string>? GetAccessiblePropertyNames(DeclaredSymbol? accessedSymbol, ObjectType? accessedObjectType)
        {
            if (accessedSymbol is null || accessedObjectType is null)
            {
                return null;
            }

            var accessiblePropertyNames = accessedObjectType.Properties
                .Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) && !kv.Value.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(kv => kv.Key);

            if (accessedSymbol is ResourceSymbol resourceSymbol)
            {
                IEnumerable<string> alwaysPresentTopLevelPropertyNames
                    = [AzResourceTypeProvider.ResourceTypePropertyName, AzResourceTypeProvider.ResourceApiVersionPropertyName];

                if (resourceSymbol.TryGetResourceType()?.IsAzResource() is true)
                {
                    string[] azAlwaysPresentTopLevelPropertyNames
                        = [AzResourceTypeProvider.ResourceIdPropertyName, AzResourceTypeProvider.ResourceNamePropertyName];
                    alwaysPresentTopLevelPropertyNames = alwaysPresentTopLevelPropertyNames.Concat(azAlwaysPresentTopLevelPropertyNames);
                }

                var declaredTopLevelPropertyNames = resourceSymbol.DeclaringResource.TryGetBody()?.ToNamedPropertyDictionary().Keys ?? [];

                accessiblePropertyNames = accessiblePropertyNames
                    .Intersect(declaredTopLevelPropertyNames.Concat(alwaysPresentTopLevelPropertyNames), LanguageConstants.IdentifierComparer);
            }

            return accessiblePropertyNames;
        }
    }
}
