// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry.Oci;
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
        var clientFactory = new BicepClientFactory();
        var cliName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        var cliPath = Path.GetFullPath(Path.Combine(typeof(BicepClientTests).Assembly.Location, $"../{cliName}"));

        Bicep = await clientFactory.Initialize(
            new() { ExistingCliPath = cliPath },
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

    public static IEnumerable<object[]> GetArchitectures()
    {
        yield return new object[] { "linux-x64", Architecture.X64, "linux" };
        yield return new object[] { "linux-arm64", Architecture.Arm64, "linux" };
        yield return new object[] { "osx-x64", Architecture.X64, "osx" };
        yield return new object[] { "osx-arm64", Architecture.Arm64, "osx" };
        yield return new object[] { "win-x64", Architecture.X64, "windows" };
        yield return new object[] { "win-arm64", Architecture.Arm64, "windows" };
    }

    [TestMethod]
    [DynamicData(nameof(GetArchitectures))]
    public async Task Download_fetches_and_installs_bicep_cli(string name, Architecture architecture, string osPlatformString)
    {
        var osPlatform = OSPlatform.Create(osPlatformString);
        var exeSuffix = osPlatform == OSPlatform.Windows ? ".exe" : string.Empty;
        var outputDir = FileHelper.GetUniqueTestOutputPath(TestContext);

        MockHttpMessageHandler mockHandler = new();
        mockHandler.When(HttpMethod.Get, "https://downloads.bicep.azure.com/releases/latest")
            .Respond("application/json", """
            {
                "tag_name": "v1.2.3"
            }
            """);

        var randomBytes = Guid.NewGuid().ToByteArray();
        mockHandler.When(HttpMethod.Get, $"https://downloads.bicep.azure.com/v1.2.3/bicep-{name}{exeSuffix}")
            .Respond(req =>
            {
                return new(System.Net.HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(randomBytes),
                };
            });

        var clientFactory = new BicepClientFactory(new(mockHandler));

        var bicepCliPath = await clientFactory.Download(new()
        {
            InstallBasePath = outputDir,
            OsPlatform = osPlatform,
            Architecture = architecture,
        }, TestContext.CancellationTokenSource.Token);

        File.Exists(bicepCliPath).Should().BeTrue();
        var installedContent = await File.ReadAllBytesAsync(bicepCliPath);
        installedContent.Should().BeEquivalentTo(randomBytes);
    }

    [TestMethod]
    public async Task Initialize_validates_version_number_format()
    {
        var clientFactory = new BicepClientFactory();
        await FluentActions.Invoking(() => clientFactory.Initialize(new() { BicepVersion = "v0.1.1" }, default))
            .Should().ThrowAsync<ArgumentException>().WithMessage("Invalid Bicep version format 'v0.1.1'. Expected format: 'x.y.z' where x, y, and z are integers.");
    }

    [DataTestMethod]
    [DataRow("1.2", "Invalid Bicep version format '1.2'. Expected format: 'x.y.z' where x, y, and z are integers.")]
    [DataRow("v1.2.3", "Invalid Bicep version format 'v1.2.3'. Expected format: 'x.y.z' where x, y, and z are integers.")]
    [DataRow("1.2.3.4", "Invalid Bicep version format '1.2.3.4'. Expected format: 'x.y.z' where x, y, and z are integers.")]
    [DataRow("latest", "Invalid Bicep version format 'latest'. Expected format: 'x.y.z' where x, y, and z are integers.")]
    public void Validate_throws_for_invalid_BicepVersion(string version, string expectedMessage)
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { BicepVersion = version }))
            .Should().Throw<ArgumentException>().WithMessage(expectedMessage);
    }

    [DataTestMethod]
    [DataRow("1.2.3")]
    [DataRow("0.0.0")]
    [DataRow("100.200.300")]
    public void Validate_accepts_valid_BicepVersion(string version)
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { BicepVersion = version }))
            .Should().NotThrow();
    }

    [TestMethod]
    public void Validate_throws_when_ExistingCliPath_combined_with_InstallBasePath()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ExistingCliPath = "/some/path", InstallBasePath = "/some/base" }))
            .Should().Throw<ArgumentException>().WithMessage("*ExistingCliPath*InstallBasePath*");
    }

    [TestMethod]
    public void Validate_throws_when_ExistingCliPath_combined_with_BicepVersion()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ExistingCliPath = "/some/path", BicepVersion = "1.0.0" }))
            .Should().Throw<ArgumentException>().WithMessage("*ExistingCliPath*BicepVersion*");
    }

    [TestMethod]
    public void Validate_throws_when_ExistingCliPath_combined_with_OsPlatform()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ExistingCliPath = "/some/path", OsPlatform = OSPlatform.Linux }))
            .Should().Throw<ArgumentException>().WithMessage("*ExistingCliPath*OsPlatform*");
    }

    [TestMethod]
    public void Validate_throws_when_ExistingCliPath_combined_with_Architecture()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ExistingCliPath = "/some/path", Architecture = Architecture.X64 }))
            .Should().Throw<ArgumentException>().WithMessage("*ExistingCliPath*Architecture*");
    }

    [TestMethod]
    public void Validate_throws_when_Stdio_combined_with_ConnectionTimeout()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ConnectionMode = BicepConnectionMode.Stdio, ConnectionTimeout = TimeSpan.FromSeconds(10) }))
            .Should().Throw<ArgumentException>().WithMessage("*ConnectionTimeout*Stdio*");
    }

    [TestMethod]
    public void Validate_accepts_Stdio_without_ConnectionTimeout()
    {
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ConnectionMode = BicepConnectionMode.Stdio }))
            .Should().NotThrow();
    }

    [TestMethod]
    public void Validate_accepts_Stdio_with_ExistingCliPath()
    {
        var existingPath = typeof(BicepClientTests).Assembly.Location;
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ConnectionMode = BicepConnectionMode.Stdio, ExistingCliPath = existingPath }))
            .Should().NotThrow();
    }

    [TestMethod]
    public async Task Initialize_validates_path_existence()
    {
        var nonExistentPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var clientFactory = new BicepClientFactory();
        await FluentActions.Invoking(() => clientFactory.Initialize(new() { ExistingCliPath = nonExistentPath }, default))
            .Should().ThrowAsync<FileNotFoundException>().WithMessage($"The specified Bicep CLI path does not exist: '{nonExistentPath}'.");
    }

    [TestMethod]
    public void Validate_throws_when_ExistingCliPath_does_not_exist()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        FluentActions.Invoking(() => BicepClientConfiguration.Validate(new() { ExistingCliPath = nonExistentPath }))
            .Should().Throw<FileNotFoundException>().WithMessage($"The specified Bicep CLI path does not exist: '{nonExistentPath}'.");
    }

    [TestMethod]
    public async Task Initialize_throws_NotSupportedException_for_unsupported_ConnectionMode()
    {
        var existingPath = typeof(BicepClientTests).Assembly.Location;
        var clientFactory = new BicepClientFactory();
        // Cast an unknown value to exercise the default branch in the factory's switch expression.
        var unknownMode = (BicepConnectionMode)99;
        await FluentActions.Invoking(() => clientFactory.Initialize(new() { ExistingCliPath = existingPath, ConnectionMode = unknownMode }, default))
            .Should().ThrowAsync<NotSupportedException>();
    }

    [TestMethod]
    public async Task WaitForPipeConnection_throws_timeout_exception_when_connection_times_out()
    {
        using var pipeStream = new NamedPipeServerStream(Guid.NewGuid().ToString(), PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        await FluentActions.Invoking(() => BicepClient.WaitForPipeConnection(pipeStream, TimeSpan.FromMilliseconds(500), CancellationToken.None))
            .Should().ThrowAsync<TimeoutException>().WithMessage("Timed out waiting for the Bicep CLI process to connect after * seconds.");
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
    public async Task Compile_runs_successfully_with_stdio()
    {
        var clientFactory = new BicepClientFactory();
        var cliName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        var cliPath = Path.GetFullPath(Path.Combine(typeof(BicepClientTests).Assembly.Location, $"../{cliName}"));

        using var bicep = await clientFactory.Initialize(
            new() { ExistingCliPath = cliPath, ConnectionMode = BicepConnectionMode.Stdio },
            TestContext.CancellationTokenSource.Token);

        var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", """
        param location string
        """);

        var result = await bicep.Compile(new(bicepFile));

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
