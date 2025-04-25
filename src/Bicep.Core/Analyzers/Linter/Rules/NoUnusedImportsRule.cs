// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Does not handle detection of unused exports when using wildcard imports. Only if the alias is unused.
    public sealed class NoUnusedImportsRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-imports";

        public NoUnusedImportsRule() : base(
            code: Code,
            description: CoreResources.ImportMustBeUsedRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: DiagnosticStyling.ShowCodeAsUnused)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.ImportMustBeUsedRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var invertedBindings = model.Binder.Bindings.ToLookup(kvp => kvp.Value, kvp => kvp.Key);

            // VariableAccessSyntax or ImportedFunctionSymbol indicates a reference to the import
            var unreferencedImports = model.Root.ImportedSymbols
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var import in unreferencedImports)
            {
                // Detect leading and following commas ro remove them along the symbol
               var parent =  model.SourceFile.ProgramSyntax.Declarations
                   .Where(decl => decl is CompileTimeImportDeclarationSyntax)
                   .FirstOrDefault(decl => decl.Span == import.EnclosingDeclaration.Span) as CompileTimeImportDeclarationSyntax;

               var codeFixSpan = import.NameSource.Span;

               if (parent is not null)
               {
                   codeFixSpan = GetSpanForImportedSymbolCodeFix(parent, import);
               }

               yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, import.Name, import.NameSource.Span, codeFixSpan, import.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }

            //Handle wildcard alias

            // VariableAccessSyntax indicates a reference to the import
            var unreferencedWildcardImports = model.Root.WildcardImports
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var import in unreferencedWildcardImports)
            {
                //Remove the whole line
                var parent =  model.SourceFile.ProgramSyntax.Declarations
                    .Where(decl => decl is CompileTimeImportDeclarationSyntax)
                    .FirstOrDefault(decl => decl.Span == import.EnclosingDeclaration.Span) as CompileTimeImportDeclarationSyntax;

                var codeFixSpan = import.NameSource.Span;
                if (parent is not null)
                {
                    codeFixSpan = parent.Span;
                }

                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, import.Name, import.NameSource.Span, codeFixSpan, import.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove import {name}";
        }

        private TextSpan GetSpanForImportedSymbolCodeFix(CompileTimeImportDeclarationSyntax importDeclarationSyntax, ImportedSymbol importedSymbol)
        {
            var importSpan = importedSymbol.NameSource.Span;
            var importExpression = (ImportedSymbolsListSyntax) importDeclarationSyntax.ImportExpression;
            var importExpressionCount = importExpression.Children.Length;
            var indexOfImportedSymbol = importExpression.Children.IndexOf(importedSymbol.DeclaringSyntax);

            if (importExpressionCount < 2)
            {
                return importSpan;
            }
            if (indexOfImportedSymbol == 0)
            {
                // remove right comma
                importSpan = TextSpan.Between(importExpression.Children[0].Span, importExpression.Children[1].Span);
            }
            else if (indexOfImportedSymbol > 0 && indexOfImportedSymbol < importExpressionCount - 1)
            {
                // remove left comma when surrounded
                importSpan = TextSpan.Between(importExpression.Children[indexOfImportedSymbol - 1].Span, importExpression.Children[indexOfImportedSymbol].Span);
            }
            else
            {
                // remove left comma when last
                importSpan = TextSpan.Between(importExpression.Children[importExpressionCount - 2].Span, importExpression.Children[importExpressionCount - 1].Span);
            }

            return importSpan;
        }
    }
}
