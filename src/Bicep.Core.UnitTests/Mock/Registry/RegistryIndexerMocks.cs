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

            privateProvider.Setup(x => x.TryGetModulesAsync())
                .ReturnsAsync([.. modules.Select(m => new RegistryModuleMetadata(registry, m.moduleName, m.description, m.documentationUri))]);
            privateProvider.Setup(x => x.TryGetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions]);

            // Default to throwing an exception for unrecognized modules
            return privateProvider.WithThrowOnUnrecognizedModules();
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
            Mock<IPublicModuleMetadataProvider>? publicProvider,
            params Mock<IRegistryModuleMetadataProvider>[] privateProviders
        )
        {
            if (publicProvider is null)
            {
                publicProvider = MockPublicMetadataProvider([]);
            }

            var privateFactory = StrictMock.Of<IPrivateAcrModuleMetadataProviderFactory>();

            // Behavior for unrecognized registries
            //indexer.Setup(x => x.GetRegistry(It.IsAny<string>(), It.IsAny<CloudConfiguration>()))
            //    .Returns((string moduleName, CloudConfiguration _) =>
            //    {
            //        var providerForUnknownRegistry = StrictMock.Of<IRegistryModuleMetadataProvider>();
            //        providerForUnknownRegistry.Setup(x => x.Registry).Returns(moduleName);
            //        providerForUnknownRegistry.Setup(x => x.GetModulesAsync())
            //            .ReturnsAsync(() => throw new InvalidOperationException($"Catalog for registry '{moduleName}' was not found"));
            //        return providerForUnknownRegistry.Object;
            //    });

            // Default - create a private provider that fails for any unrecognized registries
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


    //asdfg
//             Default to throwing an exception for unrecognized registries
//indexer.Setup(x => x.GetRegistry(It.IsAny<string>(), It.IsAny<CloudConfiguration>()))
//                .Returns((string moduleName, CloudConfiguration cloud) => throw new InvalidOperationException($"Didn't mock module {moduleName}"));

            return indexer;
        }

        public static Mock<T> WithReturnEmptyForUnrecognizedModules<T>(this Mock<T> provider) where T : class, IRegistryModuleMetadataProvider
        {
            //adfg
            //provider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>()))
            //    .Returns(Task.FromResult<ImmutableArray<RegistryModuleVersionMetadata>>([]));
            return provider;
        }

        private static Mock<T> WithThrowOnUnrecognizedModules<T>(this Mock<T> provider) where T : class, IRegistryModuleMetadataProvider
        {
            //adfg
            //provider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>()))
            //    .Returns((string moduleName) => throw new InvalidOperationException($"Didn't mock module {moduleName}"));

            return provider;
        }
    }
}
