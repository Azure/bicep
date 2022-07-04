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

            // compare existing resources to all other existing resources to find dependencies and see if it has a non-DTC value
            // and needs to remove the ReadableAtDeployTime flag from the "name" property -> O(n^2) time complexity with some optimizations
            for (int i = 0; i < existingResourceSymbols.Length; i++)
            {
                int count = existingResourceBodyObjectTypeOverrides.Count;

                foreach (var existingResourceSymbol in existingResourceSymbols)
                {
                    // skip if the existing resource symbol is already in the dictionary because there is no need to check the DTC value again
                    if (existingResourceBodyObjectTypeOverrides.TryGetValue(existingResourceSymbol, out var value))
                    {
                        continue;
                    }

                    // grab the "name" property to check if it has a DTC value or not
                    var nameObjectPropertySyntax = existingResourceSymbol.DeclaringResource.TryGetBody()?.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName);

                    var diagnosticWriterForExistingResources = SimpleDiagnosticWriter.Create();

                    // if "name" property has a non-DTC value, then make an entry for the corresponding existing ResourceSymbol to a modified ObjectType in the dictionary
                    if (nameObjectPropertySyntax != null)
                    {
                        CheckDeployTimeConstantViolations(nameObjectPropertySyntax, nameObjectPropertySyntax.Value, semanticModel, diagnosticWriterForExistingResources, existingResourceBodyObjectTypeOverrides);

                        // if a DTC diagnostic was thrown, map the resource symbol to the new modified ObjectType with the ReadableAtDeployTime flag removed
                        if (diagnosticWriterForExistingResources.HasDiagnostics())
                        {
                            RemoveReadableAtDeployTimeFlagForExistingResource(existingResourceBodyObjectTypeOverrides, existingResourceSymbol);
                        }
                    }
                }

                // if no new entries have been added to existingResourceBodyObjectTypeOverrides, that means there are no further DTC checks needed
                if (count == existingResourceBodyObjectTypeOverrides.Count)
                {
                    break;
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
                    CheckDeployTimeConstantViolations(container, childContainer, semanticModel, diagnosticWriter, existingResourceBodyObjectTypeOverrides);
                }
            }
        }

        public static void CheckDeployTimeConstantViolations(SyntaxBase container, SyntaxBase childContainer, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, Dictionary<DeclaredSymbol, ObjectType> existingResourceBodyObjectTypeOverrides)
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
