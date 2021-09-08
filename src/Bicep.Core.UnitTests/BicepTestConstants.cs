// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
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

        public static readonly IFeatureProvider Features = CreateMockFeaturesProvider(registryEnabled: false, symbolicNameCodegenEnabled: false).Object;

        public static readonly IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

        public static readonly ITemplateSpecRepositoryFactory TemplateSpecRepositoryFactory = StrictMock.Of<ITemplateSpecRepositoryFactory>().Object;

        public static readonly IModuleRegistryProvider RegistryProvider = new DefaultModuleRegistryProvider(FileResolver, ClientFactory, TemplateSpecRepositoryFactory, Features);

        public static IFeatureProvider CreateFeaturesProvider(TestContext testContext, bool registryEnabled = false, bool symbolicNameCodegenEnabled = false)
        {
            var mock = CreateMockFeaturesProvider(
                registryEnabled: registryEnabled,
                symbolicNameCodegenEnabled: symbolicNameCodegenEnabled);

            var testPath = FileHelper.GetCacheRootPath(testContext);
            mock.SetupGet(m => m.CacheRootDirectory).Returns(testPath);

            return mock.Object;
        }

        private static Mock<IFeatureProvider> CreateMockFeaturesProvider(bool registryEnabled, bool symbolicNameCodegenEnabled)
        {
            var mock = StrictMock.Of<IFeatureProvider>();
            mock.SetupGet(m => m.RegistryEnabled).Returns(registryEnabled);
            mock.SetupGet(m => m.SymbolicNameCodegenEnabled).Returns(symbolicNameCodegenEnabled);

            return mock;
        }
    }
}
