// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class SemanticDiagnosticVisitor : SymbolVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;

        public SemanticDiagnosticVisitor(IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
        }

        public override void VisitTypeSymbol(TypeSymbol symbol)
        {
            base.VisitTypeSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitMetadataSymbol(MetadataSymbol symbol)
        {
            base.VisitMetadataSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitParameterSymbol(ParameterSymbol symbol)
        {
            base.VisitParameterSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitTypeAliasSymbol(TypeAliasSymbol symbol)
        {
            base.VisitTypeAliasSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitFileSymbol(FileSymbol symbol)
        {
            base.VisitFileSymbol(symbol);
            this.CollectDiagnostics(symbol);

            // find duplicate target scope declarations
            var targetScopeSyntaxes = symbol.Syntax.Children.OfType<TargetScopeSyntax>().ToImmutableArray();

            if (targetScopeSyntaxes.Length > 1)
            {
                foreach (var targetScope in targetScopeSyntaxes)
                {
                    this.diagnosticWriter.Write(targetScope.Keyword, x => x.TargetScopeMultipleDeclarations());
                }
            }

            // find duplicate using declarations
            var usingSyntaxes = symbol.Syntax.Children.OfType<UsingDeclarationSyntax>().ToImmutableArray();

            if (usingSyntaxes.Length > 1)
            {
                foreach (var declaration in usingSyntaxes)
                {
                    this.diagnosticWriter.Write(declaration.Keyword, x => x.MoreThanOneUsingDeclarationSpecified());
                }
            }

            // find duplicate extends declarations
            var extendsSyntaxes = symbol.Syntax.Children.OfType<ExtendsDeclarationSyntax>().ToImmutableArray();

            if (extendsSyntaxes.Length > 1)
            {
                foreach (var declaration in extendsSyntaxes)
                {
                    this.diagnosticWriter.Write(declaration.Keyword, x => x.MoreThanOneExtendsDeclarationSpecified());
                }
            }

            // find instances where the same symbol is imported multiple times under different names
            foreach (var grouping in symbol.ImportedSymbols.ToLookup(t => (t.SourceModel, t.OriginalSymbolName)))
            {
                if (grouping.Key.Item1 is null || grouping.Key.OriginalSymbolName is null)
                {
                    // these import symbols had errors that prevented them from being loaded correctly, so it's unclear if they refer to the same original symbol.
                    // We're already reporting a load error for these, so add a potentially false diagnostic about duplicate imports of the same symbol
                    continue;
                }

                if (grouping.Count() > 1)
                {
                    var importedAs = grouping.Select(s => s.Name).ToArray();
                    foreach (var import in grouping)
                    {
                        this.diagnosticWriter.Write(import.DeclaringSyntax, x => x.SymbolImportedMultipleTimes(importedAs));
                    }
                }
            }

            if (symbol.FileKind == BicepSourceFileKind.ParamsFile)
            {
                var hasExtends = extendsSyntaxes.Length == 1;

                if (!hasExtends)
                {
                    foreach (var access in FindVariableAccesses(symbol.Syntax, LanguageConstants.BaseIdentifier))
                    {
                        this.diagnosticWriter.Write(access.Name, x => x.BaseIdentifierNotAvailableWithoutExtends());
                    }
                }

                foreach (var decl in symbol.Declarations.Where(d => string.Equals(d.Name, LanguageConstants.BaseIdentifier, LanguageConstants.IdentifierComparison) && d is not BaseParametersSymbol))
                {
                    this.diagnosticWriter.Write(decl.DeclaringSyntax, x => x.BaseIdentifierRedeclared());
                }
            }
        }

        private static IEnumerable<VariableAccessSyntax> FindVariableAccesses(SyntaxBase root, string identifier)
        {
            var results = new List<VariableAccessSyntax>();
            var visitor = new VariableAccessCollector(identifier, results);
            root.Accept(visitor);
            return results;
        }

        private sealed class VariableAccessCollector : CstVisitor
        {
            private readonly string identifier;
            private readonly IList<VariableAccessSyntax> results;

            public VariableAccessCollector(string identifier, IList<VariableAccessSyntax> results)
            {
                this.identifier = identifier;
                this.results = results;
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                if (syntax.Name.IdentifierName == identifier)
                {
                    results.Add(syntax);
                }
                base.VisitVariableAccessSyntax(syntax);
            }
        }

        public override void VisitVariableSymbol(VariableSymbol symbol)
        {
            base.VisitVariableSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitDeclaredFunctionSymbol(DeclaredFunctionSymbol symbol)
        {
            base.VisitDeclaredFunctionSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitResourceSymbol(ResourceSymbol symbol)
        {
            base.VisitResourceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitModuleSymbol(ModuleSymbol symbol)
        {
            base.VisitModuleSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitOutputSymbol(OutputSymbol symbol)
        {
            base.VisitOutputSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitErrorSymbol(ErrorSymbol symbol)
        {
            base.VisitErrorSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitBuiltInNamespaceSymbol(BuiltInNamespaceSymbol symbol)
        {
            base.VisitBuiltInNamespaceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitExtensionNamespaceSymbol(ExtensionNamespaceSymbol symbol)
        {
            base.VisitExtensionNamespaceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitFunctionSymbol(FunctionSymbol symbol)
        {
            base.VisitFunctionSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitLocalScope(LocalScope symbol)
        {
            base.VisitLocalScope(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitWildcardImportSymbol(WildcardImportSymbol symbol)
        {
            base.VisitWildcardImportSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitImportedTypeSymbol(ImportedTypeSymbol symbol)
        {
            base.VisitImportedTypeSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitImportedVariableSymbol(ImportedVariableSymbol symbol)
        {
            base.VisitImportedVariableSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitImportedFunctionSymbol(ImportedFunctionSymbol symbol)
        {
            base.VisitImportedFunctionSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitErroredImportSymbol(ErroredImportSymbol symbol)
        {
            base.VisitErroredImportSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        protected void CollectDiagnostics(Symbol symbol)
        {
            diagnosticWriter.WriteMultiple(symbol.GetDiagnostics());
        }
    }
}
