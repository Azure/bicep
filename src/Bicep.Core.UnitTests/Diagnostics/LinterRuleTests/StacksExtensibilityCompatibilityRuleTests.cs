// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.Testing.IO;
using Bicep.Testing.Mocks;
using Bicep.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class StacksExtensibilityCompatibilityRuleTests : LinterRuleTestsBase
    {
        private static readonly IOUri MainUri = TestFileUri.FromInMemoryPath("main.bicep");
        private static readonly IOUri ParamsUri = TestFileUri.FromInMemoryPath("main.bicepparam");

        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        [DataRow(
            "ValidSecurePropertyAssignment",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: kv.getSecret('a'), stringRequiredProp: 'value' } }"
        )]
        [DataRow(
            "SecurePropertyInheritance",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: mockExt.config.secureStringRequiredProp, stringRequiredProp: 'value' } }"
        )]
        [DataRow(
            "SecurePropertyInheritanceArrayVariant",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: mockExt['config']['secureStringRequiredProp'], stringRequiredProp: 'value' } }"
        )]
        public async Task Does_not_flag_stack_compatible_assignments(string scenario, string extConfigAssignments, string moduleBody)
        {
            var compilation = await Compile(extConfigAssignments, moduleBody);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError() || d.Code == StacksExtensibilityCompatibilityRule.Code);
        }

        // NOTE: BCP180/getSecret validation covers assigning key vault references to non-secure properties.
        [TestMethod]
        [DataRow(
            "InlinedSecretsAreFlagged",
            "extensionConfig mockExt with { secureStringRequiredProp: 'SECRET', stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: 'SECRET', stringRequiredProp: 'value' } }",
            true,
            true,
            false
        )]
        [DataRow(
            "NonSecurePropertyReferencesAreCoveredByGetSecretValidation_ParamsFile",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: az.getSecret('a', 'b', 'c', 'd') }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: kv.getSecret('a'), stringRequiredProp: 'value' } }",
            true,
            false,
            true)]
        [DataRow(
            "NonSecurePropertyReferencesAreCoveredByGetSecretValidation_MainFile",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: kv.getSecret('a'), stringRequiredProp: kv.getSecret('a') } }",
            false,
            true,
            true)]
        [DataRow(
            "NonSecurePropertyInheritsSecurePropertyIsFlagged",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: kv.getSecret('a'), stringRequiredProp: mockExt.config.secureStringRequiredProp } }",
            false,
            true,
            false
        )]
        [DataRow(
            "SecurePropertyInheritsNonSecurePropertyIsFlagged",
            "extensionConfig mockExt with { secureStringRequiredProp: az.getSecret('a', 'b', 'c', 'd'), stringRequiredProp: 'value' }",
            "extensionConfigs: { mockExt: { secureStringRequiredProp: mockExt.config.stringRequiredProp, stringRequiredProp: 'value' } }",
            false,
            true,
            false
        )]
        public async Task Provides_diagnostic_for_stacks_incompatible_assignments(string scenario, string extConfigAssignments, string moduleBody, bool paramsFileDiagExpected, bool mainFileDiagExpected, bool expectError)
        {
            var compilation = await Compile(extConfigAssignments, moduleBody);

            if (!expectError)
            {
                compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());
            }

            if (scenario is "NonSecurePropertyReferencesAreCoveredByGetSecretValidation_MainFile")
            {
                compilation.GetSourceFileDiagnostics(MainUri).Should().ContainSingleDiagnostic("BCP180", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property.", because: "param files should have this validation");

                return;
            }

            var expectedMessage = scenario switch
            {
                "InlinedSecretsAreFlagged" or "SecurePropertyInheritsNonSecurePropertyIsFlagged" => CoreResources.StacksExtensibilityCompatibilityRule_SecurePropertyValueIsNotReference,
                "NonSecurePropertyInheritsSecurePropertyIsFlagged" or "NonSecurePropertyReferencesAreCoveredByGetSecretValidation_ParamsFile" => CoreResources.StacksExtensibilityCompatibilityRule_NonSecurePropertyValueIsReference,
                _ => throw new NotImplementedException()
            };

            if (paramsFileDiagExpected)
            {
                compilation.GetSourceFileDiagnostics(ParamsUri).Should().ContainSingleDiagnostic(StacksExtensibilityCompatibilityRule.Code, DiagnosticLevel.Info, expectedMessage, because: "param files should have this validation");
            }
            else
            {
                compilation.GetSourceFileDiagnostics(ParamsUri).Should().NotContainDiagnostic(StacksExtensibilityCompatibilityRule.Code);
            }

            if (mainFileDiagExpected)
            {
                compilation.GetSourceFileDiagnostics(MainUri).Should().ContainSingleDiagnostic(StacksExtensibilityCompatibilityRule.Code, DiagnosticLevel.Info, expectedMessage, because: "bicep files should have this validation");
            }
            else
            {
                compilation.GetSourceFileDiagnostics(MainUri).Should().NotContainDiagnostic(StacksExtensibilityCompatibilityRule.Code);
            }
        }

        #region Helpers

        private async Task<Compilation> Compile(string extConfigAssignments, string moduleBody)
        {
            const string extDeclarations = """
                extension 'br:mcr.microsoft.com/bicep/extensions/mockext/v1:1.2.3' as mockExt
                """;

            var compiler = TestCompiler
                .ForInMemoryCompilation()
                .WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(
                    new(TestContext, ModuleExtensionConfigsEnabled: true));
            var artifactManager = new TestExternalArtifactManager(compiler);
            await artifactManager.PublishExtension(CreateMockExt());

            var result = await compiler.Compile(
                "main.bicepparam",
                ("main.bicepparam", $$"""
                    using 'main.bicep'

                    {{extConfigAssignments}}
                    """),
                ("main.bicep", $$"""
                    {{extDeclarations}}

                    resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
                      name: 'kv'
                    }

                    module modulea 'modulea.bicep' = {
                      {{moduleBody}}
                    }
                    """),
                ("modulea.bicep", $$"""
                    {{extDeclarations}}
                    """));

            return result.Compilation;
        }

        private static MockExtensionData CreateMockExt(string extName = "mockext") =>
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
