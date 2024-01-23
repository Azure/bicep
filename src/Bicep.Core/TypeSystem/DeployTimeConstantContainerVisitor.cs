// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Collects syntaxes that only accept deploy-time constant values (deploy-time constant containers).
    /// </summary>
    public class DeployTimeConstantContainerVisitor : AstVisitor
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

        public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
        {
            this.deployTimeConstantContainers.Add(syntax);

            base.VisitFunctionDeclarationSyntax(syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (syntax.IsExistingResource())
            {
                // DTC validation should be skipped for existing resource properties,
                // so only visiting nested resources inside the existing resource.
                var body = syntax.TryGetBody();

                if (body is not null)
                {
                    foreach (var nestedResource in body.Resources)
                    {
                        this.Visit(nestedResource);
                    }
                }
            }
            else
            {
                base.VisitResourceDeclarationSyntax(syntax);
            }
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.TryGetTypeProperty(this.semanticModel) is { } typeProperty &&
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
