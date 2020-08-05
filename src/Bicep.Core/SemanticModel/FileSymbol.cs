﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Errors;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class FileSymbol : DeclaredSymbol
    {
        public FileSymbol(
            ISemanticContext context,
            string name,
            ProgramSyntax declaringSyntax,
            IEnumerable<ParameterSymbol> parameterDeclarations,
            IEnumerable<VariableSymbol> variableDeclarations,
            IEnumerable<ResourceSymbol> resourceDeclarations,
            IEnumerable<OutputSymbol> outputDeclarations)
            : base(context, name, declaringSyntax)
        {
            this.ParameterDeclarations = parameterDeclarations.ToImmutableArray();
            this.VariableDeclarations = variableDeclarations.ToImmutableArray();
            this.ResourceDeclarations = resourceDeclarations.ToImmutableArray();
            this.OutputDeclarations = outputDeclarations.ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.ParameterDeclarations
            .Concat<Symbol>(this.VariableDeclarations)
            .Concat(this.ResourceDeclarations)
            .Concat(this.OutputDeclarations);

        public override SymbolKind Kind => SymbolKind.File;

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public ImmutableArray<VariableSymbol> VariableDeclarations { get; }

        public ImmutableArray<ResourceSymbol> ResourceDeclarations { get; }

        public ImmutableArray<OutputSymbol> OutputDeclarations { get; }

        public IEnumerable<DeclaredSymbol> AllDeclarations => this.Descendants.OfType<DeclaredSymbol>();

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            var duplicateSymbols = this.AllDeclarations
                .GroupBy(decl => decl.Name)
                .Where(group => group.Count() > 1);
            
            foreach (IGrouping<string, DeclaredSymbol> group in duplicateSymbols)
            {
                foreach (DeclaredSymbol duplicatedSymbol in group)
                {
                    // use the identifier node as the error location with fallback to full declaration span
                    SyntaxBase identifierNode = duplicatedSymbol.NameSyntax ?? duplicatedSymbol.DeclaringSyntax;

                    yield return this.CreateError(identifierNode, b => b.IdentifierMultipleDeclarations(duplicatedSymbol.Name));
                }
            }
        }

        public override SyntaxBase? NameSyntax => null;
    }
}