// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;

namespace Bicep.Core.Visitors
{
    public class ResourceDependencyFinderVisitor : AstVisitor
    {
        private readonly SemanticModel semanticModel;
        private readonly HashSet<DeclaredSymbol> resourceDependencies;

        private ResourceDependencyFinderVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.resourceDependencies = new HashSet<DeclaredSymbol>();
        }

        public static HashSet<DeclaredSymbol> GetResourceDependencies(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var visitor = new ResourceDependencyFinderVisitor(semanticModel);
            visitor.Visit(syntax);

            return visitor.resourceDependencies;
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
                case VariableSymbol variableSymbol:
                    Visit(variableSymbol.DeclaringSyntax);
                    return;
                default:
                    return;
            }
        }
    }
}
