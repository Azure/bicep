// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class SemanticModelHelper
    {
        public static FunctionCallSyntaxBase? TryGetFunctionInNamespace(SemanticModel semanticModel, string @namespace, SyntaxBase syntax)
        {
            if (syntax is not FunctionCallSyntaxBase functionCallSyntax || 
                semanticModel.GetSymbolInfo(functionCallSyntax) is not FunctionSymbol functionSymbol)
            {
                return null;
            }

            if (!semanticModel.Binder.FileSymbol.ImportedNamespaces.TryGetValue(@namespace, out var namespaceSymbol))
            {
                return null;
            }

            if (!object.ReferenceEquals(namespaceSymbol.Type, functionSymbol.DeclaringType))
            {
                return null;
            }

            return functionCallSyntax;
        }
    }
}