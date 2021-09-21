// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public static class SemanticModelHelper
    {
        public static FunctionCallSyntaxBase? TryGetFunctionInNamespace(SemanticModel semanticModel, string @namespace, SyntaxBase syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol function &&
                function.DeclaringObject is NamespaceType namespaceType &&
                LanguageConstants.IdentifierComparer.Equals(namespaceType.ProviderName, @namespace))
            {
                return syntax as FunctionCallSyntaxBase;
            }
            
            return null;
        }
    }
}
