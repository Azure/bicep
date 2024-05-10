// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
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

            var declartionsBySyntax = ImmutableDictionary.CreateBuilder<SyntaxBase, DeclaredSymbol>();
            var providerDeclarations = ImmutableArray.CreateBuilder<ProviderNamespaceSymbol>();
            var metadataDeclarations = ImmutableArray.CreateBuilder<MetadataSymbol>();
            var parameterDeclarations = ImmutableArray.CreateBuilder<ParameterSymbol>();
            var typeDeclarations = ImmutableArray.CreateBuilder<TypeAliasSymbol>();
            var variableDeclarations = ImmutableArray.CreateBuilder<VariableSymbol>();
            var functionDeclarations = ImmutableArray.CreateBuilder<DeclaredFunctionSymbol>();
            var resourceDeclarations = ImmutableArray.CreateBuilder<ResourceSymbol>();
            var moduleDeclarations = ImmutableArray.CreateBuilder<ModuleSymbol>();
            var outputDeclarations = ImmutableArray.CreateBuilder<OutputSymbol>();
            var assertDeclarations = ImmutableArray.CreateBuilder<AssertSymbol>();
            var parameterAssignments = ImmutableArray.CreateBuilder<ParameterAssignmentSymbol>();
            var testDeclarations = ImmutableArray.CreateBuilder<TestSymbol>();
            var importedTypes = ImmutableArray.CreateBuilder<ImportedTypeSymbol>();
            var importedVariables = ImmutableArray.CreateBuilder<ImportedVariableSymbol>();
            var importedFunctions = ImmutableArray.CreateBuilder<ImportedFunctionSymbol>();
            var erroredImports = ImmutableArray.CreateBuilder<ErroredImportSymbol>();
            var wildcardImports = ImmutableArray.CreateBuilder<WildcardImportSymbol>();

            foreach (var declaration in fileScope.Declarations)
            {
                declartionsBySyntax.Add(declaration.DeclaringSyntax, declaration);

                switch (declaration)
                {
                    case ProviderNamespaceSymbol providerNamespace:
                        providerDeclarations.Add(providerNamespace);
                        break;
                    case MetadataSymbol metadata:
                        metadataDeclarations.Add(metadata);
                        break;
                    case ParameterSymbol parameter:
                        parameterDeclarations.Add(parameter);
                        break;
                    case TypeAliasSymbol typeAlias:
                        typeDeclarations.Add(typeAlias);
                        break;
                    case VariableSymbol variable:
                        variableDeclarations.Add(variable);
                        break;
                    case DeclaredFunctionSymbol declaredFunction:
                        functionDeclarations.Add(declaredFunction);
                        break;
                    case ResourceSymbol resource:
                        resourceDeclarations.Add(resource);
                        break;
                    case ModuleSymbol module:
                        moduleDeclarations.Add(module);
                        break;
                    case OutputSymbol output:
                        outputDeclarations.Add(output);
                        break;
                    case AssertSymbol assertion:
                        assertDeclarations.Add(assertion);
                        break;
                    case ParameterAssignmentSymbol parameterAssignment:
                        parameterAssignments.Add(parameterAssignment);
                        break;
                    case TestSymbol test:
                        testDeclarations.Add(test);
                        break;
                    case ImportedTypeSymbol importedType:
                        importedTypes.Add(importedType);
                        break;
                    case ImportedVariableSymbol importedVariable:
                        importedVariables.Add(importedVariable);
                        break;
                    case ImportedFunctionSymbol importedFunction:
                        importedFunctions.Add(importedFunction);
                        break;
                    case ErroredImportSymbol erroredImport:
                        erroredImports.Add(erroredImport);
                        break;
                    case WildcardImportSymbol wildcardImport:
                        wildcardImports.Add(wildcardImport);
                        break;
                }
            }

            DeclarationsBySyntax = declartionsBySyntax.ToImmutable();
            ProviderDeclarations = providerDeclarations.ToImmutable();
            MetadataDeclarations = metadataDeclarations.ToImmutable();
            ParameterDeclarations = parameterDeclarations.ToImmutable();
            TypeDeclarations = typeDeclarations.ToImmutable();
            VariableDeclarations = variableDeclarations.ToImmutable();
            FunctionDeclarations = functionDeclarations.ToImmutable();
            ResourceDeclarations = resourceDeclarations.ToImmutable();
            ModuleDeclarations = moduleDeclarations.ToImmutable();
            OutputDeclarations = outputDeclarations.ToImmutable();
            AssertDeclarations = assertDeclarations.ToImmutable();
            ParameterAssignments = parameterAssignments.ToImmutable();
            TestDeclarations = testDeclarations.ToImmutable();
            ImportedTypes = importedTypes.ToImmutable();
            ImportedVariables = importedVariables.ToImmutable();
            ImportedFunctions = importedFunctions.ToImmutable();
            ErroredImports = erroredImports.ToImmutable();
            WildcardImports = wildcardImports.ToImmutable();

            this.declarationsByName = this.Declarations.ToLookup(decl => decl.Name, LanguageConstants.IdentifierComparer);

            this.usingDeclarationLazy = new Lazy<UsingDeclarationSyntax?>(() => this.Syntax.Children.OfType<UsingDeclarationSyntax>().FirstOrDefault());
        }

        public override IEnumerable<Symbol> Descendants =>
            this.NamespaceResolver.ImplicitNamespaces.Values
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
            .Concat(this.ImportedTypes)
            .Concat(this.ImportedVariables)
            .Concat(this.ImportedFunctions)
            .Concat(this.ErroredImports)
            .Concat(this.WildcardImports);

        public IEnumerable<Symbol> Namespaces =>
            this.NamespaceResolver.ImplicitNamespaces.Values
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

        public ImmutableArray<ImportedTypeSymbol> ImportedTypes { get; }

        public ImmutableArray<ImportedVariableSymbol> ImportedVariables { get; }

        public ImmutableArray<ImportedFunctionSymbol> ImportedFunctions { get; }

        public ImmutableArray<ErroredImportSymbol> ErroredImports { get; }

        public IEnumerable<ImportedSymbol> ImportedSymbols => ImportedTypes
            .Concat<ImportedSymbol>(ImportedVariables)
            .Concat(ImportedFunctions);

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
        public Result<ISemanticModel, ErrorDiagnostic> TryGetBicepFileSemanticModelViaUsing()
        {
            if (this.FileKind == BicepSourceFileKind.BicepFile)
            {
                throw new InvalidOperationException($"File '{this.FileUri}' cannot reference another Bicep file via a using declaration.");
            }

            var usingDeclaration = this.UsingDeclarationSyntax;
            if (usingDeclaration is null)
            {
                // missing using
                return new(DiagnosticBuilder.ForDocumentStart().UsingDeclarationNotSpecified());
            }

            return SemanticModelHelper.TryGetTemplateModelForArtifactReference(
                Context.Compilation.SourceFileGrouping,
                usingDeclaration,
                b => b.UsingDeclarationMustReferenceBicepFile(),
                Context.Compilation);
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
                var visitor = new DuplicateIdentifierValidatorVisitor(file.NamespaceResolver.ImplicitNamespaces);
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
                    .Select(kvp => DiagnosticBuilder.ForPosition(kvp.Key.DeclaringProvider.SpecificationString).NamespaceMultipleDeclarations(kvp.Value.ProviderName)));
            }

            private static IEnumerable<DeclaredSymbol> FindDuplicateNamedSymbols(IEnumerable<DeclaredSymbol> symbols)
                => symbols
                .Where(decl => decl.NameSource.IsValid)
                .GroupBy(decl => decl.Name, LanguageConstants.IdentifierComparer)
                .Where(group => group.Count() > 1)
                .SelectMany(group => group);

            private static IEnumerable<KeyValuePair<ProviderNamespaceSymbol, NamespaceType>> FindDuplicateNamespaceImports(IEnumerable<ProviderNamespaceSymbol> symbols)
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
                    .GroupBy(kvp => kvp.Value.ProviderName, LanguageConstants.IdentifierComparer)
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group);
            }
        }
    }
}
