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
            modules.Should().AllSatisfy(
                m => m.moduleName.Should().StartWith("bicep/", "All public modules should start with '/bicep'")
            );

            var publicProvider = StrictMock.Of<IPublicModuleMetadataProvider>();

            publicProvider.Setup(x => x.Registry).Returns(PublicRegistry);

            publicProvider.Setup(x => x.GetModulesAsync())
                .ReturnsAsync([.. modules.Select(m => new RegistryModuleMetadata(PublicRegistry, m.moduleName, m.description, m.documentationUri))]);
            publicProvider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions]);

            // Default to throwing an exception for unrecognized modules
            return publicProvider.WithThrowOnUnrecognizedModules();
        }

        public static Mock<IRegistryModuleMetadataProvider> MockPrivateMetadataProvider(
            string registry,
            IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<RegistryModuleVersionMetadata> versions)> modules
        )
        {
            var privateProvider = StrictMock.Of<IRegistryModuleMetadataProvider>();

            privateProvider.Setup(x => x.Registry).Returns(registry);

            privateProvider.Setup(x => x.GetModulesAsync())
                .ReturnsAsync([.. modules.Select(m => new RegistryModuleMetadata(registry, m.moduleName, m.description, m.documentationUri))]);
            privateProvider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions]);

            // Default to throwing an exception for unrecognized modules
            return privateProvider.WithThrowOnUnrecognizedModules();
        }

        public static Mock<IRegistryIndexer> MockRegistryIndexer(
            Mock<IPublicModuleMetadataProvider>? publicProvider,
            params Mock<IRegistryModuleMetadataProvider>[] privateProviders
        )
        {
            var indexer = StrictMock.Of<IRegistryIndexer>();

            foreach (var privateProvider in privateProviders)
            {
                privateProvider.Object.Registry.Should().NotBe(PublicRegistry);
                indexer.Setup(x => x.GetRegistry(privateProvider.Object.Registry, It.IsAny<CloudConfiguration>())).Returns(privateProvider.Object);
            }

            if (publicProvider is { })
            {
                indexer.Setup(x => x.GetRegistry(PublicRegistry, It.IsAny<CloudConfiguration>()))
                    .Returns(publicProvider.Object);
            }
            else {
                indexer.Setup(x => x.GetRegistry(PublicRegistry, It.IsAny<CloudConfiguration>()))
                    .Throws(new InvalidOperationException("No public provider mock was found"));
            }

            // Default to throwing an exception for unrecognized registries
            indexer.Setup(x => x.GetRegistry(It.IsAny<string>(), It.IsAny<CloudConfiguration>()))
                .Returns((string moduleName, CloudConfiguration cloud) => throw new InvalidOperationException($"Didn't mock module {moduleName}"));

            return indexer;
        }

        public static Mock<T> WithReturnEmptyForUnrecognizedModules<T>(this Mock<T> provider) where T : class, IRegistryModuleMetadataProvider
        {
            provider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<ImmutableArray<RegistryModuleVersionMetadata>>([]));
            return provider;
        }

        private static Mock<T> WithThrowOnUnrecognizedModules<T>(this Mock<T> provider) where T : class, IRegistryModuleMetadataProvider
        {
            provider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>()))
                .Returns((string moduleName) => throw new InvalidOperationException($"Didn't mock module {moduleName}"));

            return provider;
        }

        // This changes the mock to return a metadata provider that throws an exception when asked for modules we don't know about
        // (This is normal non-mock behavior) asdfg should this be default?
        public static Mock<IRegistryIndexer> WithNormalBehaviorOnUnrecognizedRegistries(this Mock<IRegistryIndexer> indexer)
        {
            indexer.Setup(x => x.GetRegistry(It.IsAny<string>(), It.IsAny<CloudConfiguration>()))
                .Returns((string moduleName, CloudConfiguration _) =>
                {
                    var providerForUnknownRegistry = StrictMock.Of<IRegistryModuleMetadataProvider>();
                    providerForUnknownRegistry.Setup(x => x.Registry).Returns(moduleName);
                    providerForUnknownRegistry.Setup(x => x.GetModulesAsync())
                        .ReturnsAsync(() => throw new InvalidOperationException($"Registry '{moduleName}' not found"));
                    return providerForUnknownRegistry.Object;
                });

            return indexer;
        }
    }
}
