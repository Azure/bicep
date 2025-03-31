// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class AzNamespaceTests
    {
        [TestMethod]
        public void AzNamespace_ZoneFunctions_ShouldExistAndRequireInlining()
        {
            var azNamespaceType = TestTypeHelper.GetBuiltInNamespaceType("az");

            VerifyFunctionExistsWithRequiresInliningFlag(azNamespaceType, "toLogicalZone");
            VerifyFunctionExistsWithRequiresInliningFlag(azNamespaceType, "toLogicalZones");
            VerifyFunctionExistsWithRequiresInliningFlag(azNamespaceType, "toPhysicalZone");
            VerifyFunctionExistsWithRequiresInliningFlag(azNamespaceType, "toPhysicalZones");
        }

        private static void VerifyFunctionExistsWithRequiresInliningFlag(NamespaceType namespaceType, string functionName)
        {
            var functions = namespaceType.MethodResolver.GetKnownFunctions()
                .Where(f => f.Key == functionName)
                .ToList();
            
            functions.Should().HaveCount(1, $"Function '{functionName}' should exist exactly once in the 'az' namespace");
            
            var function = functions.First().Value;
            function.Overloads.Should().HaveCount(1, $"Function '{functionName}' should have exactly one overload");
            
            var overload = function.Overloads[0];
            overload.Flags.HasFlag(FunctionFlags.RequiresInlining).Should().BeTrue(
                $"Function '{functionName}' should have the RequiresInlining flag set");
        }
    }
}