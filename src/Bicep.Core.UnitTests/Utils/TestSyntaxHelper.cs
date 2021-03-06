// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Semantics;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestSyntaxHelper
    {
        public static bool NodeShouldBeBound(ISymbolReference symbolReference, SemanticModel semanticModel)
        {
            if (!(symbolReference is InstanceFunctionCallSyntax instanceFunctionCallSyntax))
            {
                return true;
            }

            if (instanceFunctionCallSyntax.BaseExpression is VariableAccessSyntax baseVariableSyntax && 
                semanticModel.Root.ImportedNamespaces.ContainsKey(baseVariableSyntax.Name.IdentifierName))
            {
                // we only expect to have bound InstanceFunctionCallsSyntax if they accessed on a namespace - e.g. sys.concat(..)
                return true;
            }

            return false;
        }
    }
}
