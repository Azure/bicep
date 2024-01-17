// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using Azure.Core.Pipeline;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Telemetry;
using Moq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests
{
    public static class BicepTestConstants
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string BicepRegistryFolderName = "br";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly IFileSystem FileSystem = new IOFileSystem();
        public static readonly FileResolver FileResolver = new(FileSystem);

        public static readonly FeatureProviderOverrides FeatureOverrides = new();

        public static readonly ConfigurationManager ConfigurationManager = CreateFilesystemConfigurationManager();

        public static readonly IFeatureProviderFactory FeatureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(ConfigurationManager), FeatureOverrides);

        public static readonly IResourceTypeProviderFactory ResourceTypeProviderFactory = new ResourceTypeProviderFactory(FileSystem);

        public static readonly INamespaceProvider NamespaceProvider = new DefaultNamespaceProvider(ResourceTypeProviderFactory);

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        public static ResourceTypesProviderDescriptor BuiltInAzProviderDescriptor { get; } = new(
            AzNamespaceType.BuiltInName,
            AzNamespaceType.Settings.ArmTemplateProviderVersion,
            isImplicitImport: true);

        public static ResourceTypesProviderDescriptor BuiltInSysProviderDescriptor { get; } = new(
            SystemNamespaceType.BuiltInName,
            SystemNamespaceType.Settings.ArmTemplateProviderVersion,
            isImplicitImport: true);

        public static ResourceTypesProviderDescriptor MicrosoftGraphProviderDescriptor { get; } = new(
            MicrosoftGraphNamespaceType.BuiltInName,
            MicrosoftGraphNamespaceType.Settings.ArmTemplateProviderVersion,
            isImplicitImport: true);

        // Linter rules added to this list will be automtically disabled for most tests.
        // use-recent-api-versions is problematic for tests but it's off by default so doesn't need to appear here
        public static readonly string[] AnalyzerRulesToDisableInTests = Array.Empty<string>();

        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersEnabled = IConfigurationManager.GetBuiltInConfiguration();
        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersDisabled = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();
        public static readonly RootConfiguration BuiltInConfigurationWithProblematicAnalyzersDisabled = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersDisabled(AnalyzerRulesToDisableInTests);

        // By default turns off only problematic analyzers
        public static readonly RootConfiguration BuiltInConfiguration = BuiltInConfigurationWithProblematicAnalyzersDisabled;

        public static readonly IConfigurationManager BuiltInOnlyConfigurationManager = IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration);

        public static readonly IFeatureProvider Features = new OverriddenFeatureProvider(new FeatureProvider(BuiltInConfiguration), FeatureOverrides);

        public static readonly IServiceProvider EmptyServiceProvider = new Mock<IServiceProvider>(MockBehavior.Loose).Object;

        public static readonly IArtifactRegistryProvider RegistryProvider = new DefaultArtifactRegistryProvider(EmptyServiceProvider, FileResolver, FileSystem, ClientFactory, TemplateSpecRepositoryFactory, FeatureProviderFactory, BuiltInOnlyConfigurationManager);

        public static readonly IModuleDispatcher ModuleDispatcher = new ModuleDispatcher(RegistryProvider, IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration));

        // By default turns off only problematic analyzers
        public static readonly LinterAnalyzer LinterAnalyzer = new();

        public static IEnvironment EmptyEnvironment = new TestEnvironment(ImmutableDictionary<string, string?>.Empty);

        public static readonly IModuleRestoreScheduler ModuleRestoreScheduler = CreateMockModuleRestoreScheduler();

        public static RootConfiguration CreateMockConfiguration(Dictionary<string, object>? customConfigurationData = null, string? configurationPath = null)
        {
            var configurationData = new Dictionary<string, object>
            {
                ["cloud.currentProfile"] = "AzureCloud",
                ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "https://example.invalid",
                ["cloud.profiles.AzureCloud.activeDirectoryAuthority"] = "https://example.invalid",
                ["cloud.credentialPrecedence"] = new[] { "AzureCLI", "AzurePowerShell" },
                ["moduleAliases"] = new Dictionary<string, object>(),
                ["providerAliases"] = new Dictionary<string, object>(),
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

            return RootConfiguration.Bind(element, configurationPath);
        }

        public static ConfigurationManager CreateFilesystemConfigurationManager() => new(new IOFileSystem());

        public static IFeatureProviderFactory CreateFeatureProviderFactory(FeatureProviderOverrides featureOverrides, IConfigurationManager? configurationManager = null)
            => new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager ?? CreateFilesystemConfigurationManager()), featureOverrides);

        private static IModuleRestoreScheduler CreateMockModuleRestoreScheduler()
        {
            var moduleDispatcher = StrictMock.Of<IModuleDispatcher>();
            return new ModuleRestoreScheduler(moduleDispatcher.Object);
        }

        public static Mock<ITelemetryProvider> CreateMockTelemetryProvider()
        {
            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            return telemetryProvider;
        }

        public static BinaryData BicepProviderManifestWithEmptyTypesLayer
            => GetBicepProviderManifest("sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", 0);

        public static BinaryData GetBicepProviderManifest(string digest, long size) =>
            BinaryData.FromString($$"""
        {
            "schemaVersion": 2,
            "mediaType": "application/vnd.oci.image.manifest.v1+json",
            "artifactType": "{{BicepMediaTypes.BicepProviderArtifactType}}",
            "config": {
            "mediaType": "{{BicepMediaTypes.BicepProviderConfigV1}}",
            "digest": "sha256:44136fa355b3678a1146ad16f7e8649e94fb4fc21fe77e8310c060f61caaff8a",
            "size": 2
            },
            "layers": [
            {
                "mediaType": "{{BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip}}",
                "digest": "{{digest}}",
                "size": {{size}}
            }
            ],
            "annotations": {
            "bicep.serialization.format": "v1",
            "org.opencontainers.image.created": "2023-05-04T16:40:05Z"
            }
        }
        """);

        public static string BuiltinAzProviderVersion = AzNamespaceType.Settings.ArmTemplateProviderVersion;
    }
}
