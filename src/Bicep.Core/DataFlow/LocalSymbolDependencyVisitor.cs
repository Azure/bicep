// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bicep.Core.DataFlow
{
    public sealed class LocalSymbolDependencyVisitor : SyntaxVisitor
    {
        private readonly SemanticModel semanticModel;

        private LocalSymbolDependencyVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        private HashSet<LocalVariableSymbol> SymbolDependencies { get; } = new();

        public static ISet<LocalVariableSymbol> GetLocalSymbolDependencies(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var visitor = new LocalSymbolDependencyVisitor(semanticModel);
            visitor.Visit(syntax);

            return visitor.SymbolDependencies;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var symbol = this.semanticModel.GetSymbolInfo(syntax);
            if(symbol is LocalVariableSymbol local)
            {
                this.SymbolDependencies.Add(local);
            }

            base.VisitVariableAccessSyntax(syntax);
        }
    }
}
