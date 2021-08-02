// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class FileSymbol : Symbol, ILanguageScope
    {
        private readonly ILookup<string, DeclaredSymbol> declarationsByName;

        public FileSymbol(string name,
            ProgramSyntax syntax,
            ImmutableDictionary<string, NamespaceSymbol> importedNamespaces,
            IEnumerable<LocalScope> outermostScopes,
            IEnumerable<ParameterSymbol> parameterDeclarations,
            IEnumerable<VariableSymbol> variableDeclarations,
            IEnumerable<ResourceSymbol> resourceDeclarations,
            IEnumerable<ModuleSymbol> moduleDeclarations,
            IEnumerable<OutputSymbol> outputDeclarations,
            Uri fileUri)
            : base(name)
        {
            this.Syntax = syntax;
            this.ImportedNamespaces = importedNamespaces;
            FileUri = fileUri;
            this.LocalScopes = outermostScopes.ToImmutableArray();

            this.ParameterDeclarations = parameterDeclarations.ToImmutableArray();
            this.VariableDeclarations = variableDeclarations.ToImmutableArray();
            this.ResourceDeclarations = resourceDeclarations.ToImmutableArray();
            this.ModuleDeclarations = moduleDeclarations.ToImmutableArray();
            this.OutputDeclarations = outputDeclarations.ToImmutableArray();

            this.declarationsByName = this.Declarations.ToLookup(decl => decl.Name, LanguageConstants.IdentifierComparer);
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

        public ImmutableArray<LocalScope> LocalScopes { get; }

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public ImmutableArray<VariableSymbol> VariableDeclarations { get; }

        public ImmutableArray<ResourceSymbol> ResourceDeclarations { get; }

        public ImmutableArray<ModuleSymbol> ModuleDeclarations { get; }

        public ImmutableArray<OutputSymbol> OutputDeclarations { get; }

        public Uri FileUri { get; }

        /// <summary>
        /// Returns all the top-level declaration symbols.
        /// </summary>
        public IEnumerable<DeclaredSymbol> Declarations => this.Descendants.OfType<DeclaredSymbol>();

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => DuplicateIdentifierValidatorVisitor.GetDiagnostics(this);

        public IEnumerable<DeclaredSymbol> GetDeclarationsByName(string name) => this.declarationsByName[name];

        private sealed class DuplicateIdentifierValidatorVisitor : SymbolVisitor
        {
            private readonly ImmutableDictionary<string, NamespaceSymbol> importedNamespaces;

            private DuplicateIdentifierValidatorVisitor(ImmutableDictionary<string, NamespaceSymbol> importedNamespaces)
            {
                this.importedNamespaces = importedNamespaces;
            }

            public static IEnumerable<ErrorDiagnostic> GetDiagnostics(FileSymbol file)
            {
                var visitor = new DuplicateIdentifierValidatorVisitor(file.ImportedNamespaces);
                visitor.Visit(file);

                return visitor.Diagnostics;
            }

            private IList<ErrorDiagnostic> Diagnostics { get; } = new List<ErrorDiagnostic>();

            protected override void VisitInternal(Symbol node)
            {
                if (node is ILanguageScope scope)
                {
                    ValidateScope(scope);
                }

                base.VisitInternal(node);
            }

            private void ValidateScope(ILanguageScope scope)
            {
                // collect duplicate identifiers at this scope
                // declaring a variable in a local scope hides the parent scope variables,
                // so we don't need to look at other levels
                var outputDeclarations = scope.Declarations.Where(decl => decl is OutputSymbol);
                var nonOutputDeclarations = scope.Declarations.Where(decl => decl is not OutputSymbol);

                // all symbols apart from outputs are in the same namespace, so check for uniqueness.
                this.Diagnostics.AddRange(
                    FindDuplicateNamedSymbols(nonOutputDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.NameSyntax).IdentifierMultipleDeclarations(decl.Name)));

                // output symbols cannot be referenced, so the names declared by them do not need to be unique in the scope.
                // we still need to ensure that they unique among other outputs.
                this.Diagnostics.AddRange(
                    FindDuplicateNamedSymbols(outputDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.NameSyntax).OutputMultipleDeclarations(decl.Name)));

                // imported namespaces are reserved in all the scopes
                // otherwise the user could accidentally hide a namespace which would remove the ability
                // to fully qualify a function
                this.Diagnostics.AddRange(nonOutputDeclarations
                    .Where(decl => decl.NameSyntax.IsValid && this.importedNamespaces.ContainsKey(decl.Name))
                    .Select(reservedSymbol => DiagnosticBuilder.ForPosition(reservedSymbol.NameSyntax).SymbolicNameCannotUseReservedNamespaceName(reservedSymbol.Name, this.importedNamespaces.Keys)));
            }

            private static IEnumerable<DeclaredSymbol> FindDuplicateNamedSymbols(IEnumerable<DeclaredSymbol> symbols)
                => symbols
                .Where(decl => decl.NameSyntax.IsValid)
                .GroupBy(decl => decl.Name, LanguageConstants.IdentifierComparer)
                .Where(group => group.Count() > 1)
                .SelectMany(group => group);
        }
    }
}
