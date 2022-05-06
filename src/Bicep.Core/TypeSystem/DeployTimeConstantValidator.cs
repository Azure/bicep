// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.TypeSystem
{
    public static class DeployTimeConstantValidator
    {
        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var existingResourceBodyObjectTypeOverrides = new Dictionary<DeclaredSymbol, ObjectType>();

            // grab all existing resources
            var existingResourceSymbols = semanticModel.DeclaredResources
                .Select(x => x.Symbol)
                .Where(x => x.DeclaringResource.IsExistingResource())
                .ToArray();

            // first pass to remove ReadableAtDeployTime flag for existing resources containing a non-DTC value or referencing such resource
            foreach (var existingResourceSymbol in existingResourceSymbols)
            {
                // grab the property for "name" for each existing resource to check if it has a DTC value or not
                var nameObjectPropertySyntax = existingResourceSymbol.DeclaringResource.TryGetBody()?.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName);

                var diagnosticWriterForExistingResourcesFirstPass = ToListDiagnosticWriter.Create();

                // check if name property has DTC value and if it does not, then update the dictionary
                if (nameObjectPropertySyntax != null && !ContainsDeployTimeConstant(nameObjectPropertySyntax, nameObjectPropertySyntax.Value, semanticModel, diagnosticWriterForExistingResourcesFirstPass, existingResourceBodyObjectTypeOverrides))
                {
                    // map the resource symbol to the new modified ObjectType with the ReadableAtDeployTime flag removed
                    RemoveReadableAtDeployTimeFlagForExistingResource(existingResourceBodyObjectTypeOverrides, existingResourceSymbol);
                }
            }

            // second pass over dictionary in reverse order to remove ReadableAtDeployTime flag from any existing resources that may have been missed the first pass
            foreach (var existingResourceSymbol in existingResourceSymbols.Reverse())
            {
                // grab the property for "name" for each existing resource to check if it has a DTC value or not
                var nameObjectPropertySyntax = existingResourceSymbol.DeclaringResource.TryGetBody()?.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName);

                var diagnosticWriterForExistingResourcesSecondPass = ToListDiagnosticWriter.Create();

                // check if name property has DTC value and if it does not, then update the dictionary
                if (nameObjectPropertySyntax != null && !ContainsDeployTimeConstant(nameObjectPropertySyntax, nameObjectPropertySyntax.Value, semanticModel, diagnosticWriterForExistingResourcesSecondPass, existingResourceBodyObjectTypeOverrides))
                {
                    // map the resource symbol to the new modified ObjectType with the ReadableAtDeployTime flag removed
                    RemoveReadableAtDeployTimeFlagForExistingResource(existingResourceBodyObjectTypeOverrides, existingResourceSymbol);
                }
            }

            // now map every resourceSymbol in the dictionary to the same ObjectType but with the DeployTimeConstant flag removed this time
            foreach (var existingResourceSymbol in existingResourceSymbols)
            {
                if (existingResourceBodyObjectTypeOverrides.TryGetValue(existingResourceSymbol, out var value))
                {
                    if (value is { } objectType && value.Properties.TryGetValue(AzResourceTypeProvider.ResourceNamePropertyName, out var nameProperty))
                    {
                        existingResourceBodyObjectTypeOverrides[existingResourceSymbol] = new ObjectType(
                                value.Name,
                                value.ValidationFlags,
                                value.Properties.SetItem(AzResourceTypeProvider.ResourceNamePropertyName, new TypeProperty(nameProperty.Name, LanguageConstants.String, nameProperty.Flags & ~TypePropertyFlags.DeployTimeConstant)).Values,
                                value.AdditionalPropertiesType,
                                value.AdditionalPropertiesFlags,
                                value.MethodResolver.CopyToObject);
                    }
                }
            }

            // Collect all syntaxes that require DTCs (a.k.a. DTC containers).
            var containers = DeployTimeConstantContainerVisitor.CollectDeployTimeConstantContainers(semanticModel);
            foreach (var container in containers)
            {
                // Only visit child nodes of the DTC container to avoid flagging the DTC container itself.
                foreach (var childContainer in GetChildrenOfDeployTimeConstantContainer(semanticModel, container))
                {
                    ContainsDeployTimeConstant(container, childContainer, semanticModel, diagnosticWriter, existingResourceBodyObjectTypeOverrides);
                }
            }
        }

        public static bool ContainsDeployTimeConstant(SyntaxBase container, SyntaxBase childContainer, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, Dictionary<DeclaredSymbol, ObjectType> existingResourceBodyObjectTypeOverrides)
        {
            // Validate property accesses, array accesses, resource accesses and function calls.
            new DeployTimeConstantDirectViolationVisitor(container, semanticModel, diagnosticWriter, existingResourceBodyObjectTypeOverrides)
                .Visit(childContainer);

            // Validate variable dependencies.
            foreach (var variableDependency in VariableDependencyVisitor.GetVariableDependencies(semanticModel, childContainer))
            {
                new DeployTimeConstantIndirectViolationVisitor(container, variableDependency, semanticModel, diagnosticWriter, existingResourceBodyObjectTypeOverrides)
                    .Visit(variableDependency);
            }

            // if there are diagnostics, this means the name property contains a non-DTC value.
            if (diagnosticWriter is ToListDiagnosticWriter toListDiagnosticWriter && toListDiagnosticWriter.GetDiagnostics().Count > 0)
            {
                return false;
            }

            return true;
        }

        public static void RemoveReadableAtDeployTimeFlagForExistingResource(Dictionary<DeclaredSymbol, ObjectType> existingResourceBodyObjectTypeOverrides, ResourceSymbol existingResourceSymbol)
        {
            if (!existingResourceBodyObjectTypeOverrides.TryGetValue(existingResourceSymbol, out var value))
            {
                if (existingResourceSymbol.TryGetBodyObjectType() is { } objectType && objectType.Properties.TryGetValue(AzResourceTypeProvider.ResourceNamePropertyName, out var nameProperty))
                {
                    existingResourceBodyObjectTypeOverrides[existingResourceSymbol] = new ObjectType(
                            objectType.Name,
                            objectType.ValidationFlags,
                            objectType.Properties.SetItem(AzResourceTypeProvider.ResourceNamePropertyName, new TypeProperty(nameProperty.Name, LanguageConstants.String, nameProperty.Flags & ~TypePropertyFlags.ReadableAtDeployTime)).Values,
                            objectType.AdditionalPropertiesType,
                            objectType.AdditionalPropertiesFlags,
                            objectType.MethodResolver.CopyToObject);
                }
            }
        }

        private static IEnumerable<SyntaxBase> GetChildrenOfDeployTimeConstantContainer(SemanticModel semanticModel, SyntaxBase deployTimeConstantContainer) => deployTimeConstantContainer switch
        {
            ObjectPropertySyntax objectPropertySyntax => objectPropertySyntax.Key.AsEnumerable().Concat(objectPropertySyntax.Value),
            IfConditionSyntax ifConditionSyntax => ifConditionSyntax.ConditionExpression.AsEnumerable(),

            // If the ForSyntax is a child of a variable declartion, we should validate both the for-expression and the for-body.
            ForSyntax forSyntax when semanticModel.Binder.GetParent(forSyntax) is VariableDeclarationSyntax => forSyntax.Expression.AsEnumerable().Concat(forSyntax.Body),

            // Only validate the for-expression in other cases.
            ForSyntax forSyntax => forSyntax.Expression.AsEnumerable(),

            FunctionCallSyntaxBase functionCallSyntaxBase => functionCallSyntaxBase.Arguments,
            _ => throw new ArgumentOutOfRangeException(nameof(deployTimeConstantContainer), "Expected an ObjectPropertySyntax, a IfConditionSyntax, a ForSyntax, or a FunctionCallSyntaxBase."),
        };


        private class VariableDependencyVisitor : SyntaxVisitor
        {
            private readonly SemanticModel semanticModel;

            private readonly HashSet<VariableAccessSyntax> variableDependencies = new();

            private VariableDependencyVisitor(SemanticModel semanticModel)
            {
                this.semanticModel = semanticModel;
            }

            public static ImmutableHashSet<VariableAccessSyntax> GetVariableDependencies(SemanticModel semanticModel, SyntaxBase syntax)
            {
                var visitor = new VariableDependencyVisitor(semanticModel);

                visitor.Visit(syntax);

                return visitor.variableDependencies.ToImmutableHashSet();
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                // Ensure the variable symbol is cycle-free, or we'll get a stack overflow.
                if (this.semanticModel.GetSymbolInfo(syntax) is VariableSymbol symbol &&
                    this.semanticModel.Binder.TryGetCycle(symbol) is null)
                {
                    this.variableDependencies.Add(syntax);
                }
            }
        }
    }
}
