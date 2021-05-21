// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
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
            if (syntax.TryGetKeyText() is { } propertyName &&
                this.semanticModel.Binder.GetParent(syntax) is { } objectSyntax &&
                this.semanticModel.TypeManager.GetDeclaredType(objectSyntax) is ObjectType objectType &&
                objectType.Properties.TryGetValue(propertyName, out var propertyType) &&
                propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
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
            if (this.semanticModel.GetSymbolInfo(syntax) is FunctionSymbol { FunctionFlags: FunctionFlags.RequiresInlining })
            {
                this.deployTimeConstantContainers.Add(syntax);
            }
        }
    }
}
