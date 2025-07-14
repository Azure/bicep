// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class StacksExtensibilityCompatibilityRuleTests : LinterRuleTestsBase
    {
        public TestContext TestContext { get; set; } = null!;

        [DataTestMethod]
        [DataRow(
            "ValidSecurePropertyAssignment",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: kv.getSecret('a'), stringRequiredProp: 'value' } }"
        )]
        public async Task Does_not_flag_stack_compatible_assignments(string scenario, string extConfigAssignments, string moduleBody)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var extDeclarations =
                """
                extension 'br:mcr.microsoft.com/bicep/extensions/mockext/v1:1.2.3' as mockExt
                """;

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    $$"""
                      using 'main.bicep'

                      {{extConfigAssignments}}
                      """,
                [mainUri] =
                    $$"""
                      {{extDeclarations}}

                      resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
                        name: 'kv'
                      }

                      module modulea 'modulea.bicep' = {
                        {{moduleBody}}
                      }
                      """,
                [moduleAUri] =
                    $$"""
                      {{extDeclarations}}
                      """
            };

            var services = CreateServiceBuilder();

            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExt());

            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError() || d.Code == StacksExtensibilityCompatibilityRule.Code);
        }

        [DataTestMethod]
        [DataRow(
            "InlinedSecretsAreFlagged",
            "extensionConfig mockExt with { secureStringRequiredProp: 'SECRET', stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: 'SECRET', stringRequiredProp: 'value' } }"
        )]
        public async Task Provides_diagnostic_for_stacks_incompatible_assignments(string scenario, string extConfigAssignments, string moduleBody)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var extDeclarations =
                """
                extension 'br:mcr.microsoft.com/bicep/extensions/mockext/v1:1.2.3' as mockExt
                """;

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    $$"""
                      using 'main.bicep'

                      {{extConfigAssignments}}
                      """,
                [mainUri] =
                    $$"""
                      {{extDeclarations}}

                      resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
                        name: 'kv'
                      }

                      module modulea 'modulea.bicep' = {
                        {{moduleBody}}
                      }
                      """,
                [moduleAUri] =
                    $$"""
                      {{extDeclarations}}
                      """
            };

            var services = CreateServiceBuilder();

            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExt());

            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());

            var diagByFileUri = compilation.GetAllDiagnosticsByBicepFileUri();

            var expectedMessage = scenario switch
            {
                "InlinedSecretsAreFlagged" => CoreResources.StacksExtensibilityCompatibilityRule_SecurePropertyValueIsNotReference,
                "NonSecurePropertyReferencesAreFlagged" => CoreResources.StacksExtensibilityCompatibilityRule_SecurePropertyValueIsNotReference,
                _ => throw new NotImplementedException()
            };

            diagByFileUri[paramsUri].Should().ContainSingleDiagnostic(StacksExtensibilityCompatibilityRule.Code, DiagnosticLevel.Info, expectedMessage, because: "param files should have this validation");
            diagByFileUri[mainUri].Should().ContainSingleDiagnostic(StacksExtensibilityCompatibilityRule.Code, DiagnosticLevel.Info, expectedMessage, because: "bicep files should have this validation");
        }

        #region Helpers

        private static ServiceBuilder CreateServiceBuilder() => new ServiceBuilder()
            .WithFeaturesOverridden(f => f with { ModuleExtensionConfigsEnabled = true });

        private static RegistrySourcedExtensionMockData CreateMockExt(string extName = "mockext") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
                {
                    CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                        "config",
                        new Dictionary<string, ObjectTypeProperty>
                        {
                            ["secureStringRequiredProp"] = new(ctx.CoreSecureStringTypeRef, ObjectTypePropertyFlags.Required, null),
                            ["stringRequiredProp"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                            ["stringOptionalProp"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                        },
                        null))
                });

        #endregion
    }
}
