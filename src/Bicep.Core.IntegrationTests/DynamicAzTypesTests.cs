// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegistryUtils = Bicep.Core.UnitTests.Utils.ContainerRegistryClientFactoryExtensions;

namespace Bicep.Core.IntegrationTests
{

    [TestClass]
    public class DynamicAzTypesTests : TestBase
    {
        private async Task<ServiceBuilder> GetServices()
        {
            var indexJson = FileHelper.SaveResultFile(TestContext, "types/index.json", """{"resources": {}, "resourceFunctions": {}}""");

            var cacheRoot = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(cacheRoot);

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true, CacheRootDirectory: cacheRoot))
                .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForAzProvider());

            await RegistryHelper.PublishAzProvider(services.Build(), indexJson);

            return services;
        }

        private async Task<ServiceBuilder> ServicesWithTestProviderArtifact(ArtifactRegistryAddress artifactRegistryAddress, BinaryData artifactPayload)
        {
            (var clientFactory, var blobClients) = RegistryUtils.CreateMockRegistryClients(artifactRegistryAddress.ClientDescriptor());

            (_, var client) = blobClients.First();
            var configResult = await client.UploadBlobAsync(BinaryData.FromString("{}"));
            var blobResult = await client.UploadBlobAsync(artifactPayload);
            var manifest = BicepTestConstants.GetBicepProviderManifest(blobResult.Value, configResult.Value);
            await client.SetManifestAsync(manifest, artifactRegistryAddress.ProviderVersion);

            var cacheRoot = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(cacheRoot);

            return new ServiceBuilder()
                .WithFeatureOverrides(new(
                    ExtensibilityEnabled: true,
                    DynamicTypeLoadingEnabled: true,
                    CacheRootDirectory: cacheRoot))
           .WithContainerRegistryClientFactory(clientFactory);
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_without_configuration()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            provider 'br/public:az:{BicepTestConstants.BuiltinAzProviderVersion}'
            "));

            result.Should().GenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task Az_namespace_errors_with_configuration()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az:{BicepTestConstants.BuiltinAzProviderVersion}' with {{}}
            ");

            result.Should().NotGenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Provider namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_with_alias()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az:{BicepTestConstants.BuiltinAzProviderVersion}' as testAlias
            ");

            result.Should().GenerateATemplate();
            result.Compilation.GetEntrypointSemanticModel().Root.ProviderDeclarations.Should().Contain(x => x.Name.Equals("testAlias"));
        }

        [TestMethod]
        public async Task Az_namespace_can_be_used_with_bicepconfig_provider_alias()
        {
            var services = await GetServices();

            services = services.WithConfigurationPatch(c => c.WithProviderAlias("""
            {
                "br": {
                    "customAlias": {
                        "registry": "mcr.microsoft.com",
                        "providerPath": "bicep/providers"
                    }
                }
            }
            """));

            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/customAlias:az:{BicepTestConstants.BuiltinAzProviderVersion}'
            ");

            result.Should().GenerateATemplate();
            result.Compilation.GetEntrypointSemanticModel().Root.ProviderDeclarations.Should().Contain(x => x.Name.Equals("az"));
        }

        [TestMethod]
        public async Task Inlined_Az_namespace_alias_is_not_specified_in_config_yields_diagnostic()
        {
            var services = new ServiceBuilder()
               .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));

            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'br/notFound:az:0.2.661'
            ");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics([
                ("BCP379", DiagnosticLevel.Error, "The OCI artifact provider alias name \"notFound\" does not exist in the built-in Bicep configuration."),
            ]);
        }

        [TestMethod]
        public async Task Bicep_module_artifact_specified_in_provider_declaration_syntax_yields_diagnostic()
        {
            // ARRANGE
            var fsMock = new MockFileSystem();
            var testArtifact = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "bicep/providers/az", "0.2.661");
            var clientFactory = RegistryHelper.CreateMockRegistryClients((testArtifact.RegistryAddress, testArtifact.RepositoryPath)).factoryMock;
            var services = new ServiceBuilder()
                .WithFileSystem(fsMock)
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true))
                .WithContainerRegistryClientFactory(clientFactory);

            await RegistryHelper.PublishModuleToRegistryAsync(
                clientFactory,
                fsMock,
                moduleName: "az",
                target: testArtifact.ToSpecificationString(':'),
                moduleSource: "",
                publishSource: false,
                documentationUri: "mydocs.org/abc");

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{testArtifact.ToSpecificationString(':')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(
                new[] {
                ("BCP192", DiagnosticLevel.Error, """Unable to restore the artifact with reference "br:mcr.microsoft.com/bicep/providers/az:0.2.661": The OCI artifact is not a valid Bicep artifact. Expected a provider, but retrieved a module."""),
            });
        }

        [TestMethod]
        [DynamicData(nameof(ArtifactRegistryCorruptedPackageNegativeTestScenarios), DynamicDataSourceType.Method)]
        public async Task Bicep_compiler_handles_corrupted_provider_package_gracefully(
            BinaryData payload,
            string innerErrorMessage)
        {
            // ARRANGE
            var testArtifactAddress = new ArtifactRegistryAddress("biceptestdf.azurecr.io", "bicep/providers/az", "0.0.0-corruptpng");

            var services = await ServicesWithTestProviderArtifact(testArtifactAddress, payload);

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{testArtifactAddress.ToSpecificationString(':')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics([
                ("BCP396", DiagnosticLevel.Error, """The referenced provider types artifact has been published with malformed content.""")
            ]);
        }

        public record ArtifactRegistryAddress(string RegistryAddress, string RepositoryPath, string ProviderVersion)
        {
            public string ToSpecificationString(char delim) => $"br:{RegistryAddress}/{RepositoryPath}{delim}{ProviderVersion}";

            public (string, string) ClientDescriptor() => (RegistryAddress, RepositoryPath);
        }

        [TestMethod]
        [DynamicData(nameof(ArtifactRegistryAddressNegativeTestScenarios), DynamicDataSourceType.Method)]
        public async Task Repository_not_found_in_registry(
            ArtifactRegistryAddress artifactRegistryAddress,
            Exception exceptionToThrow,
            IEnumerable<(string, DiagnosticLevel, string)> expectedDiagnostics)
        {
            // ARRANGE
            // mock the blob client to throw the expected exception
            var mockBlobClient = StrictMock.Of<MockRegistryBlobClient>();
            mockBlobClient.Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exceptionToThrow);

            // mock the registry client to return the mock blob client
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();
            containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(
                artifactRegistryAddress.RegistryAddress,
                artifactRegistryAddress.RepositoryPath,
                mockBlobClient.Object);

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true))
                .WithContainerRegistryClientFactory(containerRegistryFactoryBuilder.Build().clientFactory);

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{artifactRegistryAddress.ToSpecificationString(':')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(expectedDiagnostics);
        }

        public static IEnumerable<object[]> ArtifactRegistryAddressNegativeTestScenarios()
        {
            // constants
            const string placeholderProviderVersion = "0.0.0-placeholder";

            // unresolvable host registry. For example if DNS is down or unresponsive
            const string unreachableRegistryAddress = "unknown.registry.azurecr.io";
            const string NoSuchHostMessage = $" (No such host is known. ({unreachableRegistryAddress}:443))";
            var AggregateExceptionMessage = $"Retry failed after 4 tries. Retry settings can be adjusted in ClientOptions.Retry or by configuring a custom retry policy in ClientOptions.RetryPolicy.{string.Concat(Enumerable.Repeat(NoSuchHostMessage, 4))}";
            var unreacheable = new ArtifactRegistryAddress(unreachableRegistryAddress, "bicep/providers/az", placeholderProviderVersion);
            yield return new object[] {
                unreacheable,
                new AggregateException(AggregateExceptionMessage),
                new (string, DiagnosticLevel, string)[]{
                    ("BCP192", DiagnosticLevel.Error, @$"Unable to restore the artifact with reference ""{unreacheable.ToSpecificationString(':')}"": {AggregateExceptionMessage}")
                },
            };

            // manifest not found is thrown when the repository address is not registered and/or the version doesn't exist in the registry
            const string NotFoundMessage = "The artifact does not exist in the registry.";
            var withoutRepo = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "unknown/path/az", placeholderProviderVersion);
            yield return new object[] {
                withoutRepo,
                new RequestFailedException(404, NotFoundMessage),
                new (string, DiagnosticLevel, string)[]{
                    ("BCP192", DiagnosticLevel.Error, $@"Unable to restore the artifact with reference ""{withoutRepo.ToSpecificationString(':')}"": {NotFoundMessage}")
                },
            };
        }

        public static IEnumerable<object[]> ArtifactRegistryCorruptedPackageNegativeTestScenarios()
        {
            // Scenario: When OciTypeLoader.FromDisk() throws, the exception is exposed as a diagnostic
            // Some cases covered by this test are:
            // - Artifact layer payload is not a GZip compressed
            // - Artifact layer payload is a GZip compressedbut is not composed of Tar entries
            yield return new object[]
            {
                BinaryData.FromString("This is a NOT GZip compressed data"),
                "The archive entry was compressed using an unsupported compression method.",
            };

            // Scenario: Artifact layer payload is missing an "index.json"
            yield return new object[]
            {
                ThirdPartyTypeHelper.GetTypesTgzBytesFromFiles(
                    ("unknown.json", "{}")),
                "The path: index.json was not found in artifact contents"
            };

            // Scenario: "index.json" is not valid JSON
            yield return new object[]
            {
                ThirdPartyTypeHelper.GetTypesTgzBytesFromFiles(
                    ("index.json", """{"INVALID_JSON": 777""")),
                "'7' is an invalid end of a number. Expected a delimiter. Path: $.INVALID_JSON | LineNumber: 0 | BytePositionInLine: 20."
            };

            // Scenario: "index.json" with malformed or missing required data
            yield return new object[]
            {
                ThirdPartyTypeHelper.GetTypesTgzBytesFromFiles(
                    ("index.json", """{ "UnexpectedMember": false}""")),
                "Value cannot be null. (Parameter 'source')"
            };
        }

        [TestMethod]
        public async Task External_Az_namespace_can_be_loaded_from_configuration()
        {
            var services = await GetServices();
            // Built-In Config contains the following entry:
            // {
            //   "implicitProviders": ["az"]
            // }
            services = services.WithConfigurationPatch(c => c.WithProvidersConfiguration($$"""
            {
                "az": "br:{{LanguageConstants.BicepPublicMcrRegistry}}/bicep/providers/az:{{BicepTestConstants.BuiltinAzProviderVersion}}"
            }
            """));
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            provider az
            "));

            result.Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task BuiltIn_Az_namespace_can_be_loaded_from_configuration()
        {
            var services = await GetServices();
            // Built-In Config contains the following entries:
            // {
            //   "providers": {
            //     "az": "builtin:"
            //   },
            //   "implicitProviders": ["az"]
            // }
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            provider az
            "));

            result.Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task Az_namespace_can_be_loaded_dynamically_using_provider_configuration()
        {
            //ARRANGE
            var artifactRegistryAddress = new ArtifactRegistryAddress(
                "fake.azurecr.io",
                "fake/path/az",
                "1.0.0-fake");
            var services = await ServicesWithTestProviderArtifact(
                artifactRegistryAddress,
                ThirdPartyTypeHelper.GetTypesTgzBytesFromFiles(("index.json", """{"resources": {}, "resourceFunctions": {}}""")));
            services = services.WithConfigurationPatch(c => c.WithProvidersConfiguration($$"""
            {
                "az": "{{artifactRegistryAddress.ToSpecificationString(':')}}"
            }
            """));
            //ACT
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            provider az
            "));
            //ASSERT
            result.Should().GenerateATemplate();
            result.Template.Should().NotBeNull();
            result.Template.Should().HaveValueAtPath("$.imports.az.version", AzNamespaceType.EmbeddedAzProviderVersion);
        }
    }
}
