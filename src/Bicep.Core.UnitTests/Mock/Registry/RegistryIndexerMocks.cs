// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Indexing;
using FluentAssertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Registry
{
    public static class RegistryIndexerMocks
    {
        private const string PublicRegistry = "mcr.microsoft.com";

        public static Mock<IPublicModuleMetadataProvider> MockPublicMetadataProvider(
            IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<RegistryModuleVersionMetadata> versions)> modules)
        {
            if (modules.Any())
            {
                modules.Should().AllSatisfy(
                    m => m.moduleName.Should().StartWith("bicep/", "All public modules should start with '/bicep'")
                );
            }

            var publicProvider = StrictMock.Of<IPublicModuleMetadataProvider>();

            publicProvider.Setup(x => x.Registry).Returns(PublicRegistry);

            publicProvider.Setup(x => x.TryGetModulesAsync())
                .ReturnsAsync([.. modules.Select(m => new RegistryModuleMetadata(PublicRegistry, m.moduleName, m.description, m.documentationUri))]);
            publicProvider.Setup(x => x.TryGetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions.Select(v => v.Version)]);

            publicProvider.Setup(x => x.TryGetModuleVersionMetadataAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string modulePath, string version) =>
                    modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions.SingleOrDefault(v => v.Version.EqualsOrdinally(version)));

            return publicProvider;
        }

        public static Mock<IRegistryModuleMetadataProvider> MockPrivateMetadataProvider(
            string registry,
            IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<RegistryModuleVersionMetadata> versions)> modules
        )
        {
            var privateProvider = StrictMock.Of<IRegistryModuleMetadataProvider>();

            privateProvider.Setup(x => x.Registry).Returns(registry);

            privateProvider.Setup(x => x.TryGetModulesAsync())
                .ReturnsAsync([.. modules.Select(m => new RegistryModuleMetadata(registry, m.moduleName, m.description, m.documentationUri))]);
            privateProvider.Setup(x => x.TryGetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions.Select(v => v.Version)]);

            return privateProvider;
        }

        public static Mock<IRegistryModuleMetadataProvider> MockFailingPrivateMetadataProvider(
            string registry,
            Exception? exception = null
        )
        {
            exception ??= new Exception($"Loading metadata for registry {registry} (intentionally) failed");

            var privateProvider = StrictMock.Of<IRegistryModuleMetadataProvider>();
            privateProvider.Setup(x => x.Registry).Returns(registry);
            privateProvider.Setup(x => x.DownloadError).Returns(exception.Message);
            privateProvider.Setup(x => x.TryGetModulesAsync()).ReturnsAsync([]);

            return privateProvider;
        }

        public static IRegistryIndexer CreateRegistryIndexer(
            Mock<IPublicModuleMetadataProvider>? publicProvider = null,
            params Mock<IRegistryModuleMetadataProvider>[] privateProviders
        )
        {
            if (publicProvider is null)
            {
                publicProvider = MockPublicMetadataProvider([]);
            }

            var privateFactory = StrictMock.Of<IPrivateAcrModuleMetadataProviderFactory>();

            // Default - when an unrecognized registry is requested, return a provider that fails to load (similar to real behavior)
            privateFactory.Setup(x => x.Create(It.IsAny<CloudConfiguration>(), It.IsAny<string>(), It.IsAny<IContainerRegistryClientFactory>()))
                .Returns((CloudConfiguration _, string registry, IContainerRegistryClientFactory _) => MockFailingPrivateMetadataProvider(registry).Object);

            foreach (var privateProvider in privateProviders)
            {
                privateProvider.Object.Registry.Should().NotBe(PublicRegistry);
                privateFactory.Setup(x => x.Create(It.IsAny<CloudConfiguration>(), It.IsAny<string>(), It.IsAny<IContainerRegistryClientFactory>()))
                    .Returns(privateProvider.Object);
            }

            var indexer = new RegistryIndexer(
                publicProvider.Object,
                privateFactory.Object,
                StrictMock.Of<IContainerRegistryClientFactory>().Object,
                BicepTestConstants.BuiltInOnlyConfigurationManager);

            return indexer;
        }
    }
}
