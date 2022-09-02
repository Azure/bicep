// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public sealed class ParamAssignmentSymbolCollectVisitor : SyntaxVisitor
    {
        private readonly IList<ParameterAssignmentSymbol> symbols;
        private ParamsSymbolContext paramsSymbolContext;

        private ParamAssignmentSymbolCollectVisitor(IList<ParameterAssignmentSymbol> symbols, ParamsSymbolContext paramsSymbolContext)
        {
            this.symbols = symbols;
            this.paramsSymbolContext = paramsSymbolContext;
        }
        
        public static ImmutableArray<ParameterAssignmentSymbol> GetSymbols(BicepParamFile bicepParamFile, ParamsSymbolContext paramsSymbolContext)
        {
            // collect declarations
            var symbols = new List<ParameterAssignmentSymbol>();
            var symbolCollectVisitor = new ParamAssignmentSymbolCollectVisitor(symbols, paramsSymbolContext);
            symbolCollectVisitor.Visit(bicepParamFile.ProgramSyntax);

            return symbols.ToImmutableArray();
        }
        
        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            base.VisitParameterAssignmentSyntax(syntax);

            var symbol = new ParameterAssignmentSymbol(syntax.Name.IdentifierName, syntax, syntax.Name, paramsSymbolContext);
            symbols.Add(symbol);
        }

        
    }
}
