// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public static class SemanticModelHelper
    {
        public static FunctionCallSyntaxBase? TryGetFunctionInNamespace(SemanticModel semanticModel, string @namespace, SyntaxBase syntax)
        {
            switch (syntax)
            {
                case InstanceFunctionCallSyntax ifc:
                    if (semanticModel.GetSymbolInfo(ifc.BaseExpression) is NamespaceSymbol ifcNamespace && 
                        LanguageConstants.IdentifierComparer.Equals(@namespace, ifcNamespace.Name))
                    {
                        return ifc;
                    }
                    break;
                case FunctionCallSyntax fc:
                    if (semanticModel.Binder.FileSymbol.ImportedNamespaces.TryGetValue(@namespace, out var fcNamespace) &&
                        fcNamespace.Type.MethodResolver.TryGetSymbol(fc.Name) is not null)
                    {
                        return fc;
                    }
                    break;
            }
            
            return null;
        }
    }
}