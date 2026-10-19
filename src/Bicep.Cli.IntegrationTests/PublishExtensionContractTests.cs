// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Testing.Mocks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class PublishExtensionContractTests : TestBase
{
    private const string Registry = "example.com";
    private const string RepositoryPath = "test/extension";
    private const string Version = "0.0.1";
    private const string Target = $"br:{Registry}/{RepositoryPath}:{Version}";

    // The media type strings are a published contract, so they are spelled out instead of read from BicepMediaTypes.
    private const string TypesLayerMediaType = "application/vnd.ms.bicep.provider.layer.v1.tar+gzip";
    private const string V1LinuxX64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v1.linux-x64.binary";
    private const string V2LinuxX64BinaryMediaType = "application/vnd.ms.bicep.provider.layer.v2.linux-x64.binary";

    [TestMethod]
    public async Task Publish_extension_with_contract_v2_should_push_v2_binary_layer_and_leave_local_deploy_unset()
    {
        var (settings, blobClient) = CreateRegistrySettings();
        var indexPath = SaveTestExtensionIndex();
        var binaryPath = FileHelper.SaveResultFile(TestContext, "linux-x64.bin", "linux-x64 binary");

        var result = await Bicep(settings, "publish-extension", indexPath, "--contract", "v2", "--bin-linux-x64", binaryPath, "--target", Target);

        result.Should().Succeed().And.NotHaveStdout();
        result.Stderr.Should().Match("WARNING: The 'publish-extension' CLI command group is an experimental feature.*");

        var manifest = GetManifest(blobClient);
        using (new AssertionScope())
        {
            manifest.ArtifactType.Should().Be("application/vnd.ms.bicep.provider.artifact");
            manifest.Config.MediaType.Should().Be("application/vnd.ms.bicep.provider.config.v1+json");
            GetConfig(blobClient, manifest).EnumerateObject().Should().BeEmpty("old clients must not try to launch an HTTP binary over gRPC");
            manifest.Layers.Select(x => x.MediaType).Should().Equal(TypesLayerMediaType, V2LinuxX64BinaryMediaType);
            blobClient.Blobs[manifest.Layers[1].Digest].ToString().Should().Be("linux-x64 binary");
        }
    }

    [TestMethod]
    public async Task Publish_extension_without_contract_should_use_v1_binary_layers()
    {
        var (settings, blobClient) = CreateRegistrySettings();
        var indexPath = SaveTestExtensionIndex();
        var binaryPath = FileHelper.SaveResultFile(TestContext, "linux-x64.bin", "linux-x64 binary");

        var result = await Bicep(settings, "publish-extension", indexPath, "--bin-linux-x64", binaryPath, "--target", Target);

        result.Should().Succeed();

        var manifest = GetManifest(blobClient);
        var config = GetConfig(blobClient, manifest);
        using (new AssertionScope())
        {
            manifest.Layers.Select(x => x.MediaType).Should().Equal(TypesLayerMediaType, V1LinuxX64BinaryMediaType);
            config.GetProperty("localDeployEnabled").GetBoolean().Should().BeTrue();
            config.GetProperty("supportedArchitectures").EnumerateArray().Select(x => x.GetString()).Should().Equal("linux-x64");
        }
    }

    [TestMethod]
    public async Task Publish_extension_with_contract_v2_without_index_file_should_fail_without_launching_the_binary()
    {
        var binaryPath = FileHelper.SaveResultFile(TestContext, "linux-x64.bin", "linux-x64 binary");

        // A strict mock with no setups fails the command if anything tries to start the binary.
        var result = await Bicep(
            services => services.AddSingleton(StrictMock.Of<ILocalExtensionFactory>().Object),
            "publish-extension", "--contract", "v2", "--bin-linux-x64", binaryPath, "--target", Target);

        result.Should().Fail().And.HaveStderrMatch("*An index file and at least one binary must be specified with --contract v2.*");
    }

    [TestMethod]
    public async Task Publish_extension_with_contract_v2_without_binaries_should_fail()
    {
        var indexPath = SaveTestExtensionIndex();

        var result = await Bicep("publish-extension", indexPath, "--contract", "v2", "--target", Target);

        result.Should().Fail().And.HaveStderrMatch("*An index file and at least one binary must be specified with --contract v2.*");
    }

    [TestMethod]
    public async Task Publish_extension_with_contract_v2_to_local_file_should_fail()
    {
        var indexPath = SaveTestExtensionIndex();
        var binaryPath = FileHelper.SaveResultFile(TestContext, "linux-x64.bin", "linux-x64 binary");
        var targetPath = Path.Combine(FileHelper.GetUniqueTestOutputPath(TestContext), "extension.tgz");

        var result = await Bicep("publish-extension", indexPath, "--contract", "v2", "--bin-linux-x64", binaryPath, "--target", targetPath);

        result.Should().Fail().And.HaveStderrMatch("*Only registry (br:) targets are supported with --contract v2.*");
    }

    [DataTestMethod]
    [DataRow("1")]
    [DataRow("2")]
    [DataRow("v3")]
    public async Task Publish_extension_with_unknown_contract_should_fail_without_publishing(string contract)
    {
        var (settings, blobClient) = CreateRegistrySettings();
        var indexPath = SaveTestExtensionIndex();
        var binaryPath = FileHelper.SaveResultFile(TestContext, "linux-x64.bin", "linux-x64 binary");

        var result = await Bicep(settings, "publish-extension", indexPath, "--contract", contract, "--bin-linux-x64", binaryPath, "--target", Target);

        result.Should().Fail().And.HaveStderrMatch($"*Argument '{contract}' not recognized*");
        blobClient.ManifestTags.Should().BeEmpty();
    }

    [DataTestMethod]
    [DataRow("v2", false)]
    [DataRow("v1", true)]
    public async Task Restoring_a_published_extension_should_cache_only_v1_binaries(string contract, bool expectBinary)
    {
        var cacheRoot = FileHelper.GetCacheRootDirectory(TestContext);
        var (settings, _) = CreateRegistrySettings(cacheRoot);
        var indexPath = SaveTestExtensionIndex();
        var architecture = SupportedArchitectures.TryGetCurrent() ?? throw new InvalidOperationException("Unsupported test platform.");
        var binaryPath = FileHelper.SaveResultFile(TestContext, "current.bin", "current binary");

        var publishResult = await Bicep(settings, "publish-extension", indexPath, "--contract", contract, $"--bin-{architecture.Name}", binaryPath, "--target", Target);
        publishResult.Should().Succeed();

        var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", $"extension '{Target}'\n");
        var buildResult = await Bicep(settings, "build", bicepPath);

        buildResult.Should().Succeed();
        var restoredBinaries = Directory.EnumerateFiles(cacheRoot.Uri.GetFilePath(), "extension.bin", SearchOption.AllDirectories);
        restoredBinaries.Should().HaveCount(expectBinary ? 1 : 0);
    }

    [TestMethod]
    public async Task Publishing_a_v2_package_should_not_advertise_local_deploy_even_when_requested()
    {
        var (clientFactory, blobClient) = CreateMockRegistry();
        var services = new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, RegistryEnabled: true))
            .WithContainerRegistryClientFactory(clientFactory);
        var linuxX64 = SupportedArchitectures.All.Single(x => x.Name == "linux-x64");
        var package = new ExtensionPackage(ExtensionResourceTypeHelper.GetTestTypesTgz(), LocalDeployEnabled: true, [new ExtensionBinary(linuxX64, BinaryData.FromString("linux-x64 binary"))])
        {
            BinaryLayerVersion = ExtensionBinaryLayerVersion.V2,
        };

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), Target, package);

        var manifest = GetManifest(blobClient);
        using (new AssertionScope())
        {
            GetConfig(blobClient, manifest).EnumerateObject().Should().BeEmpty("restore only reads v1 binary layers");
            manifest.Layers.Select(x => x.MediaType).Should().Equal(TypesLayerMediaType, V2LinuxX64BinaryMediaType);
        }
    }

    private (IContainerRegistryClientFactory clientFactory, FakeRegistryBlobClient blobClient) CreateMockRegistry()
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor(Registry, RepositoryPath, ["tag"]));
        var blobClient = (FakeRegistryBlobClient)clientFactory.CreateAuthenticatedBlobClient(BicepTestConstants.BuiltInConfiguration.Cloud, new Uri($"https://{Registry}"), RepositoryPath);

        return (clientFactory, blobClient);
    }

    private (InvocationSettings settings, FakeRegistryBlobClient blobClient) CreateRegistrySettings(IDirectoryHandle? cacheRoot = null)
    {
        var (clientFactory, blobClient) = CreateMockRegistry();
        var features = new FeatureProviderOverrides(cacheRoot ?? FileHelper.GetCacheRootDirectory(TestContext), RegistryEnabled: true);
        var settings = new InvocationSettings(features, clientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

        return (settings, blobClient);
    }

    private string SaveTestExtensionIndex()
    {
        var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(
            TestContext,
            typeof(PublishExtensionContractTests).Assembly,
            "Files/PublishExtensionCommandTests/TestExtension");

        return Path.Combine(outputDirectory, "index.json");
    }

    private static OciManifest GetManifest(FakeRegistryBlobClient blobClient)
        => blobClient.ManifestObjects[blobClient.ManifestTags[Version]];

    private static JsonElement GetConfig(FakeRegistryBlobClient blobClient, OciManifest manifest)
        => JsonDocument.Parse(blobClient.Blobs[manifest.Config.Digest].ToString()).RootElement;
}
