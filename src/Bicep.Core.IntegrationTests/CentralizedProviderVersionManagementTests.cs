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
    public class CentralizedProviderVersionManagementTests : TestBase
    {
        private ServiceBuilder Services => new ServiceBuilder()
            .WithFeatureOverrides(new(
                ExtensibilityEnabled: true,
                DynamicTypeLoadingEnabled: true));

        [TestMethod]
        [DynamicData(nameof(ProvidersConfig_SupportForConfigManagedProviderDeclarationSyntax_When_ProviderIsBuiltIn_TestCases))]
        public void ProvidersConfig_SupportForConfigManagedProviderDeclarationSyntax_When_ProviderIsBuiltIn(string providerIdentifier, bool shouldSucceed, (string code, DiagnosticLevel level, string message)[] expectedDiagnostics)
        {
            var result = CompilationHelper.Compile(Services, @$"
                provider {providerIdentifier}
            ");

            if (shouldSucceed)
            {
                result.Should().NotHaveAnyDiagnostics();
                return;
            }
            result.Should().HaveDiagnostics(expectedDiagnostics);
        }

        public static IEnumerable<object[]> ProvidersConfig_SupportForConfigManagedProviderDeclarationSyntax_When_ProviderIsBuiltIn_TestCases
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
                    true,
                    emptyDiagnostics };
                yield return new object[] {
                    "az",
                    true,
                    emptyDiagnostics };
                yield return new object[] {
                    "kubernetes",
                    false,
                    new (string, DiagnosticLevel, string)[] {
                     ("BCP206", DiagnosticLevel.Error, "Provider namespace \"kubernetes\" requires configuration, but none was provided.") } };
            }
        }
    }
}
