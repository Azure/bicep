// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ResourceTypeProviderFactoryTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task ProviderNameAndVersionAreUsedAsCacheKeys()
    {
        var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
            TestContext,
            typeof(RegistryProviderTests).Assembly,
            "Files/RegistryProviderTests/HttpProvider");

        var registry = "example.azurecr.io";
        var repositoryPath = $"test/provider";
        var repositoryNames = new[] { "foo", "bar" };

        var (clientFactory, _) = RegistryHelper.CreateMockRegistryClients(repositoryNames.Select(name => (registry, $"{repositoryPath}/{name}")).ToArray());

        var services = new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true, ProviderRegistry: true, DynamicTypeLoadingEnabled: true))
            .WithContainerRegistryClientFactory(clientFactory);

        foreach (var repoName in repositoryNames)
        {
            var indexJsonPath = Path.Combine(outputDirectory, "types", "index.json");
            await RegistryHelper.PublishProviderToRegistryAsync(services.Build(), indexJsonPath, $"br:{registry}/{repositoryPath}/{repoName}:1.2.3");
        }

        var result = await CompilationHelper.RestoreAndCompile(
            services,
            (
                "main.bicep",
                @$"
                provider 'br:example.azurecr.io/test/provider/foo:1.2.3' as foo

                module mod './mod.bicep' = {{
                    name: 'mod'
                    params: {{ }}
                }}
                "
            ),
            (
                "mod.bicep",
                @$"
                provider 'br:example.azurecr.io/test/provider/bar:1.2.3' as foo
                "
            ));

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }
}
