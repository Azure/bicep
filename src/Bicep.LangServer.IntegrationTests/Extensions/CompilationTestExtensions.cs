// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
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

            var syntaxNodes = SyntaxAggregator.Aggregate(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax, new List<SyntaxBase>(), (accumulated, node) =>
            {
                accumulated.Add(node);
                return accumulated;
            }, accumulated => accumulated);

            return syntaxNodes
                .Where(syntax => model.Binder.GetSymbolInfo(syntax) is not null)
                .ToDictionary(syntax => syntax, syntax => model.Binder.GetSymbolInfo(syntax)!);
        }
    }
}
