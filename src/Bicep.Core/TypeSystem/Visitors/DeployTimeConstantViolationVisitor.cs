// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Visitors
{
    public abstract class DeployTimeConstantViolationVisitor : AstVisitor
    {
        public DeployTimeConstantViolationVisitor(
            SyntaxBase deployTimeConstantContainer,
            SemanticModel semanticModel,
            IDiagnosticWriter diagnosticWriter,
            ResourceTypeResolver resourceTypeResolver)
        {
            DeployTimeConstantContainer = deployTimeConstantContainer;
            SemanticModel = semanticModel;
            DiagnosticWriter = diagnosticWriter;
            ResourceTypeResolver = resourceTypeResolver;
        }

        protected SyntaxBase DeployTimeConstantContainer { get; }

        protected SemanticModel SemanticModel { get; }

        protected IDiagnosticWriter DiagnosticWriter { get; }

        protected ResourceTypeResolver ResourceTypeResolver { get; }

        protected void FlagDeployTimeConstantViolation(SyntaxBase errorSyntax, DeclaredSymbol? accessedSymbol = null, ObjectType? accessedObjectType = null, IEnumerable<string>? variableDependencyChain = null, string? violatingPropertyName = null)
        {
            var accessedSymbolName = accessedSymbol?.Name;
            var accessiblePropertyNames = GetAccessiblePropertyNames(accessedSymbol, accessedObjectType);

            var containerObjectSyntax = DeployTimeConstantContainer is ObjectPropertySyntax
                ? SemanticModel.Binder.GetParent(DeployTimeConstantContainer) as ObjectSyntax
                : null;
            var containerObjectType = containerObjectSyntax is not null
                ? SemanticModel.TypeManager.GetDeclaredType(containerObjectSyntax)
                : null;

            var diagnosticBuilder = DiagnosticBuilder.ForPosition(errorSyntax);
            var diagnostic = DeployTimeConstantContainer switch
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
                _ => throw new ArgumentOutOfRangeException(nameof(DeployTimeConstantContainer), "Expected an ObjectPropertySyntax with a propertyName, an IfConditionSyntax, a ForSyntax, a FunctionCallSyntaxBase, or a FunctionDeclarationSyntax."),
            };

            DiagnosticWriter.Write(diagnostic);
        }

        private string? ErrorSyntaxInForBodyOfVariable(ForSyntax forSyntax, SyntaxBase errorSyntax) =>
            SemanticModel.Binder.GetParent(forSyntax) is VariableDeclarationSyntax variableDeclarationSyntax &&
            TextSpan.AreOverlapping(errorSyntax, forSyntax.Body)
                ? variableDeclarationSyntax.Name.IdentifierName
                : null;

        private static IEnumerable<string>? GetAccessiblePropertyNames(DeclaredSymbol? accessedSymbol, ObjectType? accessedObjectType)
        {
            if (accessedSymbol is null || accessedObjectType is null)
            {
                return null;
            }

            var accessiblePropertyNames = accessedObjectType.Properties
                .Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) && !kv.Value.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(kv => kv.Key);

            if (accessedSymbol is ResourceSymbol { DeclaringResource: var declaringResource } &&
                declaringResource.TryGetBody() is { } bodySyntax)
            {
                var declaredTopLevelPropertyNames = bodySyntax.ToNamedPropertyDictionary().Keys
                    .Append(AzResourceTypeProvider.ResourceIdPropertyName)
                    .Append(AzResourceTypeProvider.ResourceTypePropertyName)
                    .Append(AzResourceTypeProvider.ResourceApiVersionPropertyName);

                accessiblePropertyNames = accessiblePropertyNames.Intersect(declaredTopLevelPropertyNames, LanguageConstants.IdentifierComparer);
            }

            return accessiblePropertyNames;
        }
    }
}
