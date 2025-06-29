// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Telemetry;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using OnDiskFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests
{
    public static class BicepTestConstants
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string BicepRegistryFolderName = "br";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly IFileSystem FileSystem = new OnDiskFileSystem();

        public static readonly FileResolver FileResolver = new(FileSystem);

        public static readonly IFileExplorer FileExplorer = new FileSystemFileExplorer(FileSystem);

        public static readonly FeatureProviderOverrides FeatureOverrides = new();

        public static readonly ConfigurationManager ConfigurationManager = CreateFilesystemConfigurationManager();

        public static readonly IFeatureProviderFactory FeatureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(ConfigurationManager, FileExplorer), FeatureOverrides);

        public static readonly IAuxiliaryFileCache AuxiliaryFileCache = new AuxiliaryFileCache();

        public static readonly ISourceFileFactory SourceFileFactory = new SourceFileFactory(ConfigurationManager, FeatureProviderFactory, AuxiliaryFileCache, FileExplorer);

        public static readonly BicepFile DummyBicepFile = CreateDummyBicepFile();

        public static readonly IResourceTypeProviderFactory ResourceTypeProviderFactory = new ResourceTypeProviderFactory();

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        // Linter rules added to this list will be automatically disabled for most tests.
        public static readonly string[] NonStableAnalyzerRules = [UseRecentApiVersionRule.Code, UseRecentModuleVersionsRule.Code];

        // Rules that are currently skipped due to configuration for ProgramsShouldProduceExpectedDiagnostics
        public static readonly string[] TestAnalyzersToSkip = [UseRecentApiVersionRule.Code, UseRecentModuleVersionsRule.Code, NoHardcodedLocationRule.Code, ExplicitValuesForLocationParamsRule.Code, NoLocationExprOutsideParamsRule.Code];

        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersDisabled = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();
        public static readonly RootConfiguration BuiltInConfigurationWithStableAnalyzers = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzers().WithAnalyzersDisabled(NonStableAnalyzerRules);

        public static readonly RootConfiguration BuiltInConfiguration = BuiltInConfigurationWithStableAnalyzers;

        public static readonly IConfigurationManager BuiltInOnlyConfigurationManager = IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration);

        public static readonly IFeatureProvider Features = new OverriddenFeatureProvider(new FeatureProvider(BuiltInConfiguration, FileExplorer), FeatureOverrides);

        public static readonly INamespaceProvider NamespaceProvider = new NamespaceProvider(ResourceTypeProviderFactory);

        public static readonly IServiceProvider EmptyServiceProvider = new Mock<IServiceProvider>(MockBehavior.Loose).Object;

        public static IArtifactRegistryProvider CreateRegistryProvider(IServiceProvider services) =>
            new DefaultArtifactRegistryProvider(services, ClientFactory, TemplateSpecRepositoryFactory);

        public static IModuleDispatcher CreateModuleDispatcher(IServiceProvider services) => new ModuleDispatcher(CreateRegistryProvider(services));

        public static readonly NamespaceResolver DefaultNamespaceResolver = NamespaceResolver.Create([
            new("az", AzNamespaceType.Create("az", ResourceScope.ResourceGroup, AzNamespaceType.BuiltInTypeProvider, BicepSourceFileKind.BicepFile), null),
            new("sys", SystemNamespaceType.Create("sys", Features, BicepSourceFileKind.BicepFile), null),
        ]);

        // By default turns off only problematic analyzers
        public static readonly LinterAnalyzer LinterAnalyzer = new(EmptyServiceProvider);

        public static readonly IEnvironment EmptyEnvironment = TestEnvironment.Default;

        public static RootConfiguration GetConfiguration(string contents)
            => RootConfiguration.Bind(IConfigurationManager.BuiltInConfigurationElement.Merge(JsonElementFactory.CreateElement(contents)));

        public static RootConfiguration CreateMockConfiguration(Dictionary<string, object>? customConfigurationData = null, string? configFilePath = null)
        {
            var configurationData = new Dictionary<string, object>
            {
                ["cloud.currentProfile"] = "AzureCloud",
                ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "https://example.invalid",
                ["cloud.profiles.AzureCloud.activeDirectoryAuthority"] = "https://example.invalid",
                ["cloud.credentialPrecedence"] = new[] { "AzureCLI", "AzurePowerShell" },
                ["moduleAliases"] = new Dictionary<string, object>(),
                ["extensions"] = new Dictionary<string, object>(),
                ["implicitExtensions"] = new[] { "az" },
                ["analyzers"] = new Dictionary<string, object>(),
                ["experimentalFeaturesEnabled"] = new Dictionary<string, bool>(),
                ["formatting"] = new Dictionary<string, bool>(),
            };

            if (customConfigurationData is not null)
            {
                foreach (var (path, value) in customConfigurationData)
                {
                    configurationData[path] = value;
                }
            }

            var element = JsonElementFactory.CreateElement("{}");

            foreach (var (path, value) in configurationData)
            {
                element = element.SetPropertyByPath(path, value);
            }

            IOUri? configFileIdentifier = configFilePath is not null ? new IOUri("file", "", configFilePath) : null;

            return RootConfiguration.Bind(element, configFileIdentifier);
        }

        public static ConfigurationManager CreateFilesystemConfigurationManager() => new(new FileSystemFileExplorer(new OnDiskFileSystem()));

        public static IFeatureProviderFactory CreateFeatureProviderFactory(FeatureProviderOverrides featureOverrides, IConfigurationManager? configurationManager = null)
            => new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager ?? CreateFilesystemConfigurationManager(), FileExplorer), featureOverrides);

        public static Mock<ITelemetryProvider> CreateMockTelemetryProvider()
        {
            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            return telemetryProvider;
        }

        public static BinaryData GetBicepExtensionManifest(UploadRegistryBlobResult layer, UploadRegistryBlobResult config) =>
            BinaryData.FromString($$"""
        {
            "schemaVersion": 2,
            "mediaType": "application/vnd.oci.image.manifest.v1+json",
            "artifactType": "{{BicepMediaTypes.BicepExtensionArtifactType}}",
            "config": {
            "mediaType": "{{BicepMediaTypes.BicepExtensionConfigV1}}",
            "digest": "{{config.Digest}}",
            "size": {{config.SizeInBytes}}
            },
            "layers": [
            {
                "mediaType": "{{BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip}}",
                "digest": "{{layer.Digest}}",
                "size": {{layer.SizeInBytes}}
            }
            ],
            "annotations": {
            "bicep.serialization.format": "v1",
            "org.opencontainers.image.created": "2023-05-04T16:40:05Z"
            }
        }
        """);

        public static BicepFile CreateDummyBicepFile(RootConfiguration? configuration = null, FeatureProviderOverrides? featureOverrides = null)
        {
            var configurationManager = IConfigurationManager.WithStaticConfiguration(configuration ?? IConfigurationManager.GetBuiltInConfiguration());
            var featureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager, FileExplorer), featureOverrides ?? FeatureOverrides);

            return CreateDummyBicepFile(configurationManager, featureProviderFactory);
        }

        public static BicepFile CreateDummyBicepFile(IConfigurationManager configurationManager, IFeatureProviderFactory? featureProviderFactory = null)
        {
            return new(
                new Uri($"inmemory:///main.bicep"),
                DummyFileHandle.Instance,
                [],
                SyntaxFactory.EmptyProgram,
                configurationManager,
                featureProviderFactory ?? FeatureProviderFactory,
                BicepTestConstants.AuxiliaryFileCache,
                EmptyDiagnosticLookup.Instance,
                EmptyDiagnosticLookup.Instance);
        }

        public readonly static string BuiltinAzExtensionVersion = AzNamespaceType.Settings.TemplateExtensionVersion;

        public const string MsGraphVersionV10 = "1.2.3";
        public const string MsGraphVersionBeta = "1.2.3-beta";

        public static string GetMsGraphIndexJson(string version)
        {
            var isBeta = version.EndsWithInsensitively("-beta");

            return
                $$"""
                  {
                    "resources": {},
                    "resourceFunctions": {},
                    "settings": {
                      "name": "MicrosoftGraph{{(isBeta ? "Beta" : "V1.0")}}",
                      "version": "{{version}}",
                      "isSingleton": false
                    }
                  }
                  """;
        }
    }
}
