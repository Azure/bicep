// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class MsGraphTypesViaRegistryTests : TestBase
    {
        private async Task<ServiceBuilder> GetServices()
        {
            return await ExtensionTestHelper.AddMockMsGraphExtension(new(), TestContext);
        }

        private async Task<ServiceBuilder> ServicesWithTestExtensionArtifact(ArtifactRegistryAddress artifactRegistryAddress, BinaryData artifactPayload)
        {
            var clientFactory = RegistryHelper.CreateMockRegistryClient(artifactRegistryAddress.ClientDescriptor());
            var blobClient = clientFactory.CreateAnonymousBlobClient(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                artifactRegistryAddress.RegistryUri,
                artifactRegistryAddress.RepositoryPath);

            var configResult = await blobClient.UploadBlobAsync(BinaryData.FromString("{}"));
            var blobResult = await blobClient.UploadBlobAsync(artifactPayload);
            var manifest = BicepTestConstants.GetBicepExtensionManifest(blobResult.Value, configResult.Value);
            await blobClient.SetManifestAsync(manifest, artifactRegistryAddress.ExtensionVersion);

            var cacheRoot = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();

            return new ServiceBuilder()
                .WithFeatureOverrides(new(CacheRootDirectory: cacheRoot))
                .WithContainerRegistryClientFactory(clientFactory);
        }

        [TestMethod]
        [DynamicData(nameof(ArtifactRegistryCorruptedPackageNegativeTestScenarios), DynamicDataSourceType.Method)]
        public async Task Bicep_compiler_handles_corrupted_extension_package_gracefully(
            BinaryData payload,
            string innerErrorMessage)
        {
            // ARRANGE
            var testArtifactAddress = new ArtifactRegistryAddress("biceptestdf.azurecr.io", "bicep/extensions/microsoftgraph/beta", "0.0.0-corruptpng");

            var services = await ServicesWithTestExtensionArtifact(testArtifactAddress, payload);

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            extension '{testArtifactAddress.ToSpecificationString(':')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics([
                ("BCP396", DiagnosticLevel.Error, """The referenced extension types artifact has been published with malformed content.""")
            ]);
        }

        public record ArtifactRegistryAddress(string RegistryAddress, string RepositoryPath, string ExtensionVersion)
        {
            public string ToSpecificationString(char delim) => $"br:{RegistryAddress}/{RepositoryPath}{delim}{ExtensionVersion}";

            public RepoDescriptor ClientDescriptor() => new(RegistryAddress, RepositoryPath, [ExtensionVersion]);

            public Uri RegistryUri => new($"https://{RegistryAddress}");
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
            var mockBlobClient = StrictMock.Of<FakeRegistryBlobClient>();
            mockBlobClient.Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exceptionToThrow);

            // mock the registry client to return the mock blob client
            var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();
            containerRegistryFactoryBuilder.WithRepository(
                new RepoDescriptor(artifactRegistryAddress.RegistryAddress, artifactRegistryAddress.RepositoryPath, [artifactRegistryAddress.ExtensionVersion]), mockBlobClient.Object);

            var services = new ServiceBuilder()
                .WithContainerRegistryClientFactory(containerRegistryFactoryBuilder.Build());

            // ACT
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            extension '{artifactRegistryAddress.ToSpecificationString(':')}'
            ");

            // ASSERT
            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(expectedDiagnostics);
        }

        public static IEnumerable<object[]> ArtifactRegistryAddressNegativeTestScenarios()
        {
            // constants
            const string placeholderExtensionVersion = "0.0.0-placeholder";

            // unresolvable host registry. For example if DNS is down or unresponsive
            const string unreachableRegistryAddress = "unknown.registry.azurecr.io";
            const string NoSuchHostMessage = $" (No such host is known. ({unreachableRegistryAddress}:443))";
            var AggregateExceptionMessage = $"Retry failed after 4 tries. Retry settings can be adjusted in ClientOptions.Retry or by configuring a custom retry policy in ClientOptions.RetryPolicy.{string.Concat(Enumerable.Repeat(NoSuchHostMessage, 4))}";
            var unreachable = new ArtifactRegistryAddress(unreachableRegistryAddress, "bicep/extensions/microsoftgraph/beta", placeholderExtensionVersion);
            yield return new object[] {
                unreachable,
                new AggregateException(AggregateExceptionMessage),
                new (string, DiagnosticLevel, string)[]{
                    ("BCP192", DiagnosticLevel.Error, @$"Unable to restore the artifact with reference ""{unreachable.ToSpecificationString(':')}"": {AggregateExceptionMessage}")
                },
            };

            // manifest not found is thrown when the repository address is not registered and/or the version doesn't exist in the registry
            const string NotFoundMessage = "The artifact does not exist in the registry.";
            var withoutRepo = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "unknown/path/microsoftgraph/beta", placeholderExtensionVersion);
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
                ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
                    ("unknown.json", "{}")),
                "The path: index.json was not found in artifact contents"
            };

            // Scenario: "index.json" is not valid JSON
            yield return new object[]
            {
                ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
                    ("index.json", """{"INVALID_JSON": 777""")),
                "'7' is an invalid end of a number. Expected a delimiter. Path: $.INVALID_JSON | LineNumber: 0 | BytePositionInLine: 20."
            };

            // Scenario: "index.json" with malformed or missing required data
            yield return new object[]
            {
                ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(
                    ("index.json", """{ "UnexpectedMember": false}""")),
                "Value cannot be null. (Parameter 'source')"
            };
        }

        [TestMethod]
        public async Task External_MsGraph_namespace_can_be_loaded_from_configuration()
        {
            var services = await GetServices();

            services = services.WithConfigurationPatch(c => c.WithExtensions($$"""
            {
                "az": "builtin:",
                "msGraphBeta": "br:{{LanguageConstants.BicepPublicMcrRegistry}}/bicep/extensions/microsoftgraph/beta:{{BicepTestConstants.MsGraphVersionBeta}}",
                "msGraphV1": "br:{{LanguageConstants.BicepPublicMcrRegistry}}/bicep/extensions/microsoftgraph/v1:{{BicepTestConstants.MsGraphVersionV10}}"
            }
            """));

            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            extension msGraphBeta
            extension msGraphV1
            "));

            result.Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task MsGraph_namespace_can_be_loaded_from_configuration_if_defined()
        {
            var services = await GetServices();

            services = services.WithConfigurationPatch(c => c.WithExtensions($$"""
            {
                "az": "builtin:",
                "microsoftGraph": "br:{{LanguageConstants.BicepPublicMcrRegistry}}/bicep/extensions/microsoftgraph/beta:{{BicepTestConstants.MsGraphVersionBeta}}"
            }
            """));

            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            extension microsoftGraph
            "));

            result.Should().GenerateATemplate();
        }

        [TestMethod]
        public async Task BuiltIn_MsGraph_namespace_should_show_retired()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            extension microsoftGraph
            "));

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics([
                ("BCP407", DiagnosticLevel.Error, """Built-in extension "microsoftGraph" is retired. Use dynamic types instead. See https://aka.ms/graphbicep/dynamictypes""")
            ]);
        }

        [TestMethod]
        public async Task MsGraph_namespace_can_be_loaded_dynamically_using_extension_configuration()
        {
            //ARRANGE
            var artifactRegistryAddress = new ArtifactRegistryAddress(
                "fake.azurecr.io",
                "fake/path/microsoftgraph/beta",
                "1.0.0-fake");
            var services = await ServicesWithTestExtensionArtifact(
                artifactRegistryAddress,
                ExtensionResourceTypeHelper.GetTypesTgzBytesFromFiles(("index.json", BicepTestConstants.GetMsGraphIndexJson(BicepTestConstants.MsGraphVersionBeta))));
            services = services.WithConfigurationPatch(c => c.WithExtensions($$"""
            {
                "az": "builtin:",
                "msGraphBeta": "{{artifactRegistryAddress.ToSpecificationString(':')}}"
            }
            """));

            //ACT
            var result = await CompilationHelper.RestoreAndCompile(services, ("main.bicep", @$"
            extension msGraphBeta
            "));

            //ASSERT
            result.Should().GenerateATemplate();
            result.Template.Should().NotBeNull();
            result.Template.Should().HaveValueAtPath("$.imports.msGraphBeta.version", BicepTestConstants.MsGraphVersionBeta);
        }

        [TestMethod]
        public async Task MsGraphResourceTypeProvider_should_warn_for_property_mismatch()
        {
            var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
                typeof(ExtensionRegistryTests).Assembly,
                "Files/ExtensionRegistryTests/microsoftgraph");

            var registry = "example.azurecr.io";
            var repository = "microsoftgraph/v1";

            var services = ExtensionTestHelper.GetServiceBuilder(fileSystem, registry, repository, new());

            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), "/index.json", $"br:{registry}/{repository}:1.2.3");

            var compilation = await CompilationHelper.RestoreAndCompile(
                services,
                @"extension 'br:example.azurecr.io/microsoftgraph/v1:1.2.3'

resource app 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'test'
  displayName: 'test'
  extraProp: 'extra'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"extraProp\" is not allowed on objects of type \"Microsoft.Graph/applications\". Permissible properties include \"appId\", \"dependsOn\", \"id\", \"spa\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.")
            });

            compilation = await CompilationHelper.RestoreAndCompile(
                services,
                @"extension 'br:example.azurecr.io/microsoftgraph/v1:1.2.3'

resource app 'Microsoft.Graph/applications@v1.0' = {
  uniqueName: 'test'
  displayName: 'test'
  spa: {
    extraNestedProp: 'extra'
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"extraNestedProp\" is not allowed on objects of type \"MicrosoftGraphSpaApplication\". Permissible properties include \"redirectUris\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.")
            });
        }

        // -----------------------------------------------------------------------
        // ReadableAtDeployTime tests (Required + DeployTimeConstant fix)
        // -----------------------------------------------------------------------

        private static BinaryData GetMsGraphTypesTgzWithApplication()
        {
            // Builds a minimal MS Graph extension types package that contains a single
            // Microsoft.Graph/applications resource type with a uniqueName identifier
            // property (flags = Required | DeployTimeConstant | Identifier, value 25)
            // and a plain read-only property (createdDateTime) for the negative test.
            return ExtensionTestHelper.CreateMockExtensionMockData(
                "MicrosoftGraph",
                "1.0.0",
                "v1",
                new()
                {
                    CreateResourceTypes = (ctx, factory) =>
                    {
                        var stringType = factory.Create(() => new Azure.Bicep.Types.Concrete.StringType());

                        var appBody = factory.Create(() => new Azure.Bicep.Types.Concrete.ObjectType(
                            "Microsoft.Graph/applications",
                            new Dictionary<string, Azure.Bicep.Types.Concrete.ObjectTypeProperty>
                            {
                                ["uniqueName"] = new(
                                    factory.GetReference(stringType),
                                    Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.Required
                                        | Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.DeployTimeConstant
                                        | Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.Identifier,
                                    "The unique name of the application"),
                                ["createdDateTime"] = new(
                                    factory.GetReference(stringType),
                                    Azure.Bicep.Types.Concrete.ObjectTypePropertyFlags.ReadOnly,
                                    "The date and time the application was created (read-only, server-generated)"),
                            },
                            null));

                        return [factory.Create(() => new Azure.Bicep.Types.Concrete.ResourceType(
                            "Microsoft.Graph/applications@v1.0",
                            factory.GetReference(appBody),
                            null,
                            writableScopes_in: Azure.Bicep.Types.Concrete.ScopeType.All,
                            readableScopes_in: Azure.Bicep.Types.Concrete.ScopeType.All))];
                    }
                }).TypesTgzData;
        }

        [TestMethod]
        public async Task MsGraph_uniqueName_is_readable_at_deploy_time()
        {
            // ARRANGE — publish a mock MS Graph extension with an applications resource
            var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                GetMsGraphTypesTgzWithApplication(),
                new(),
                artifactTarget: "example.azurecr.io/microsoftgraph/v1:1.0.0");

            // ACT — use graphApp.uniqueName (Required + DeployTimeConstant) as storage account name
            var result = await CompilationHelper.RestoreAndCompile(services, """
                extension 'br:example.azurecr.io/microsoftgraph/v1:1.0.0' as graph

                resource graphApp 'Microsoft.Graph/applications@v1.0' = {
                  uniqueName: 'myApp'
                }

                resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                  name: graphApp.uniqueName
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """);

            // ASSERT — must compile clean, no BCP120
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task MsGraph_readOnly_property_is_not_readable_at_deploy_time()
        {
            // ARRANGE — same extension as above
            var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                GetMsGraphTypesTgzWithApplication(),
                new(),
                artifactTarget: "example.azurecr.io/microsoftgraph/v1:1.0.0");

            // ACT — use graphApp.createdDateTime (ReadOnly only, NOT Required+DTC) as storage account name
            var result = await CompilationHelper.RestoreAndCompile(services, """
                extension 'br:example.azurecr.io/microsoftgraph/v1:1.0.0' as graph

                resource graphApp 'Microsoft.Graph/applications@v1.0' = {
                  uniqueName: 'myApp'
                }

                resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                  name: graphApp.createdDateTime
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """);

            // ASSERT — must still produce BCP120 (createdDateTime is server-generated, not a deploy-time constant)
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
                ("BCP120", DiagnosticLevel.Error,
                    """This expression is being used in an assignment to the "name" property of the "Microsoft.Storage/storageAccounts" type, which requires a value that can be calculated at the start of the deployment. Properties of graphApp which can be calculated at the start include "uniqueName".""")
            ]);
        }
    }
}
