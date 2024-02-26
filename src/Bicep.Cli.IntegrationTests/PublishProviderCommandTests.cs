// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class PublishProviderCommandTests : TestBase
{
    [TestMethod]
    public async Task Publish_provider_should_succeed()
    {
        var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
            TestContext,
            typeof(PublishProviderCommandTests).Assembly,
            "Files/PublishProviderCommandTests/TestProvider");

        var registryStr = "example.com";
        var registryUri = new Uri($"https://{registryStr}");
        var repository = $"test/provider";
        var version = "0.0.1";

        var (clientFactory, blobClientMocks) = RegistryHelper.CreateMockRegistryClients((registryStr, repository));
        var mockBlobClient = blobClientMocks[(registryUri, repository)];

        var indexPath = Path.Combine(outputDirectory, "index.json");
        var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

        List<string> requiredArgs = new() { "publish-provider", indexPath, "--target", $"br:{registryStr}/{repository}:{version}" };

        string[] args = requiredArgs.ToArray();

        var result = await Bicep(settings, args);
        result.Should().Succeed().And.NotHaveStdout();

        // this command should output an experimental warning
        result.Stderr.Should().Match("The 'publish-provider' CLI command group is an experimental feature.*");

        // verify the provider was published
        mockBlobClient.Should().HaveProvider(version, out var tgzStream);

        var typeLoader = OciTypeLoader.FromStream(tgzStream);
        var azTypeLoader = new AzResourceTypeLoader(typeLoader);

        // verify the index works
        var saTypeReference = azTypeLoader.GetAvailableTypes().Should().Contain(x => x.Name == "Microsoft.Storage/storageAccounts@2022-05-01").Subject;
        var saType = azTypeLoader.LoadType(saTypeReference);

        // verify we can load a type
        var saBodyType = (ObjectType)saType.Body.Type;
        saBodyType.Properties.Keys.Should().Contain("name", "location", "properties", "sku", "tags");

        // publishing without --force should fail
        result = await Bicep(settings, requiredArgs.ToArray());
        result.Should().Fail().And.HaveStderrMatch("*The Provider \"*\" already exists in registry. Use --force to overwrite the existing provider.*");

        // test with force
        requiredArgs.Add("--force");

        var result2 = await Bicep(settings, requiredArgs.ToArray());
        result2.Should().Succeed().And.NotHaveStdout();

        // verify the provider was published
        mockBlobClient.Should().HaveProvider(version, out var tgzStream2);

        var typeLoader2 = OciTypeLoader.FromStream(tgzStream2);
        var azTypeLoader2 = new AzResourceTypeLoader(typeLoader2);

        // verify the index works
        var saTypeReference2 = azTypeLoader2.GetAvailableTypes().Should().Contain(x => x.Name == "Microsoft.Storage/storageAccounts@2022-05-01").Subject;
        var saType2 = azTypeLoader2.LoadType(saTypeReference2);

        // verify we can load a type
        var saBodyType2 = (ObjectType)saType2.Body.Type;
        saBodyType2.Properties.Keys.Should().Contain("name", "location", "properties", "sku", "tags");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_malformed_target()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = Path.Combine(outputDirectory, "index.json");

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"asdf:123");
        result.Should().Fail().And.HaveStderrMatch("*The specified module reference scheme \"asdf\" is not recognized.*");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_missing_index_path()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = Path.Combine(outputDirectory, "index.json");

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"br:example.com/test/provider:0.0.1");
        result.Should().Fail().And.HaveStderrMatch("*Provider package creation failed: Could not find a part of the path '*'.*");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_malformed_index()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = FileHelper.SaveResultFile(TestContext, "index.json", "malformed", outputDirectory);

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"br:example.com/test/provider:0.0.1");
        result.Should().Fail().And.HaveStderrMatch("*Provider package creation failed: 'm' is an invalid start of a value.*");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_missing_referenced_types_json()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = FileHelper.SaveResultFile(TestContext, "index.json", """
{
  "resources": {
    "Microsoft.Storage/storageAccounts@2022-05-01": {
      "$ref": "types.json#/179"
    }
  },
  "resourceFunctions": {}
}
""", outputDirectory);

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"br:example.com/test/provider:0.0.1");
        result.Should().Fail().And.HaveStderrMatch("*Provider package creation failed: Could not find file '*types.json'.*");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_malformed_types_json()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = FileHelper.SaveResultFile(TestContext, "index.json", """
{
  "resources": {
    "Microsoft.Storage/storageAccounts@2022-05-01": {
      "$ref": "types.json#/179"
    }
  },
  "resourceFunctions": {}
}
""", outputDirectory);
        FileHelper.SaveResultFile(TestContext, "types.json", "malformed", outputDirectory);

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"br:example.com/test/provider:0.0.1");
        result.Should().Fail().And.HaveStderrMatch("*Provider package creation failed: 'm' is an invalid start of a value.*");
    }

    [TestMethod]
    public async Task Publish_provider_should_fail_for_bad_type_location()
    {
        var outputDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
        var indexPath = FileHelper.SaveResultFile(TestContext, "index.json", """
{
  "resources": {
    "Microsoft.Storage/storageAccounts@2022-05-01": {
      "$ref": "types.json#/179"
    }
  },
  "resourceFunctions": {}
}
""", outputDirectory);
        FileHelper.SaveResultFile(TestContext, "types.json", """
[
  {
    "$type": "StringType",
    "minLength": 3,
    "maxLength": 24
  },
  {
    "$type": "StringLiteralType",
    "value": "Microsoft.Storage/storageAccounts"
  }
]
""", outputDirectory);

        var result = await Bicep(CreateDefaultSettings(), "publish-provider", indexPath, "--target", $"br:example.com/test/provider:0.0.1");
        result.Should().Fail().And.HaveStderrMatch("*Provider package creation failed: Index was outside the bounds of the array.*");
    }
}
