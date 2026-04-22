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
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Oci;
using FluentAssertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.UnitTests.Mock.Registry.Catalog;

public static class RegistryCatalogMocks
{
    private const string PublicRegistry = "mcr.microsoft.com";

    public static Mock<IPublicModuleMetadataProvider> MockPublicMetadataProvider(
        IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<(string version, string? description, string? documentUri)> versions)> modules)
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
            .ReturnsAsync(
            [.. modules
                .Select(m => new RegistryModuleMetadata(
                    PublicRegistry,
                    m.moduleName,
                    new RegistryModuleMetadata.ComputedData(
                        new RegistryMetadataDetails(m.description, m.documentationUri),
                        [.. modules.Single(m2 => m2.moduleName.EqualsOrdinally(m.moduleName))
                            .versions
                            .Select(v => new RegistryModuleVersionMetadata(v.version, IsBicepModule: true, new(v.description, v.documentUri)))
                        ]
                    )
            ))]
        );

        return publicProvider;
    }

    public static Mock<IRegistryModuleMetadataProvider> MockPrivateMetadataProvider(
        string registry,
        IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<RepoTagDescriptor> versions)> modules
    )
    {
        var privateProvider = StrictMock.Of<IRegistryModuleMetadataProvider>();

        privateProvider.Setup(x => x.Registry).Returns(registry);

        privateProvider.Setup(x => x.TryGetModulesAsync())
            .ReturnsAsync([..
                modules.Select(m => new RegistryModuleMetadata(
                    registry,
                    m.moduleName,
                    getDataAsyncFunc: async () =>
                        new RegistryModuleMetadata.ComputedData(
                            await DelayedValue(new RegistryMetadataDetails(m.description, m.documentationUri)),
                            [.. modules.Single(m2 => m2.moduleName.EqualsOrdinally(m.moduleName))
                                .versions
                                    .Select(v => new RegistryModuleVersionMetadata(v.Tag, true, new(v.Description, v.DocumentationUri))
                            )]
                        )
                ))
            ]);

        return privateProvider;
    }

    private static async Task<T> DelayedValue<T>(T value)
    {
        await Task.Delay(1);
        return value;
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

    public static IRegistryModuleCatalog CreateCatalogWithMocks(
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
        privateFactory.Setup(x => x.Create(It.IsAny<CloudConfiguration>(), It.IsAny<string>(), It.IsAny<IOciRegistryTransportFactory>()))
            .Returns((CloudConfiguration _, string registry, IOciRegistryTransportFactory _) =>
                MockFailingPrivateMetadataProvider(registry, new Exception($"Registry {registry} not found in mock")).Object);

        foreach (var privateProvider in privateProviders)
        {
            privateProvider.Object.Registry.Should().NotBe(PublicRegistry);
            privateFactory.Setup(x => x.Create(It.IsAny<CloudConfiguration>(), privateProvider.Object.Registry, It.IsAny<IOciRegistryTransportFactory>()))
                .Returns(privateProvider.Object);
        }

        var indexer = new RegistryModuleCatalog(
            publicProvider.Object,
            privateFactory.Object,
            StrictMock.Of<IOciRegistryTransportFactory>().Object,
            BicepTestConstants.BuiltInOnlyConfigurationManager);

        return indexer;
    }

    public static ModuleAliasesConfiguration ModuleAliases(
        string moduleAliasesJson
    )
    {
        return ModuleAliasesConfiguration.Bind(JsonElementFactory.CreateElement(moduleAliasesJson), null);
    }
}
