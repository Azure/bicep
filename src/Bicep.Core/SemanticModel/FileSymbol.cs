// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class FileSymbol : Symbol
    {
        public FileSymbol(string name,
            ProgramSyntax syntax,
            ImmutableDictionary<string, NamespaceSymbol> importedNamespaces,
            IEnumerable<ParameterSymbol> parameterDeclarations,
            IEnumerable<VariableSymbol> variableDeclarations,
            IEnumerable<ResourceSymbol> resourceDeclarations,
            IEnumerable<ModuleSymbol> moduleDeclarations,
            IEnumerable<OutputSymbol> outputDeclarations)
            : base(name)
        {
            this.Syntax = syntax;
            this.ImportedNamespaces = importedNamespaces;
            this.ParameterDeclarations = parameterDeclarations.ToImmutableArray();
            this.VariableDeclarations = variableDeclarations.ToImmutableArray();
            this.ResourceDeclarations = resourceDeclarations.ToImmutableArray();
            this.ModuleDeclarations = moduleDeclarations.ToImmutableArray();
            this.OutputDeclarations = outputDeclarations.ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.ImportedNamespaces.Values
            .Concat<Symbol>(this.ParameterDeclarations)
            .Concat(this.VariableDeclarations)
            .Concat(this.ResourceDeclarations)
            .Concat(this.ModuleDeclarations)
            .Concat(this.OutputDeclarations);

        public override SymbolKind Kind => SymbolKind.File;

        public ProgramSyntax Syntax { get; }

        public ImmutableDictionary<string, NamespaceSymbol> ImportedNamespaces { get; }

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public ImmutableArray<VariableSymbol> VariableDeclarations { get; }

        public ImmutableArray<ResourceSymbol> ResourceDeclarations { get; }

        public ImmutableArray<ModuleSymbol> ModuleDeclarations { get; }

        public ImmutableArray<OutputSymbol> OutputDeclarations { get; }
        
        public IEnumerable<DeclaredSymbol> AllDeclarations => this.Descendants.OfType<DeclaredSymbol>();

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            // only consider declarations with valid identifiers
            var duplicateSymbols = this.AllDeclarations
                .Where(decl => decl.NameSyntax.IsValid)
                .GroupBy(decl => decl.Name)
                .Where(group => group.Count() > 1);

            foreach (IGrouping<string, DeclaredSymbol> group in duplicateSymbols)
            {
                foreach (DeclaredSymbol duplicatedSymbol in group)
                {
                    yield return this.CreateError(duplicatedSymbol.NameSyntax, b => b.IdentifierMultipleDeclarations(duplicatedSymbol.Name));
                }
            }

            var namespaceKeys = this.ImportedNamespaces.Keys;
            var reservedSymbols = this.AllDeclarations
                .Where(decl => decl.NameSyntax.IsValid)
                .Where(decl => namespaceKeys.Contains(decl.Name));

            foreach (DeclaredSymbol reservedSymbol in reservedSymbols)
            {
                yield return this.CreateError(
                    reservedSymbol.NameSyntax, 
                    b => b.SymbolicNameCannotUseReservedNamespaceName(
                        reservedSymbol.Name,
                        namespaceKeys));
            }
        }
    }
}
