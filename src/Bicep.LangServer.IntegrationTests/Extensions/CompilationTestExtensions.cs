// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.LangServer.IntegrationTests.Extensions
{
    public static class CompilationTestExtensions
    {
        public static IDictionary<SyntaxBase, Symbol> ReconstructSymbolTable(this Compilation compilation)
        {
            var model = compilation.GetEntrypointSemanticModel();

            return SyntaxAggregator.Aggregate(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, new Dictionary<SyntaxBase, Symbol>(), (accumulated, node) =>
            {
                if (model.GetSymbolInfo(node) is Symbol symbol)
                {   
                    accumulated[node] = symbol;
                }

                return accumulated;
            }, accumulated => accumulated);
        }
    }
}
