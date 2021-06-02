// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public abstract class DeployTimeConstantViolationVisitor : SyntaxVisitor
    {
        public DeployTimeConstantViolationVisitor(
            SyntaxBase deployTimeConstantContainer,
            SemanticModel semanticModel,
            IDiagnosticWriter diagnosticWriter)
        {
            this.DeployTimeConstantContainer = deployTimeConstantContainer;
            this.SemanticModel = semanticModel;
            this.DiagnosticWriter = diagnosticWriter;
        }

        protected SyntaxBase DeployTimeConstantContainer { get; }

        protected SemanticModel SemanticModel { get; }

        protected IDiagnosticWriter DiagnosticWriter { get; }

        protected void FlagDeployTimeConstantViolation(SyntaxBase errorSyntax, DeclaredSymbol? accessedSymbol = null, ObjectType? accessedObjectType = null, IEnumerable<string>? variableDependencyChain = null)
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
                    diagnosticBuilder.RuntimeValueNotAllowedInVariableForBody(variableName, accessedSymbolName, accessiblePropertyNames, variableDependencyChain),

                ForSyntax => diagnosticBuilder.RuntimeValueNotAllowedInForExpression(accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                FunctionCallSyntaxBase functionCallSyntaxBase => diagnosticBuilder.RuntimeValueNotAllowedInRunTimeFunctionArguments(functionCallSyntaxBase.Name.IdentifierName, accessedSymbolName, accessiblePropertyNames, variableDependencyChain),
                _ => throw new ArgumentOutOfRangeException(nameof(this.DeployTimeConstantContainer), "Expected an ObjectPropertySyntax with a propertyName, a IfConditionSyntax, a ForSyntax, or a FunctionCallSyntaxBase."),
            };

            this.DiagnosticWriter.Write(diagnostic);
        }

        private string? ErrorSyntaxInForBodyOfVariable(ForSyntax forSyntax, SyntaxBase errorSyntax) =>
            this.SemanticModel.Binder.GetParent(forSyntax) is VariableDeclarationSyntax variableDeclarationSyntax &&
            TextSpan.AreOverlapping(errorSyntax, forSyntax.Body)
                ? variableDeclarationSyntax.Name.IdentifierName
                : null;

        protected (DeclaredSymbol?, ObjectType?) TryExtractResourceOrModuleSymbolAndBodyType(SyntaxBase syntax, bool isCollection)
        {
            if (syntax is ArrayAccessSyntax { BaseExpression: var baseSyntax })
            {
                return TryExtractResourceOrModuleSymbolAndBodyType(baseSyntax, true);
            }

            return this.SemanticModel.GetSymbolInfo(syntax) switch
            {
                ResourceSymbol resourceSymbol when resourceSymbol.IsCollection == isCollection => (resourceSymbol, resourceSymbol.TryGetBodyObjectType()),
                ModuleSymbol moduleSymbol when moduleSymbol.IsCollection == isCollection => (moduleSymbol, moduleSymbol.TryGetBodyObjectType()),
                _ => (null, null),
            };
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

            if (accessedSymbol is ResourceSymbol { DeclaringResource: var declaringResource } &&
                declaringResource.TryGetBody() is { } bodySyntax)
            {
                var declaredTopLevelPropertyNames = bodySyntax.ToNamedPropertyDictionary().Keys
                    .Append(LanguageConstants.ResourceIdPropertyName)
                    .Append(LanguageConstants.ResourceTypePropertyName)
                    .Append(LanguageConstants.ResourceApiVersionPropertyName);

                accessiblePropertyNames = accessiblePropertyNames.Intersect(declaredTopLevelPropertyNames, LanguageConstants.IdentifierComparer);
            }

            return accessiblePropertyNames;
        }
    }
}
