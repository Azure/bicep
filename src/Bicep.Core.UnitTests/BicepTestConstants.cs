// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceCode;
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

        public static readonly FileResolver FileResolver = new(new IOFileSystem());

        public static readonly FeatureProviderOverrides FeatureOverrides = new();

        public static readonly ConfigurationManager ConfigurationManager = CreateFilesystemConfigurationManager();

        public static readonly IFeatureProviderFactory FeatureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(ConfigurationManager), FeatureOverrides);

        public static readonly IResourceTypeProviderFactory ResourceTypeProviderFactory = new ResourceTypeProviderFactory();

        public static readonly INamespaceProvider NamespaceProvider = new DefaultNamespaceProvider(ResourceTypeProviderFactory);

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

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

        public static readonly IArtifactRegistryProvider RegistryProvider = new DefaultArtifactRegistryProvider(EmptyServiceProvider, FileResolver, ClientFactory, TemplateSpecRepositoryFactory, FeatureProviderFactory, BuiltInOnlyConfigurationManager);

        public static readonly IModuleDispatcher ModuleDispatcher = new ModuleDispatcher(RegistryProvider, IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration));

        // By default turns off only problematic analyzers
        public static readonly LinterAnalyzer LinterAnalyzer = new();

        public static IEnvironment EmptyEnvironment = new TestEnvironment(ImmutableDictionary<string, string?>.Empty);

        public static readonly IModuleRestoreScheduler ModuleRestoreScheduler = CreateMockModuleRestoreScheduler(null/*asdfg?*/).Item2; //asdfg

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

        public static IConfigurationManager CreateConfigurationManager(Func<RootConfiguration, RootConfiguration> patchFunc)
            => new PatchingConfigurationManager(CreateFilesystemConfigurationManager(), patchFunc);

        public static IFeatureProviderFactory CreateFeatureProviderFactory(FeatureProviderOverrides featureOverrides, IConfigurationManager? configurationManager = null)
            => new OverriddenFeatureProviderFactory(new FeatureProviderFactory(configurationManager ?? CreateFilesystemConfigurationManager()), featureOverrides);

        public static (IModuleDispatcher, IModuleRestoreScheduler) CreateMockModuleRestoreScheduler(IArtifactRegistry[]? artifactRegistries /*asdfg= null*/)
        {
            var moduleDispatcher = StrictMock.Of<IModuleDispatcher>();//asdfgasdfg
            moduleDispatcher.Setup(x => x.RestoreModules(It.IsAny<ImmutableArray<ArtifactReference>>(), It.IsAny<bool>())).
                ReturnsAsync(true); //asdfg
            moduleDispatcher.Setup(x => x.PruneRestoreStatuses());

            MockRepository repository = new(MockBehavior.Strict);
            var provider = repository.Create<IArtifactRegistryProvider>(); //ASDFGasdfg

            moduleDispatcher.Setup(m => m.TryGetModuleSourceArchive(It.IsAny<ArtifactReference>())).Returns((ArtifactReference reference) => //asdfgasdfg
                (artifactRegistries ?? new IArtifactRegistry[] { }).Select(r => r.TryGetSource(reference)).FirstOrDefault(s => s is not null));

            return (moduleDispatcher.Object, new ModuleRestoreScheduler(moduleDispatcher.Object)); //asdfg refactor?
        }

        private class MockRegistry : IArtifactRegistry //asdfg duplicated
        {
            public ConcurrentStack<IEnumerable<ArtifactReference>> ModuleRestores { get; } = new();

            public string Scheme => "mock";

            public RegistryCapabilities GetCapabilities(ArtifactReference reference) => throw new NotImplementedException();

            public bool IsArtifactRestoreRequired(ArtifactReference reference) => true;

            public Task PublishModule(ArtifactReference reference, Stream compiledArmTemplates, Stream? bicepSources, string? documentationUri, string? description)
                => throw new NotImplementedException();

            public Task PublishProvider(ArtifactReference reference, Stream typesTgz)
                => throw new NotImplementedException();

            public Task<bool> CheckArtifactExists(ArtifactReference reference) => throw new NotImplementedException();

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> references)
            {
                throw new NotImplementedException();
            }

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references)
            {
                this.ModuleRestores.Push(references);
                return Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>>(new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>());
            }

            public ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference reference)
            {
                throw new NotImplementedException();
            }

            public string? GetDocumentationUri(ArtifactReference reference) => null;

            public Task<string?> TryGetDescription(ArtifactReference reference) => Task.FromResult<string?>(null);

            public ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? _, string reference)
            {
                return new(new MockArtifactRef(reference, PathHelper.FilePathToFileUrl(Path.GetTempFileName())));
            }

            public SourceArchive? TryGetSource(ArtifactReference artifactReference) => null;
        }

        private class MockArtifactRef : ArtifactReference //asdfg duplicated
        {
            public MockArtifactRef(string value, Uri parentModuleUri)
                : base("mock", parentModuleUri)
            {
                this.Value = value;
            }

            public string Value { get; }

            public override string UnqualifiedReference => this.Value;

            public override bool IsExternal => true;
        }


        public static Mock<ITelemetryProvider> CreateMockTelemetryProvider()
        {
            var telemetryProvider = StrictMock.Of<ITelemetryProvider>();
            telemetryProvider.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()));

            return telemetryProvider;
        }

        public static BinaryData BicepProviderManifestWithEmptyTypesLayer = BinaryData.FromString($$"""
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
                "digest": "sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                "size": 0
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
