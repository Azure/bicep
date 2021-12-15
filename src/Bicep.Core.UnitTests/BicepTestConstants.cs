// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
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
    public static class BicepTestConstants
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly FileResolver FileResolver = new();

        public static readonly IFeatureProvider Features = CreateMockFeaturesProvider(registryEnabled: false, symbolicNameCodegenEnabled: false, importsEnabled: false, assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion).Object;

        public static readonly EmitterSettings EmitterSettings = new EmitterSettings(Features);

        public static readonly EmitterSettings EmitterSettingsWithSymbolicNames = new EmitterSettings(
            CreateMockFeaturesProvider(registryEnabled: false, symbolicNameCodegenEnabled: true, importsEnabled: false, assemblyFileVersion: BicepTestConstants.DevAssemblyFileVersion).Object);

        public static readonly IAzResourceTypeLoader AzResourceTypeLoader = new AzResourceTypeLoader();

        public static readonly INamespaceProvider NamespaceProvider = TestTypeHelper.CreateWithAzTypes();

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        public static readonly IModuleRegistryProvider RegistryProvider = new DefaultModuleRegistryProvider(FileResolver, ClientFactory, TemplateSpecRepositoryFactory, Features);

        public static readonly IConfigurationManager ConfigurationManager = new ConfigurationManager(new IOFileSystem());

        public static readonly RootConfiguration BuiltInConfiguration = ConfigurationManager.GetBuiltInConfiguration();

        public static readonly RootConfiguration BuiltInConfigurationWithAnalyzersDisabled = ConfigurationManager.GetBuiltInConfiguration(disableAnalyzers: true);

        public static readonly IModuleRestoreScheduler ModuleRestoreScheduler = CreateMockModuleRestoreScheduler();

        public static IFeatureProvider CreateFeaturesProvider(
            TestContext testContext,
            bool registryEnabled = false,
            bool symbolicNameCodegenEnabled = false,
            bool importsEnabled = false,
            string assemblyFileVersion = DevAssemblyFileVersion)
        {
            var mock = CreateMockFeaturesProvider(
                registryEnabled: registryEnabled,
                symbolicNameCodegenEnabled: symbolicNameCodegenEnabled,
                importsEnabled: importsEnabled,
                assemblyFileVersion: assemblyFileVersion);

            var testPath = FileHelper.GetCacheRootPath(testContext);
            mock.SetupGet(m => m.CacheRootDirectory).Returns(testPath);

            return mock.Object;
        }

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

        private static Mock<IFeatureProvider> CreateMockFeaturesProvider(bool registryEnabled, bool symbolicNameCodegenEnabled, bool importsEnabled, string assemblyFileVersion)
        {
            var mock = StrictMock.Of<IFeatureProvider>();
            mock.SetupGet(m => m.RegistryEnabled).Returns(registryEnabled);
            mock.SetupGet(m => m.SymbolicNameCodegenEnabled).Returns(symbolicNameCodegenEnabled);
            mock.SetupGet(m => m.ImportsEnabled).Returns(importsEnabled);
            mock.SetupGet(m => m.AssemblyVersion).Returns(assemblyFileVersion);

            return mock;
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
