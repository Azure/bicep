// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class TestSyntaxHelper
    {
        public static bool NodeShouldBeBound(ISymbolReference symbolReference)
            => symbolReference is not InstanceFunctionCallSyntax 
                and not PropertyAccessSyntax
                and not ObjectPropertySyntax;
    }
}