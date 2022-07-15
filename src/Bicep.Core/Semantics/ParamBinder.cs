// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class ParamBinder
    {
        private readonly BicepParamFile bicepParamFile;
        private readonly ImmutableDictionary<SyntaxBase, Symbol> bindings;
        private readonly ImmutableDictionary<BindableSymbol, ImmutableArray<BindableSymbol>> cyclesBySymbol;

        public ParamBinder(BicepParamFile bicepParamFile, ParamsSymbolContext paramsSymbolContext)
        {
            this.bicepParamFile = bicepParamFile;
            var symbols = ParamAssignmentSymbolCollectVisitor.GetSymbols(bicepParamFile, paramsSymbolContext);
            var uniqueSymbols = GetUniqueSymbols(symbols);
            this.bindings = ParamNameBindingVisitor.GetBindings(bicepParamFile.ProgramSyntax, uniqueSymbols);
            this.cyclesBySymbol = GetCyclesBySymbol(bicepParamFile, this.bindings);

            this.ParamFileSymbol = new ParamFileSymbol(
                symbols,
                bicepParamFile.FileUri.LocalPath,
                bicepParamFile.ProgramSyntax,
                symbols,
                bicepParamFile.FileUri);
        }

        public ParamFileSymbol ParamFileSymbol { get; }
        
        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.bindings.TryGetValue(syntax);

        public ImmutableArray<BindableSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol)
            => this.cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle) ? cycle : null;

        private static ImmutableDictionary<string, ParameterAssignmentSymbol> GetUniqueSymbols(IEnumerable<ParameterAssignmentSymbol> symbols)
        {
            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            return symbols
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        private static ImmutableDictionary<BindableSymbol, ImmutableArray<BindableSymbol>> GetCyclesBySymbol(BicepParamFile bicepParamFile, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            return CyclicCheckVisitor.FindCycles(bicepParamFile.ProgramSyntax, bindings);
        }
    }
}
