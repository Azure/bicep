// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.RpcClient.Helpers;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class BicepClientTests
{
    public TestContext TestContext { get; set; } = null!;

    public required IBicepClient Bicep { get; set; }

    [TestInitialize]
    public async Task TestInitialize()
    {
        MockHttpMessageHandler mockHandler = new();

        mockHandler.When(HttpMethod.Get, "https://downloads.bicep.azure.com/releases/latest")
            .Respond("application/json", """
            {
                "tag_name": "v0.0.0-dev"
            }
            """);

        mockHandler.When(HttpMethod.Get, "https://downloads.bicep.azure.com/v0.0.0-dev/bicep-*-*")
            .Respond(req =>
            {
                var cliName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";
                var cliPath = Path.GetFullPath(Path.Combine(typeof(BicepClientTests).Assembly.Location, $"../PublishedCli/{cliName}"));

                return new(System.Net.HttpStatusCode.OK)
                {
                    Content = new StreamContent(File.OpenRead(cliPath))
                };
            });

        var clientFactory = new BicepClientFactory(new(mockHandler));

        Bicep = await clientFactory.DownloadAndInitialize(
            new() { InstallPath = FileHelper.GetUniqueTestOutputPath(TestContext) },
            TestContext.CancellationTokenSource.Token);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Bicep.Dispose();
        if (Directory.Exists(TestContext.TestResultsDirectory))
        {
            // This test creates a new Bicep CLI install for each test run, so we need to clean it up afterwards
            Directory.Delete(TestContext.TestResultsDirectory, true);
        }
    }

    [TestMethod]
    public async Task DownloadAndInitialize_validates_version_number_format()
    {
        var clientFactory = new BicepClientFactory(new());
        await FluentActions.Invoking(() => clientFactory.DownloadAndInitialize(new() { BicepVersion = "v0.1.1" }, default))
            .Should().ThrowAsync<ArgumentException>().WithMessage("Invalid Bicep version format 'v0.1.1'. Expected format: 'x.y.z' where x, y, and z are integers.");
    }

    [TestMethod]
    public async Task DownloadAndInitialize_validates_path_existence()
    {
        var nonExistentPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var clientFactory = new BicepClientFactory(new());
        await FluentActions.Invoking(() => clientFactory.InitializeFromPath(nonExistentPath, default))
            .Should().ThrowAsync<FileNotFoundException>().WithMessage($"The specified Bicep CLI path does not exist: '{nonExistentPath}'.");
    }

    [TestMethod]
    public void BuildDownloadUrlForTag_returns_correct_url()
    {
        BicepInstaller.BuildDownloadUrlForTag(OSPlatform.Linux, Architecture.X64, "v0.24.24")
            .Should().Be("https://downloads.bicep.azure.com/v0.24.24/bicep-linux-x64");
    }

    [TestMethod]
    public async Task GetVersion_runs_successfully()
    {
        var result = await Bicep.GetVersion();

        result.Should().MatchRegex(@"^\d+\.\d+\.\d+(-.+)?$");
    }

    [TestMethod]
    public async Task Compile_runs_successfully()
    {
        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", """
        param location string
        """);

        var result = await Bicep.Compile(new(bicepFile));

        result.Success.Should().BeTrue();
        result.Contents.Should().NotBeNullOrEmpty();
        result.Diagnostics.Should().Contain(x => x.Code == "no-unused-params");
    }

    [TestMethod]
    public async Task CompileParams_runs_successfully()
    {
        var outputPath = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
            param location string
            """),
            new("main.bicepparam", """
            using 'main.bicep'

            param location = 'westus'
            """),
        ]);

        var result = await Bicep.CompileParams(new(Path.Combine(outputPath, "main.bicepparam"), []));

        result.Success.Should().BeTrue();
        result.Parameters.Should().NotBeNullOrEmpty();
        result.Template.Should().NotBeNullOrEmpty();
        result.Diagnostics.Should().Contain(x => x.Code == "no-unused-params");
    }

    [TestMethod]
    public async Task Format_runs_successfully()
    {
        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", """
        param location      string
        """);

        var result = await Bicep.Format(new(bicepFile));

        result.Contents.Should().Be("""
        param location string

        """);
    }

    [TestMethod]
    public async Task GetSnapshot_runs_successfully()
    {
        var outputPath = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
            param sku string
            
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
              name: 'myStgAct'
              location: resourceGroup().location
              kind: 'StorageV2'
              sku: {
                name: sku
              }
            }
            """),
            new("main.bicepparam", """
            using 'main.bicep'
            
            param sku = 'Premium_LRS'
            """),
        ]);

        var result = await Bicep.GetSnapshot(new(Path.Combine(outputPath, "main.bicepparam"), new(
            TenantId: null,
            SubscriptionId: "0910bc80-1614-479b-a3f4-07178d3ea77b",
            ResourceGroup: "ant-test",
            Location: "West US",
            DeploymentName: "main"), []));

        result.Snapshot.Should().Contain("/subscriptions/0910bc80-1614-479b-a3f4-07178d3ea77b/resourceGroups/ant-test/providers/Microsoft.Storage/storageAccounts/myStgAct");
    }

    [TestMethod]
    public async Task GetMetadataResponse_runs_successfully()
    {
        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            @export()
            @description('A foo object')
            type foo = {
              bar: string
            }
            """);

        var result = await Bicep.GetMetadata(new(bicepFile));

        result.Exports[0].Description.Should().Be("A foo object");
    }
}
