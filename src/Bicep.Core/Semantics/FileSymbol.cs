// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class FileSymbol : Symbol, ILanguageScope
    {
        public FileSymbol(string name,
            ProgramSyntax syntax,
            ImmutableDictionary<string, NamespaceSymbol> importedNamespaces,
            IEnumerable<LocalScopeSymbol> outermostScopes,
            IEnumerable<ParameterSymbol> parameterDeclarations,
            IEnumerable<VariableSymbol> variableDeclarations,
            IEnumerable<ResourceSymbol> resourceDeclarations,
            IEnumerable<ModuleSymbol> moduleDeclarations,
            IEnumerable<OutputSymbol> outputDeclarations)
            : base(name)
        {
            this.Syntax = syntax;
            this.ImportedNamespaces = importedNamespaces;
            this.LocalScopes = outermostScopes.ToImmutableArray();
            this.ParameterDeclarations = parameterDeclarations.ToImmutableArray();
            this.VariableDeclarations = variableDeclarations.ToImmutableArray();
            this.ResourceDeclarations = resourceDeclarations.ToImmutableArray();
            this.ModuleDeclarations = moduleDeclarations.ToImmutableArray();
            this.OutputDeclarations = outputDeclarations.ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.ImportedNamespaces.Values
            .Concat<Symbol>(this.LocalScopes)
            .Concat(this.ParameterDeclarations)
            .Concat(this.VariableDeclarations)
            .Concat(this.ResourceDeclarations)
            .Concat(this.ModuleDeclarations)
            .Concat(this.OutputDeclarations);

        public override SymbolKind Kind => SymbolKind.File;

        public ProgramSyntax Syntax { get; }

        public ImmutableDictionary<string, NamespaceSymbol> ImportedNamespaces { get; }

        // TODO: Make this hierarchical at some point.
        public ImmutableArray<LocalScopeSymbol> LocalScopes { get; }

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public ImmutableArray<VariableSymbol> VariableDeclarations { get; }

        public ImmutableArray<ResourceSymbol> ResourceDeclarations { get; }

        public ImmutableArray<ModuleSymbol> ModuleDeclarations { get; }

        public ImmutableArray<OutputSymbol> OutputDeclarations { get; }
        
        /// <summary>
        /// Returns all the top-level declaration symbols.
        /// </summary>
        public IEnumerable<DeclaredSymbol> AllDeclarations => this.Descendants.OfType<DeclaredSymbol>();

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            foreach (var duplicatedSymbol in DuplicateIdentifierDiagnosticCollectorVisitor.GetDuplicateDeclarations(this))
            {
                yield return this.CreateError(duplicatedSymbol.NameSyntax, b => b.IdentifierMultipleDeclarations(duplicatedSymbol.Name));
            }

            // TODO: This isn't aware of locals. Fix it.
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

        public IEnumerable<DeclaredSymbol> GetDeclarationsByName(string name) => this.AllDeclarations.Where(symbol => symbol.NameSyntax.IsValid && string.Equals(symbol.Name, name, LanguageConstants.IdentifierComparison)).ToList();

        private sealed class DuplicateIdentifierDiagnosticCollectorVisitor : SymbolVisitor
        {
            private readonly List<ILanguageScope> activeScopes = new();

            private readonly FileSymbol file;

            private DuplicateIdentifierDiagnosticCollectorVisitor(FileSymbol file)
            {
                this.file = file;

                // initialize global scope
                this.activeScopes.Add(file);
            }

            private HashSet<DeclaredSymbol> Duplicates { get; } = new();
            
            public static IEnumerable<DeclaredSymbol> GetDuplicateDeclarations(FileSymbol file)
            {
                var visitor = new DuplicateIdentifierDiagnosticCollectorVisitor(file);
                visitor.Visit(file);

                return visitor.Duplicates;
            }

            protected override void VisitInternal(Symbol node)
            {
                if (node is DeclaredSymbol declaredSymbol && declaredSymbol.NameSyntax.IsValid)
                {
                    // TODO: There's an 
                    // collect symbols with the same name from current and parent scopes
                    var symbolsWithMatchingName = FindSymbolsByName(declaredSymbol.Name);

                    // we're visiting the symbol now, so we should have at least 1
                    // (0 would be possible if we didn't assert on NameSyntax being valid at the top)
                    Debug.Assert(symbolsWithMatchingName.Count > 0, "symbolsWithMatchingName.Count > 0");

                    // more than 1 in currently active scopes indicates a user error
                    if (symbolsWithMatchingName.Count > 1)
                    {
                        // add to the list of duplicates
                        this.Duplicates.AddRange(symbolsWithMatchingName);
                    }
                }

                base.VisitInternal(node);
            }

            public override void VisitLocalScopeSymbol(LocalScopeSymbol symbol)
            {
                var localSymbols = symbol.DeclaredSymbols.ToLookup(decl => decl.Name, LanguageConstants.IdentifierComparer);
                this.activeScopes.Add(symbol);

                base.VisitLocalScopeSymbol(symbol);

                this.activeScopes.RemoveAt(this.activeScopes.Count - 1);
            }

            private IList<DeclaredSymbol> FindSymbolsByName(string name)
            {
                var symbols = new List<DeclaredSymbol>();

                // iterating from outer to inner scopes
                // the order doesn't matter because we are only collecting symbols
                foreach (var scope in this.activeScopes)
                {
                    // lookups return empty for non-existed keys
                    symbols.AddRange(scope.GetDeclarationsByName(name));
                }

                return symbols;
            }
        }
    }
}
