// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ResourceTypeProviderFactoryTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task ExtensionNameAndVersionAreUsedAsCacheKeys()
    {
        var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
            TestContext,
            typeof(ExtensionRegistryTests).Assembly,
            "Files/ExtensionRegistryTests/http");

        var registry = "example.azurecr.io";
        var repositoryPath = $"test/extension";
        var repositoryNames = new[] { "foo", "bar" };

        var clientFactory = RegistryHelper.CreateMockRegistryClient([.. repositoryNames.Select(name => new RepoDescriptor(registry, $"{repositoryPath}/{name}", ["v1"]))]);

        var services = new ServiceBuilder()
            .WithContainerRegistryClientFactory(clientFactory);

        foreach (var repoName in repositoryNames)
        {
            var indexJsonPath = Path.Combine(outputDirectory, "types", "index.json");
            await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), indexJsonPath, $"br:{registry}/{repositoryPath}/{repoName}:1.2.3");
        }

        var result = await CompilationHelper.RestoreAndCompile(
            services,
            (
                "main.bicep",
                @$"
                extension 'br:example.azurecr.io/test/extension/foo:1.2.3' as foo

                module mod './mod.bicep' = {{
                    name: 'mod'
                    params: {{ }}
                }}
                "
            ),
            (
                "mod.bicep",
                @$"
                extension 'br:example.azurecr.io/test/extension/bar:1.2.3' as foo
                "
            ));

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }
}
