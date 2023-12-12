// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataSet = Bicep.Core.Samples.DataSet;
using Bicep.Core.UnitTests.TypeSystem.Az;
using CommandLine.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using MessagePack.Resolvers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class PublishProviderCommandTests : TestBase
{
    [NotNull]
    public TestContext? TestContext { get; set; }

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

        var (clientFactory, blobClientMocks) = DataSetsExtensions.CreateMockRegistryClients(false, (registryUri, repository));
        var mockBlobClient = blobClientMocks[(registryUri, repository)];

        var indexPath = Path.Combine(outputDirectory, DataSet.TestIndex);
        var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

        List<string> requiredArgs = new() { "publish-provider", indexPath, "--target", $"br:{registryStr}/{repository}:{version}" };
        //publish-provider "C:\bicep\bicep\src\Bicep.Core.Samples\Files\baselines\Publish_Providers\index.json" --target "br:example.azurecr.io/hello/world:v1"

        string[] args = requiredArgs.ToArray();

        var result = await Bicep(settings, args);
        result.Should().Succeed().And.NotHaveStdout();

        // verify the provider was published
        mockBlobClient.Should().HaveProvider(version, out var tgzStream);        

        var typeLoader = OciTypeLoader.FromTgz(tgzStream);
        var azTypeLoader = new AzResourceTypeLoader(typeLoader);

        // verify the index works
        var saTypeReference = azTypeLoader.GetAvailableTypes().Should().Contain(x => x.Name == "Microsoft.Storage/storageAccounts@2022-05-01").Subject;
        var saType = azTypeLoader.LoadType(saTypeReference);

        // verify we can load a type
        var saBodyType = (ObjectType)saType.Body.Type;
        saBodyType.Properties.Keys.Should().Contain("name", "location", "properties", "sku", "tags");

        // TODO: add tests for --force
    }
}