// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests
{
    public record TestFeatureProvider(
        string AssemblyVersion,
        string CacheRootDirectory,
        bool RegistryEnabled,
        bool SymbolicNameCodegenEnabled,
        bool ImportsEnabled,
        bool AdvancedListComprehensionEnabled,
        bool ResourceTypedParamsAndOutputsEnabled,
        bool SourceMappingEnabled) : IFeatureProvider;

    public static class BicepTestConstants
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly FileResolver FileResolver = new();

        public static readonly TestFeatureProvider Features = CreateFeatureProvider(registryEnabled: false, symbolicNameCodegenEnabled: false, importsEnabled: false, resourceTypedParamsAndOutputsEnabled: false, sourceMappingEnabled: false, assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion);

        public static readonly EmitterSettings EmitterSettings = new EmitterSettings(Features);

        public static readonly IAzResourceTypeLoader AzResourceTypeLoader = new AzResourceTypeLoader();

        public static readonly INamespaceProvider NamespaceProvider = TestTypeHelper.CreateWithAzTypes();

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        public static readonly IModuleRegistryProvider RegistryProvider = new DefaultModuleRegistryProvider(FileResolver, ClientFactory, TemplateSpecRepositoryFactory, Features);

        public static readonly IConfigurationManager ConfigurationManager = new ConfigurationManager(new IOFileSystem());

        // Linter rules added to this list will be automtically disabled for most tests.
        public static readonly string[] AnalyzerRulesToDisableInTests = new string[] {
        };

        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersEnabled = ConfigurationManager.GetBuiltInConfiguration();
        public static readonly RootConfiguration BuiltInConfigurationWithAllAnalyzersDisabled = ConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();
        public static readonly RootConfiguration BuiltInConfigurationWithProblematicAnalyzersDisabled = ConfigurationManager.GetBuiltInConfiguration().WithAnalyzersDisabled(AnalyzerRulesToDisableInTests);

        // By default turns off only problematic analyzers
        public static readonly RootConfiguration BuiltInConfiguration = BuiltInConfigurationWithProblematicAnalyzersDisabled;

        // By default turns off only problematic analyzers
        public static readonly LinterAnalyzer LinterAnalyzer = new LinterAnalyzer(BuiltInConfiguration);

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
                ["analyzers"] = new Dictionary<string, object>(),
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

        public static TestFeatureProvider CreateFeaturesProvider(
            TestContext testContext,
            bool registryEnabled = false,
            bool symbolicNameCodegenEnabled = false,
            bool importsEnabled = false,
            bool resourceTypedParamsAndOutputsEnabled = false,
            bool sourceMappingEnabled = false,
            string assemblyFileVersion = DevAssemblyFileVersion)
        {
            var features = CreateFeatureProvider(
                registryEnabled,
                symbolicNameCodegenEnabled,
                importsEnabled,
                resourceTypedParamsAndOutputsEnabled,
                sourceMappingEnabled,
                assemblyFileVersion);

            return features with
            {
                CacheRootDirectory = FileHelper.GetCacheRootPath(testContext),
            };
        }

        private static TestFeatureProvider CreateFeatureProvider(
            bool registryEnabled,
            bool symbolicNameCodegenEnabled,
            bool importsEnabled,
            bool resourceTypedParamsAndOutputsEnabled,
            bool sourceMappingEnabled,
            string assemblyFileVersion)
        {
            return new TestFeatureProvider(
                assemblyFileVersion,
                string.Empty,
                registryEnabled,
                symbolicNameCodegenEnabled,
                importsEnabled,
                true,
                resourceTypedParamsAndOutputsEnabled,
                sourceMappingEnabled);
        }

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
