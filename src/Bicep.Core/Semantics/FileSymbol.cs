// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class FileSymbol : Symbol, ILanguageScope
    {
        private readonly ILookup<string, DeclaredSymbol> declarationsByName;
        private readonly Lazy<UsingDeclarationSyntax?> usingDeclarationLazy;

        public FileSymbol(
            ISymbolContext context,
            BicepSourceFile sourceFile,
            NamespaceResolver namespaceResolver,
            LocalScope fileScope)
            : base(sourceFile.FileUri.LocalPath)
        {
            this.Context = context;
            this.Syntax = sourceFile.ProgramSyntax;
            this.NamespaceResolver = namespaceResolver;
            this.FileUri = sourceFile.FileUri;
            this.FileKind = sourceFile.FileKind;
            this.LocalScopes = fileScope.ChildScopes;

            // TODO: Avoid looping 12 times?
            this.DeclarationsBySyntax = fileScope.Declarations.ToImmutableDictionary(x => x.DeclaringSyntax);
            this.ProviderDeclarations = fileScope.Declarations.OfType<ProviderNamespaceSymbol>().ToImmutableArray();
            this.MetadataDeclarations = fileScope.Declarations.OfType<MetadataSymbol>().ToImmutableArray();
            this.ParameterDeclarations = fileScope.Declarations.OfType<ParameterSymbol>().ToImmutableArray();
            this.TypeDeclarations = fileScope.Declarations.OfType<TypeAliasSymbol>().ToImmutableArray();
            this.VariableDeclarations = fileScope.Declarations.OfType<VariableSymbol>().ToImmutableArray();
            this.FunctionDeclarations = fileScope.Declarations.OfType<DeclaredFunctionSymbol>().ToImmutableArray();
            this.ResourceDeclarations = fileScope.Declarations.OfType<ResourceSymbol>().ToImmutableArray();
            this.ModuleDeclarations = fileScope.Declarations.OfType<ModuleSymbol>().ToImmutableArray();
            this.OutputDeclarations = fileScope.Declarations.OfType<OutputSymbol>().ToImmutableArray();
            this.AssertDeclarations = fileScope.Declarations.OfType<AssertSymbol>().ToImmutableArray();
            this.ParameterAssignments = fileScope.Declarations.OfType<ParameterAssignmentSymbol>().ToImmutableArray();
            this.TestDeclarations = fileScope.Declarations.OfType<TestSymbol>().ToImmutableArray();
            this.ImportedSymbols = fileScope.Declarations.OfType<ImportedSymbol>().ToImmutableArray();
            this.WildcardImports = fileScope.Declarations.OfType<WildcardImportSymbol>().ToImmutableArray();

            this.declarationsByName = this.Declarations.ToLookup(decl => decl.Name, LanguageConstants.IdentifierComparer);

            this.usingDeclarationLazy = new Lazy<UsingDeclarationSyntax?>(() => this.Syntax.Children.OfType<UsingDeclarationSyntax>().FirstOrDefault());
        }

        public override IEnumerable<Symbol> Descendants =>
            this.NamespaceResolver.BuiltIns.Values
            .Concat<Symbol>(this.ProviderDeclarations)
            .Concat(this.LocalScopes)
            .Concat(this.MetadataDeclarations)
            .Concat(this.ParameterDeclarations)
            .Concat(this.TypeDeclarations)
            .Concat(this.VariableDeclarations)
            .Concat(this.FunctionDeclarations)
            .Concat(this.ResourceDeclarations)
            .Concat(this.ModuleDeclarations)
            .Concat(this.OutputDeclarations)
            .Concat(this.AssertDeclarations)
            .Concat(this.ParameterAssignments)
            .Concat(this.TestDeclarations)
            .Concat(this.ImportedSymbols)
            .Concat(this.WildcardImports);

        public IEnumerable<Symbol> Namespaces =>
            this.NamespaceResolver.BuiltIns.Values
            .Concat<Symbol>(this.ProviderDeclarations);

        public override SymbolKind Kind => SymbolKind.File;

        public BicepSourceFileKind FileKind { get; }

        public ISymbolContext Context { get; }

        public ProgramSyntax Syntax { get; }

        public NamespaceResolver NamespaceResolver { get; }

        public ImmutableArray<LocalScope> LocalScopes { get; }

        public ImmutableDictionary<SyntaxBase, DeclaredSymbol> DeclarationsBySyntax { get; }

        public ImmutableArray<ProviderNamespaceSymbol> ProviderDeclarations { get; }

        public ImmutableArray<MetadataSymbol> MetadataDeclarations { get; }

        public ImmutableArray<ParameterSymbol> ParameterDeclarations { get; }

        public ImmutableArray<TypeAliasSymbol> TypeDeclarations { get; }

        public ImmutableArray<VariableSymbol> VariableDeclarations { get; }

        public ImmutableArray<DeclaredFunctionSymbol> FunctionDeclarations { get; }

        public ImmutableArray<ResourceSymbol> ResourceDeclarations { get; }

        public ImmutableArray<ModuleSymbol> ModuleDeclarations { get; }

        public ImmutableArray<OutputSymbol> OutputDeclarations { get; }

        public ImmutableArray<AssertSymbol> AssertDeclarations { get; }

        public ImmutableArray<TestSymbol> TestDeclarations { get; }

        public ImmutableArray<ParameterAssignmentSymbol> ParameterAssignments { get; }

        public ImmutableArray<ImportedSymbol> ImportedSymbols { get; }

        public ImmutableArray<WildcardImportSymbol> WildcardImports { get; }

        public UsingDeclarationSyntax? UsingDeclarationSyntax => this.usingDeclarationLazy.Value;

        public Uri FileUri { get; }

        /// <summary>
        /// Returns all the top-level declaration symbols.
        /// </summary>
        public IEnumerable<DeclaredSymbol> Declarations => this.Descendants.OfType<DeclaredSymbol>();

        public ScopeResolution ScopeResolution => ScopeResolution.GlobalsOnly;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitFileSymbol(this);
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => DuplicateIdentifierValidatorVisitor.GetDiagnostics(this);

        public IEnumerable<DeclaredSymbol> GetDeclarationsByName(string name) => this.declarationsByName[name];

        /// <summary>
        /// Tries to get the semantic module of the Bicep File referenced via a using declaration from the current file.
        /// If current file is not a parameter file, the method will return false.
        /// </summary>
        public bool TryGetBicepFileSemanticModelViaUsing([NotNullWhen(true)] out SemanticModel? semanticModel, [NotNullWhen(false)] out Diagnostic? failureDiagnostic)
        {
            semanticModel = null;
            failureDiagnostic = null;
            if (this.FileKind == BicepSourceFileKind.BicepFile)
            {
                throw new InvalidOperationException($"File '{this.FileUri}' cannot reference another Bicep file via a using declaration.");
            }

            var usingDeclaration = this.UsingDeclarationSyntax;
            if (usingDeclaration is null)
            {
                // missing using
                failureDiagnostic = DiagnosticBuilder.ForDocumentStart().UsingDeclarationNotSpecified();
                return false;
            }

            if(this.Context.Compilation.SourceFileGrouping.TryGetErrorDiagnostic(usingDeclaration) is { } errorBuilder)
            {
                failureDiagnostic = errorBuilder(DiagnosticBuilder.ForPosition(usingDeclaration.Path));
                return false;
            }

            // SourceFileGroupingBuilder should have already visited every using declaration and either recorded a failure or mapped it to a syntax tree.
            // So it is safe to assume that this lookup will succeed without throwing an exception.
            var sourceFile = Context.Compilation.SourceFileGrouping.TryGetSourceFile(usingDeclaration) ?? throw new InvalidOperationException($"Failed to find source file for using declaration.");
            if(sourceFile is not BicepFile)
            {
                // TODO: If we wanted to support referencing ARM templates via using, it probably wouldn't very difficult to do
                failureDiagnostic = DiagnosticBuilder.ForPosition(usingDeclaration.Path).UsingDeclarationMustReferenceBicepFile();
                return false;
            }

            if (this.Context.Compilation.GetSemanticModel(sourceFile) is SemanticModel model)
            {
                semanticModel = model;
                return true;
            }

            throw new InvalidOperationException($"Unexpected semantic model type when following using declaration.");
        }

        private sealed class DuplicateIdentifierValidatorVisitor : SymbolVisitor
        {
            private readonly ImmutableDictionary<string, BuiltInNamespaceSymbol> builtInNamespaces;

            private DuplicateIdentifierValidatorVisitor(ImmutableDictionary<string, BuiltInNamespaceSymbol> builtInNamespaces)
            {
                this.builtInNamespaces = builtInNamespaces;
            }

            public static IEnumerable<ErrorDiagnostic> GetDiagnostics(FileSymbol file)
            {
                var visitor = new DuplicateIdentifierValidatorVisitor(file.NamespaceResolver.BuiltIns);
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
                var outputDeclarations = scope.Declarations.OfType<OutputSymbol>();
                var metadataDeclarations = scope.Declarations.OfType<MetadataSymbol>();
                var namespaceDeclarations = scope.Declarations.OfType<ProviderNamespaceSymbol>();
                var referenceableDeclarations = scope.Declarations.Where(decl => decl.CanBeReferenced());

                // all symbols apart from outputs are in the same namespace, so check for uniqueness.
                this.Diagnostics.AddRange(
                    FindDuplicateNamedSymbols(referenceableDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.NameSource).IdentifierMultipleDeclarations(decl.Name)));

                // output symbols cannot be referenced, so the names declared by them do not need to be unique in the scope.
                // we still need to ensure that they unique among other outputs.
                this.Diagnostics.AddRange(
                    FindDuplicateNamedSymbols(outputDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.NameSource).OutputMultipleDeclarations(decl.Name)));

                // metadata symbols cannot be referenced, so the names declared by them do not need to be unique in the scope.
                // we still need to ensure that they unique among other metadata.
                this.Diagnostics.AddRange(
                    FindDuplicateNamedSymbols(metadataDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.NameSource).OutputMultipleDeclarations(decl.Name)));

                // imported namespaces are reserved in all the scopes.
                // otherwise the user could accidentally hide a namespace which would remove the ability
                // to fully qualify a function
                this.Diagnostics.AddRange(referenceableDeclarations
                    .Where(decl => decl.NameSource.IsValid && this.builtInNamespaces.ContainsKey(decl.Name))
                    .Select(reservedSymbol => DiagnosticBuilder.ForPosition(reservedSymbol.NameSource).SymbolicNameCannotUseReservedNamespaceName(reservedSymbol.Name, this.builtInNamespaces.Keys)));

                // singleton namespaces cannot be duplicated
                // TODO: validation for alias x name.
                this.Diagnostics.AddRange(
                    FindDuplicateNamespaceImports(namespaceDeclarations)
                    .Select(decl => DiagnosticBuilder.ForPosition(decl.DeclaringImport.Specification).NamespaceMultipleDeclarations(decl.DeclaringImport.Specification.Name)));
            }

            private static IEnumerable<DeclaredSymbol> FindDuplicateNamedSymbols(IEnumerable<DeclaredSymbol> symbols)
                => symbols
                .Where(decl => decl.NameSource.IsValid)
                .GroupBy(decl => decl.Name, LanguageConstants.IdentifierComparer)
                .Where(group => group.Count() > 1)
                .SelectMany(group => group);

            private static IEnumerable<ProviderNamespaceSymbol> FindDuplicateNamespaceImports(IEnumerable<ProviderNamespaceSymbol> symbols)
            {
                var typeBySymbol = new Dictionary<ProviderNamespaceSymbol, NamespaceType>();

                foreach (var symbol in symbols)
                {
                    if (symbol.TryGetNamespaceType() is { } namespaceType)
                    {
                        typeBySymbol[symbol] = namespaceType;
                    }
                }

                return typeBySymbol
                    .Where(kvp => kvp.Value.Settings.IsSingleton)
                    .GroupBy(kvp => kvp.Key.DeclaringImport.Specification.Name, LanguageConstants.IdentifierComparer)
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group.Select(kvp => kvp.Key));
            }
        }
    }
}
