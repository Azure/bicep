// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class SymbolReplacer : SyntaxRewriteVisitor
    {
        private readonly SemanticModel semanticModel;
        private IReadOnlyDictionary<Symbol, SyntaxBase> replacements;

        private SymbolReplacer(SemanticModel semanticModel, IReadOnlyDictionary<Symbol, SyntaxBase> replacements)
        {
            this.semanticModel = semanticModel;
            this.replacements = replacements;
        }

        public static SyntaxBase Replace(SemanticModel semanticModel, IReadOnlyDictionary<Symbol, SyntaxBase> replacements, SyntaxBase syntax) =>
            new SymbolReplacer(semanticModel, replacements).Rewrite(syntax);

        protected override SyntaxBase ReplaceVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (this.semanticModel.GetSymbolInfo(syntax) is not { } symbol || !this.replacements.TryGetValue(symbol, out var replacementSyntax))
            {
                // unbound variable access or not a symbol that we need to replace
                // leave syntax as-is
                return base.ReplaceVariableAccessSyntax(syntax);
            }

            // inject the replacement syntax
            return replacementSyntax;
        }
    }
}
