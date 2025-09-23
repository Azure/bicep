// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CentralizedExtensionVersionManagementTests : TestBase
    {
        private ServiceBuilder Services => new();

        [TestMethod]
        [DynamicData(nameof(ExtensionsConfig_SupportForConfigManagedExtensionDeclarationSyntax_When_ExtensionIsBuiltIn_TestCases))]
        public void ExtensionsConfig_SupportForConfigManagedExtensionDeclarationSyntax_When_ExtensionIsBuiltIn(string identifier, bool shouldSucceed, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            var result = CompilationHelper.Compile(Services, @$"
                extension {identifier}
            ");

            if (shouldSucceed)
            {
                result.Should().NotHaveAnyDiagnostics();
                return;
            }
            result.Should().HaveDiagnostics(expectedDiagnostics);
        }

        public static IEnumerable<object[]> ExtensionsConfig_SupportForConfigManagedExtensionDeclarationSyntax_When_ExtensionIsBuiltIn_TestCases
        {
            get
            {
                (string, DiagnosticLevel, string)[] emptyDiagnostics = [];
                yield return new object[] {
                    "sys",
                    true,
                    emptyDiagnostics };
                yield return new object[] {
                    "microsoftGraph",
                    false,
                    new (string, DiagnosticLevel, string)[] {
                     ("BCP407", DiagnosticLevel.Error, "Built-in extension \"microsoftGraph\" is retired. Use dynamic types instead. See https://aka.ms/graphbicep/dynamictypes" ) } };
                yield return new object[] {
                    "az",
                    true,
                    emptyDiagnostics };
                yield return new object[] {
                    "kubernetes",
                    false,
                    new (string, DiagnosticLevel, string)[] {
                     ("BCP206", DiagnosticLevel.Error, "Extension \"kubernetes\" requires configuration, but none was provided.") } };
            }
        }
    }
}
