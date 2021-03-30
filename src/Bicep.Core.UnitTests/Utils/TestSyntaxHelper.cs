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
            if (symbolReference is InstanceFunctionCallSyntax ||
                symbolReference is PropertyAccessSyntax ||
                symbolReference is ObjectPropertySyntax)
            {
                return false;
            }

            return true;
        }
    }
}
