// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Collects syntaxes that only accept deploy-time constant values (deploy-time constant containers).
    /// </summary>
    public class DeployTimeConstantContainerVisitor : SyntaxVisitor
    {
        private readonly SemanticModel semanticModel;

        private readonly List<SyntaxBase> deployTimeConstantContainers = new();

        public DeployTimeConstantContainerVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public static IEnumerable<SyntaxBase> CollectDeployTimeConstantContainers(SemanticModel semanticModel)
        {
            var visitor = new DeployTimeConstantContainerVisitor(semanticModel);

            visitor.Visit(semanticModel.Root.Syntax);

            return visitor.deployTimeConstantContainers;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.TryGetTypeProperty(this.semanticModel.Binder, this.semanticModel.TypeManager) is { } typeProperty &&
                typeProperty.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
            {
                // The property type exists and and has the DTC flag.
                this.deployTimeConstantContainers.Add(syntax);
            }

            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            deployTimeConstantContainers.Add(syntax);

            base.VisitIfConditionSyntax(syntax);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            deployTimeConstantContainers.Add(syntax);

            base.VisitForSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.CollectIfFunctionRequiresInlining(syntax);

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.CollectIfFunctionRequiresInlining(syntax);

            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        private void CollectIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (this.semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                this.deployTimeConstantContainers.Add(syntax);
            }
        }
    }
}
