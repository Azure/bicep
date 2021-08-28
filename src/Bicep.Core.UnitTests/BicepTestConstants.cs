// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests
{
    public static class BicepTestConstants 
    {
        public const string DevAssemblyFileVersion = "dev";

        public const string GeneratorTemplateHashPath = "metadata._generator.templateHash";

        public static readonly FileResolver FileResolver = new();

        public static readonly IFeatureProvider Features = CreateMockFeaturesProvider(registryEnabled: false).Object;

        public static readonly IContainerRegistryClientFactory ClientFactory = new Mock<IContainerRegistryClientFactory>(MockBehavior.Strict).Object;

        public static readonly IModuleRegistryProvider RegistryProvider = new DefaultModuleRegistryProvider(FileResolver, ClientFactory, Features);

        public static IFeatureProvider CreateFeaturesProvider(TestContext testContext, bool registryEnabled = false)
        {
            var mock = CreateMockFeaturesProvider(registryEnabled);

            var testPath = FileHelper.GetCacheRootPath(testContext);
            mock.SetupGet(m => m.CacheRootDirectory).Returns(testPath);

            return mock.Object;
        }

        private static Mock<IFeatureProvider> CreateMockFeaturesProvider(bool registryEnabled)
        {
            var mock = new Mock<IFeatureProvider>(MockBehavior.Strict);
            mock.SetupGet(m => m.RegistryEnabled).Returns(registryEnabled);

            return mock;
        }
    }
}
