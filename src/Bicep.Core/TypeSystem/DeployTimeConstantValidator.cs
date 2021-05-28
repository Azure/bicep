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

namespace Bicep.Core.TypeSystem
{
    public static class DeployTimeConstantValidator
    {
        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            // Collect all sytnaxes that require DTCs (a.k.a. DTC containers).
            var containers = DeployTimeConstantContainerVisitor.CollectDeployTimeConstantContainers(semanticModel);

            foreach (var container in containers)
            {
                // Only visit child nodes of the DTC container to avoid flagging the DTC container itself.
                foreach (var childContainer in GetChildrenOfDeployTimeConstantContainer(semanticModel, container))
                {
                    // Validate property accesses, array accesses, resource accesses and function calls.
                    new DeployTimeConstantDirectViolationVisitor(container, semanticModel, diagnosticWriter)
                        .Visit(childContainer);

                    // Validate variable dependencies.
                    foreach (var variableDependency in VariableDependencyVisitor.GetVariableDependencies(semanticModel, childContainer))
                    {
                        new DeployTimeConstantIndirectViolationVisitor(container, variableDependency, semanticModel, diagnosticWriter)
                            .Visit(variableDependency);
                    }
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
