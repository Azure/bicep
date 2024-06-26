// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.DataFlow
{
    public sealed class LocalSymbolDependencyVisitor : AstVisitor
    {
        private readonly SemanticModel semanticModel;

        private LocalSymbolDependencyVisitor(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        private HashSet<LocalVariableSymbol> SymbolDependencies { get; } = new();

        public static ImmutableHashSet<LocalVariableSymbol> GetLocalSymbolDependencies(SemanticModel semanticModel, SyntaxBase syntax)
        {
            var visitor = new LocalSymbolDependencyVisitor(semanticModel);
            visitor.Visit(syntax);

            return [.. visitor.SymbolDependencies];
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var symbol = this.semanticModel.GetSymbolInfo(syntax);
            if (symbol is LocalVariableSymbol local)
            {
                this.SymbolDependencies.Add(local);
            }

            base.VisitVariableAccessSyntax(syntax);
        }
    }
}
