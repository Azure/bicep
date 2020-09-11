// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.LangServer.IntegrationTests.Extensions
{
    public static class CompilationTestExtensions
    {
        public static IDictionary<SyntaxBase, Symbol> ReconstructSymbolTable(this Compilation compilation)
        {
            var model = compilation.GetSemanticModel();

            var syntaxNodes = SyntaxAggregator.Aggregate(compilation.ProgramSyntax, new List<SyntaxBase>(), (accumulated, node) =>
            {
                accumulated.Add(node);
                return accumulated;
            }, accumulated => accumulated);

            return syntaxNodes
                .Where(syntax => model.GetSymbolInfo(syntax) != null)
                .ToDictionary(syntax => syntax, syntax => model.GetSymbolInfo(syntax)!);
        }
    }
}
