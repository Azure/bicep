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
                var parent = import.EnclosingDeclaration;

                var codeFixSpan = import.NameSource.Span;

                codeFixSpan = GetSpanForImportedSymbolCodeFix(parent, import);

                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, import.Name, import.NameSource.Span, codeFixSpan);
            }

            //Handle wildcard alias

            // VariableAccessSyntax indicates a reference to the import
            var unreferencedWildcardImports = model.Root.WildcardImports
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var import in unreferencedWildcardImports)
            {
                //Remove the whole line
                var parent = import.EnclosingDeclaration;

                var codeFixSpan = import.NameSource.Span;

                codeFixSpan = parent.Span;

                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, import.Name, import.NameSource.Span, codeFixSpan);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove unused import {name}";
        }

        private TextSpan GetSpanForImportedSymbolCodeFix(CompileTimeImportDeclarationSyntax importDeclarationSyntax, ImportedSymbol importedSymbol)
        {
            var importSpan = importedSymbol.NameSource.Span;
            var importSyntaxExpression = (ImportedSymbolsListSyntax)importDeclarationSyntax.ImportExpression;
            var importSyntaxExpressionChildCount = importSyntaxExpression.Children.Length;
            var indexOfImportedSymbol = importSyntaxExpression.Children.IndexOf(importedSymbol.DeclaringSyntax);

            if (importSyntaxExpressionChildCount < 2)
            {
                return importSpan;
            }
            if (indexOfImportedSymbol == 0)
            {
                // remove right comma
                importSpan = TextSpan.Between(importSyntaxExpression.Children[0].Span, importSyntaxExpression.Children[1].Span);
            }
            else if (indexOfImportedSymbol > 0 && indexOfImportedSymbol < importSyntaxExpressionChildCount - 1)
            {
                // remove left comma when surrounded
                importSpan = TextSpan.Between(importSyntaxExpression.Children[indexOfImportedSymbol - 1].Span, importSyntaxExpression.Children[indexOfImportedSymbol].Span);
            }
            else
            {
                // remove left comma when last
                importSpan = TextSpan.Between(importSyntaxExpression.Children[importSyntaxExpressionChildCount - 2].Span, importSyntaxExpression.Children[importSyntaxExpressionChildCount - 1].Span);
            }

            return importSpan;
        }
    }
}
