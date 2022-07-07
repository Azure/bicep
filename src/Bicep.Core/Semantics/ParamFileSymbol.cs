// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class ParamFileSymbol : Symbol
    {
        private readonly ILookup<string, ParameterAssignmentSymbol> symbolsByName;
        private readonly Lazy<IEnumerable<ErrorDiagnostic>> lazyDiagnostics;

        public ParamFileSymbol(ImmutableArray<ParameterAssignmentSymbol> parameterAssignmentSymbols,
            string name,
            ProgramSyntax syntax,
            IEnumerable<BindableSymbol> symbols,
            Uri fileUri)
            : base(name)
        {
            this.lazyDiagnostics = new(() => FindDuplicateNamedSymbols(parameterAssignmentSymbols)
                .Select(symbol => DiagnosticBuilder.ForPosition(symbol.NameSyntax).ParameterMultipleAssignments(symbol.Name)));

            this.Syntax = syntax;
            FileUri = fileUri;
            this.ParameterAssignmentSymbols = parameterAssignmentSymbols;
            this.symbolsByName = this.ParameterAssignmentSymbols.ToLookup(symbol => symbol.Name, LanguageConstants.IdentifierComparer);
        }

        public override IEnumerable<Symbol> Descendants =>
            this.ParameterAssignmentSymbols;

        public override SymbolKind Kind => SymbolKind.File;

        public ProgramSyntax Syntax { get; }

        public ImmutableArray<ParameterAssignmentSymbol> ParameterAssignmentSymbols { get; }

        public Uri FileUri { get; }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParamFileSymbol(this);
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => this.lazyDiagnostics.Value;

        public IEnumerable<ParameterAssignmentSymbol> GetSymbolsByName(string name) => this.symbolsByName[name];

        private static IEnumerable<BindableSymbol> FindDuplicateNamedSymbols(IEnumerable<ParameterAssignmentSymbol> symbols) => symbols
            .Where(symbol => symbol.NameSyntax.IsValid)
            .GroupBy(symbol => symbol.Name, LanguageConstants.IdentifierComparer)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group);
    }
}
