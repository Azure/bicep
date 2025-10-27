// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class AzNamespaceTests
    {
        [DataTestMethod]
        [DataRow("toLogicalZone")]
        [DataRow("toLogicalZones")]
        [DataRow("toPhysicalZone")]
        [DataRow("toPhysicalZones")]
        public void ZoneFunctions_ShouldExistAndRequireInlining(string functionName)
        {
            VerifyFunctionProperties(functionName, function =>
            {
                function.Name.Should().Be(functionName);
                function.Overloads.Should().HaveCount(1, $"Function '{functionName}' should have exactly one overload");
                function.Overloads[0].Flags.HasFlag(FunctionFlags.RequiresInlining).Should().BeTrue(
                    $"Function '{functionName}' should have the RequiresInlining flag set");
            });
        }

        [TestMethod]
        public void DeployerFunctionReturnType_ShouldHaveExpectedProperties()
        {
            var functionName = "deployer";
            VerifyFunctionProperties(functionName, function =>
            {
                function.Name.Should().Be(functionName);
                function.Overloads.Should().HaveCount(1, $"Function '{functionName}' should have exactly one overload");

                var overload = function.Overloads[0];
                overload.TypeSignatureSymbol.Should().BeOfType<ObjectType>();

                var overloadType = (ObjectType)overload.TypeSignatureSymbol;
                overloadType.Properties.Should().HaveCount(3, $"The return type for function '{functionName}' should have exactly three properties");

                overloadType.Properties.Should().ContainKey("objectId").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'objectId' property should be of type string");
                overloadType.Properties.Should().ContainKey("tenantId").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'tenantId' property should be of type string");
                overloadType.Properties.Should().ContainKey("userPrincipalName").WhoseValue.TypeReference.Should().Be(LanguageConstants.String,
                    "The 'userPrincipalName' property should be of type string");
            });
        }

        private static void VerifyFunctionProperties(string functionName, Action<FunctionSymbol> assertion)
        {
            var azNamespaceType = TestTypeHelper.GetBuiltInNamespaceType("az");
            var functions = azNamespaceType.MethodResolver.GetKnownFunctions()
                .Where(f => f.Key == functionName)
                .ToList();

            functions.Should().HaveCount(1, $"Function '{functionName}' should exist exactly once in the 'az' namespace");
            assertion(functions[0].Value);
        }
    }
}
