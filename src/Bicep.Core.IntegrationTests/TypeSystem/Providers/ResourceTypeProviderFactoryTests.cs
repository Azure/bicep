// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Bicep.Core.Samples;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;
using System;
using System.Linq;
using Bicep.Core.UnitTests;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ResourceTypeProviderFactoryTests
{
    [TestMethod]
    // the cache uses the provider name and version as keys for the cache
    public async Task ProviderNameAndVersionAreUsedAsCacheKeys()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(
              typeof(RegistryProviderTests).Assembly,
              "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repositoryPath = $"test/provider";
        var repositoryNames = new[] { "foo", "bar" };

        var (clientFactory, _) = DataSetsExtensions.CreateMockRegistryClients(repositoryNames.Select(name => (registry, $"{repositoryPath}/{name}")).ToArray());

        var services = new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, ProviderRegistry: true))
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);

        foreach (var repoName in new[] { "foo", "bar" })
        {
            await DataSetsExtensions.PublishProviderToRegistryAsync(services.Build(), "/types/index.json", $"br:{registry}/{repositoryPath}/{repoName}:1.2.3");
        }

        var result = await CompilationHelper.RestoreAndCompile(
            services,
            (
                "main.bicep",
                @$"
                provider 'br:example.azurecr.io/test/provider/foo@1.2.3' as foo
                
                module mod './mod.bicep' = {{
                    name: 'mod'
                    params: {{ }}
                }}
                "
            ),
            (
                "mod.bicep",
                @$"
                provider 'br:example.azurecr.io/test/provider/bar@1.2.3' as foo
                "
            ));

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }
}