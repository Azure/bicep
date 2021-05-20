// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public class ResourceDependencyFinderVisitor : SyntaxVisitor // TODO: rename.  ResourceImplicitDependenciesVisitor?
    {
        private readonly SemanticModel semanticModel;
        private readonly HashSet<DeclaredSymbol> resourceDependencies;

        private ResourceDependencyFinderVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.resourceDependencies = new HashSet<DeclaredSymbol>();
        }

        // TODO: rename.  GetResourcesReferenced?
        public static HashSet<DeclaredSymbol> GetResourceDependencies(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var visitor = new ResourceDependencyFinderVisitor(semanticModel);
            visitor.Visit(syntax);

            return visitor.resourceDependencies;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Do not traverse into child resources - references inside a child to resource1
            //   should not imply a reference from the parent to resource1
            return;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var symbol = semanticModel.GetSymbolInfo(syntax);
            switch (symbol)
            {
                case ResourceSymbol resourceSymbol:
                    resourceDependencies.Add(resourceSymbol);
                    return;
                case ModuleSymbol moduleSymbol:
                    resourceDependencies.Add(moduleSymbol);
                    return;
                case VariableSymbol variableSymbol: // Loop variables (TODO:?  test)
                    Visit(variableSymbol.DeclaringSyntax);
                    return;
                default:
                    return;
            }
        }
    }
}
