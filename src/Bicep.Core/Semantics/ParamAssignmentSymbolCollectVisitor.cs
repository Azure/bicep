// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public sealed class ParamAssignmentSymbolCollectVisitor : SyntaxVisitor
    {
        private readonly IList<ParameterAssignmentSymbol> symbols;

        private ParamAssignmentSymbolCollectVisitor(IList<ParameterAssignmentSymbol> symbols)
        {
            this.symbols = symbols;
        }
        
        public static ImmutableArray<ParameterAssignmentSymbol> GetSymbols(BicepParamFile bicepParamFile)
        {
            // collect declarations
            var symbols = new List<ParameterAssignmentSymbol>();
            var symbolCollectVisitor = new ParamAssignmentSymbolCollectVisitor(symbols);
            symbolCollectVisitor.Visit(bicepParamFile.ProgramSyntax);

            return symbols.ToImmutableArray();
        }
        
        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            base.VisitParameterAssignmentSyntax(syntax);

            var symbol = new ParameterAssignmentSymbol(syntax.Name.IdentifierName, syntax, syntax.Name);
            symbols.Add(symbol);
        }

        
    }
}
