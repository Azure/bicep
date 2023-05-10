// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Telemetry;
using Moq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests
{
    public static class BicepTestConstants
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly FileResolver FileResolver = new(new IOFileSystem());

        public static readonly FeatureProviderOverrides FeatureOverrides = new();

        public static readonly ConfigurationManager ConfigurationManager = CreateFilesystemConfigurationManager();

        public static readonly IFeatureProviderFactory FeatureProviderFactory = new OverriddenFeatureProviderFactory(new FeatureProviderFactory(ConfigurationManager), FeatureOverrides);

        public static readonly IAzResourceTypeLoaderFactory AzResourceTypeLoaderFactory = new AzResourceTypeLoaderFactory(FeatureProviderFactory);

        public static readonly INamespaceProvider NamespaceProvider = new DefaultNamespaceProvider(AzResourceTypeLoaderFactory);

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        // Linter rules added to this list will be automtically disabled for most tests.
        public static readonly string[] AnalyzerRulesToDisableInTests = new string[] {
            // use-recent-api-versions is problematic for tests but it's off by default so doesn't need to appear here
        };

        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersEnabled = IConfigurationManager.GetBuiltInConfiguration();
        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersDisabled = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();
        public static readonly RootConfiguration BuiltInConfigurationWithProblematicAnalyzersDisabled = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersDisabled(AnalyzerRulesToDisableInTests);

        // By default turns off only problematic analyzers
        public static readonly RootConfiguration BuiltInConfiguration = BuiltInConfigurationWithProblematicAnalyzersDisabled;

        public static readonly IConfigurationManager BuiltInOnlyConfigurationManager = IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration);

        public static readonly IFeatureProvider Features = new OverriddenFeatureProvider(new FeatureProvider(BuiltInConfiguration), FeatureOverrides);

        public static readonly IModuleRegistryProvider RegistryProvider = new DefaultModuleRegistryProvider(FileResolver, ClientFactory, TemplateSpecRepositoryFactory, FeatureProviderFactory, BuiltInOnlyConfigurationManager);

        public static readonly IModuleDispatcher ModuleDispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider, IConfigurationManager.WithStaticConfiguration(BuiltInConfiguration));

        // By default turns off only problematic analyzers
        public static readonly LinterAnalyzer LinterAnalyzer = new LinterAnalyzer();

        public static readonly IModuleRestoreScheduler ModuleRestoreScheduler = CreateMockModuleRestoreScheduler();
        public static readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider(Features, AzResourceTypeLoaderFactory.GetResourceTypeLoader(null, Features));
        public static readonly IApiVersionProviderFactory ApiVersionProviderFactory = IApiVersionProviderFactory.WithStaticApiVersionProvider(ApiVersionProvider);

        public static RootConfiguration CreateMockConfiguration(Dictionary<string, object>? customConfigurationData = null, string? configurationPath = null)
        {
            var configurationData = new Dictionary<string, object>
            {
                ["cloud.currentProfile"] = "AzureCloud",
                ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "https://example.invalid",
                ["cloud.profiles.AzureCloud.activeDirectoryAuthority"] = "https://example.invalid",
                ["cloud.credentialPrecedence"] = new[] { "AzureCLI", "AzurePowerShell" },
                ["moduleAliases"] = new Dictionary<string, object>(),
                ["analyzers"] = new Dictionary<string, object>(),
                ["experimentalFeaturesEnabled"] = new Dictionary<string, bool>(),
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

        public static ConfigurationManager CreateFilesystemConfigurationManager() => new ConfigurationManager(new IOFileSystem());

        public static IConfigurationManager CreateConfigurationManager(Func<RootConfiguration, RootConfiguration> patchFunc)
            => new PatchingConfigurationManager(CreateFilesystemConfigurationManager(), patchFunc);

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
    }
}
