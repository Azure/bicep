// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Cli.Rpc;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Json;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class JsonRpcCommandTests : TestBase
{
    private async Task RunServerTest(Action<IServiceCollection> registerAction, Func<ICliJsonRpcProtocol, CancellationToken, Task> testFunc)
    {
        var pipeName = Guid.NewGuid().ToString();
        using var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var result = await Bicep(registerAction, cts.Token, "jsonrpc", "--pipe", pipeName);
                result.ExitCode.Should().Be(0);
                result.Stderr.Should().Be("");
                result.Stdout.Should().Be("");
            }),
            Task.Run(async () =>
            {
                try
                {
                    await pipeStream.WaitForConnectionAsync(cts.Token);
                    var client = JsonRpc.Attach<ICliJsonRpcProtocol>(CliJsonRpcServer.CreateMessageHandler(pipeStream, pipeStream));
                    await testFunc(client, cts.Token);
                }
                finally
                {
                    await cts.CancelAsync();
                }
            }, cts.Token));
    }

    [TestMethod]
    public async Task Version_returns_bicep_version()
    {
        await RunServerTest(
            services => { },
            async (client, token) =>
            {
                var response = await client.Version(new(), token);
                response.Version.Should().Be(ThisAssembly.AssemblyInformationalVersion.Split('+')[0]);
            });
    }

    [TestMethod]
    public async Task Compile_returns_a_compilation_result()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicepparam"] = """
using 'main.bicep'

param foo = 'foo'
""",
            ["/main.bicep"] = """
param foo string
""",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.Compile(new("/main.bicep"), token);
                response.Contents.FromJson<JToken>().Should().HaveValueAtPath("$['$schema']", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");
                response.Contents.FromJson<JToken>().Should().HaveJsonAtPath("$.parameters['foo']", """
                {
                  "type": "string"
                }
                """);

                response = await client.Compile(new("/main.bicepparam"), token);
                response.Contents.FromJson<JToken>().Should().HaveValueAtPath("$['$schema']", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
                response.Contents.FromJson<JToken>().Should().HaveJsonAtPath("$.parameters['foo']", """
                {
                  "value": "foo"
                }
                """);
            });
    }

    [TestMethod]
    public async Task GetMetadata_returns_file_metadata()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = """
metadata description = 'my file'

@description('foo param')
param foo string

param inlineType {
  sdf: string
}

param declaredType asdf

@export()
@description('asdf type')
type asdf = {
  foo: string
}

@description('bar output')
output bar string = foo
""",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetMetadata(new("/main.bicep"), token);
                response.Metadata.Should().Equal([
                    new("description", "my file"),
                ]);
                response.Parameters.Should().Equal([
                    new(new(new(2, 0), new(3, 16)), "foo", new(null, "string"), "foo param"),
                    new(new(new(5, 0), new(7, 1)), "inlineType", new(null, "{ sdf: string }"), null),
                    new(new(new(9, 0), new(9, 23)), "declaredType", new(new(new(11, 0), new(15, 1)), "asdf"), null),
                ]);
                response.Outputs.Should().Equal([
                    new(new(new(17, 0), new(18, 23)), "bar", new(null, "string"), "bar output"),
                ]);
                response.Exports.Should().Equal([
                    new(new(new(11, 0), new(15, 1)), "asdf", "TypeAlias", "asdf type"),
                ]);
            });
    }

    [TestMethod]
    public async Task OutputDocs_returns_rendered_documentation()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = """
                metadata name = 'RPC Module'
                metadata description = 'Rendered through JSON-RPC.'

                @description('Example value.')
                param value string = 'default'
                """,
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.OutputDocs(
                    new("/main.bicep", null, null, null, NoRestore: false),
                    token);

                response.Result.Success.Should().BeTrue();
                response.Result.Path.Should().Be(fileSystem.Path.GetFullPath("/main.bicep"));
                response.Result.OutputPath.Should().BeNull();
                response.Result.Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Level == "Warning" &&
                    diagnostic.Code == "no-unused-params");
                response.Result.Contents.Should().ContainAll("# RPC Module", "Rendered through JSON-RPC.", "`value`");
                fileSystem.File.Exists("/README.md").Should().BeFalse();
            });
    }

    [TestMethod]
    public async Task OutputDocs_custom_template_supports_includes_and_custom_values()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'RPC Module'",
            ["/template.scriban"] = "{{ include \"_header.md\" }} {{ module.name }} {{ custom.owner }}",
            ["/_header.md"] = "Header",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.OutputDocs(
                    new(
                        "/main.bicep",
                        "/template.scriban",
                        "/",
                        new() { ["owner"] = "Platform" },
                        NoRestore: true),
                    token);

                response.Result.Success.Should().BeTrue();
                response.Result.Contents.Should().Be("Header RPC Module Platform\n");
            });
    }

    [TestMethod]
    public async Task Docs_methods_apply_configuration_and_request_overrides()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'RPC Config'",
            ["/module/examples/default/main.bicep"] = "metadata name = 'ignored'",
            ["/template.scriban"] = "{{ module.name }}|{{ custom.owner }}|{{ module.usageExamples.size }}",
            ["/docs.json"] = """
                {
                  "input": {
                    "include": ["main.bicep"]
                  },
                  "output": {
                    "file": "RPC.md"
                  },
                  "template": {
                    "file": "template.scriban",
                    "values": {
                      "owner": "Config"
                    }
                  },
                  "examples": {
                    "sources": []
                  }
                }
                """,
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var output = await client.OutputDocs(
                    new(
                        "/module",
                        null,
                        null,
                        new() { ["owner"] = "Request" },
                        NoRestore: false)
                    {
                        ConfigFilePath = "/docs.json",
                    },
                    token);
                var generated = await client.GenerateDocs(
                    new(
                        ["/module"],
                        null,
                        null,
                        new() { ["owner"] = "Request" },
                        null,
                        NoRestore: false)
                    {
                        ConfigFilePath = "/docs.json",
                    },
                    token);

                output.Result.Success.Should().BeTrue();
                output.Result.Contents.Should().Be("RPC Config|Request|0\n");
                generated.Results.Should().ContainSingle();
                generated.Results[0].Success.Should().BeTrue();
                generated.Results[0].OutputPath.Should().Be(fileSystem.Path.GetFullPath("/module/RPC.md"));
                fileSystem.File.ReadAllText("/module/RPC.md").Should().Be(output.Result.Contents);
            });
    }

    [TestMethod]
    public async Task Docs_methods_do_not_auto_discover_configuration()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'RPC defaults'",
            ["/module/bicepdocsconfig.json"] = "{ invalid",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var output = await client.OutputDocs(
                    new("/module", null, null, null, NoRestore: false),
                    token);
                var generated = await client.GenerateDocs(
                    new(["/module"], null, null, null, null, NoRestore: false),
                    token);

                output.Result.Success.Should().BeTrue();
                output.Result.Contents.Should().Contain("# RPC defaults");
                generated.Results.Should().ContainSingle(result =>
                    result.Success &&
                    result.OutputPath == fileSystem.Path.GetFullPath("/module/README.md"));
            });
    }

    [TestMethod]
    public async Task GenerateDocs_writes_successful_modules_and_continues_failures()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/valid/main.bicep"] = "metadata name = 'Valid'",
            ["/invalid/main.bicep"] = "param value invalidType",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GenerateDocs(
                    new(
                        ["/valid/main.bicep", "/invalid/main.bicep"],
                        null,
                        null,
                        null,
                        null,
                        NoRestore: false),
                    token);

                response.Results.Should().HaveCount(2);
                response.Results[0].Success.Should().BeTrue();
                response.Results[0].OutputPath.Should().Be(fileSystem.Path.GetFullPath("/valid/README.md"));
                response.Results[0].Contents.Should().Be(fileSystem.File.ReadAllText("/valid/README.md"));
                response.Results[1].Success.Should().BeFalse();
                response.Results[1].Contents.Should().BeNull();
                response.Results[1].Diagnostics.Should().Contain(diagnostic => diagnostic.Level == "Error");
                fileSystem.File.Exists("/invalid/README.md").Should().BeFalse();
            });
    }

    [TestMethod]
    public async Task Docs_methods_return_structured_failures()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'Disabled'",
            ["/main.txt"] = "not bicep",
            ["/invalid.scriban"] = "{{ if module.name }}",
            ["/a.bicep"] = "metadata name = 'A'",
            ["/b.bicep"] = "metadata name = 'B'",
            ["/nonbicep.json"] = """{ "input": { "include": ["main.txt"] } }""",
            ["/multiple.json"] = """{ "input": { "include": ["*.bicep"] } }""",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var invalidExtension = await client.OutputDocs(
                    new("/main.txt", null, null, null, NoRestore: false),
                    token);
                invalidExtension.Result.Success.Should().BeFalse();
                invalidExtension.Result.Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var configuredInvalidExtension = await client.OutputDocs(
                    new("/", null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/nonbicep.json",
                    },
                    token);
                configuredInvalidExtension.Result.Success.Should().BeFalse();
                configuredInvalidExtension.Result.Diagnostics.Should().ContainSingle();
                configuredInvalidExtension.Result.Diagnostics.Single().Code.Should().Be("DOCS001");
                configuredInvalidExtension.Result.Diagnostics.Single().Message.Should().Contain("Invalid Bicep file path");
                var configuredInvalidGenerateExtension = await client.GenerateDocs(
                    new(["/"], null, null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/nonbicep.json",
                    },
                    token);
                configuredInvalidGenerateExtension.Results.Should().ContainSingle(result =>
                    !result.Success &&
                    result.Diagnostics.Any(diagnostic =>
                        diagnostic.Code == "DOCS001" &&
                        diagnostic.Message.Contains("Invalid Bicep file path")));

                var configuredMultipleOutput = await client.OutputDocs(
                    new("/", null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/multiple.json",
                    },
                    token);
                configuredMultipleOutput.Result.Success.Should().BeFalse();
                configuredMultipleOutput.Result.Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message.Contains("requires each path to select exactly one input file"));
                var configuredMultipleGenerate = await client.GenerateDocs(
                    new(["/"], null, null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/multiple.json",
                    },
                    token);
                configuredMultipleGenerate.Results.Should().ContainSingle(result =>
                    !result.Success &&
                    result.Diagnostics.Any(diagnostic =>
                        diagnostic.Code == "DOCS001" &&
                        diagnostic.Message.Contains("requires each path to select exactly one input file")));

                var invalidPath = await client.OutputDocs(
                    new("invalid\0path", null, null, null, NoRestore: false),
                    token);
                invalidPath.Result.Success.Should().BeFalse();
                invalidPath.Result.Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var invalidGeneratePath = await client.GenerateDocs(
                    new(["invalid\0path"], null, null, null, null, NoRestore: false),
                    token);
                invalidGeneratePath.Results.Should().ContainSingle();
                invalidGeneratePath.Results[0].Success.Should().BeFalse();

                var missingGeneratePath = await client.GenerateDocs(
                    new(["/missing.bicep"], null, null, null, null, NoRestore: false),
                    token);
                missingGeneratePath.Results.Should().ContainSingle();
                missingGeneratePath.Results[0].Success.Should().BeFalse();

                var missingPath = await client.OutputDocs(
                    new("/missing", null, null, null, NoRestore: false),
                    token);
                missingPath.Result.Success.Should().BeFalse();
                missingPath.Result.Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var invalidTemplate = await client.OutputDocs(
                    new("/main.bicep", "/invalid.scriban", null, null, NoRestore: false),
                    token);
                invalidTemplate.Result.Success.Should().BeFalse();
                invalidTemplate.Result.Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS003");

                var missingConfig = await client.OutputDocs(
                    new("/main.bicep", null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/missing.json",
                    },
                    token);
                missingConfig.Result.Success.Should().BeFalse();
                missingConfig.Result.Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message.Contains("does not exist"));
                var missingGenerateConfig = await client.GenerateDocs(
                    new(["/main.bicep", "/a.bicep"], null, null, null, null, NoRestore: false)
                    {
                        ConfigFilePath = "/missing.json",
                    },
                    token);
                missingGenerateConfig.Results.Should().HaveCount(2);
                missingGenerateConfig.Results.Should().OnlyContain(result =>
                    !result.Success &&
                    result.Diagnostics.Any(diagnostic => diagnostic.Code == "DOCS001"));

                var missingTemplateRoot = await client.OutputDocs(
                    new("/main.bicep", null, "/missing", null, NoRestore: false),
                    token);
                missingTemplateRoot.Result.Success.Should().BeFalse();
                missingTemplateRoot.Result.Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var inputOverwrite = await client.GenerateDocs(
                    new(["/main.bicep"], null, null, null, "main.bicep", NoRestore: false),
                    token);
                inputOverwrite.Results.Should().ContainSingle();
                inputOverwrite.Results[0].Success.Should().BeFalse();

                var sourceExtension = await client.GenerateDocs(
                    new(["/main.bicep"], null, null, null, "child.bicep", NoRestore: false),
                    token);
                sourceExtension.Results.Should().ContainSingle();
                sourceExtension.Results[0].Success.Should().BeFalse();

                foreach (var invalidOutputFile in new[] { "", " ", ".", "..", "../README.md", @"..\README.md", "bad?.md", "README.md.", "CON.md" })
                {
                    var invalidOutput = await client.GenerateDocs(
                        new(["/main.bicep"], null, null, null, invalidOutputFile, NoRestore: false),
                        token);
                    invalidOutput.Results.Should().ContainSingle();
                    invalidOutput.Results[0].Success.Should().BeFalse();
                    invalidOutput.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                        diagnostic.Code == "DOCS001" &&
                        diagnostic.Message.Contains("must be a file name"));
                }

                var outputCollision = await client.GenerateDocs(
                    new(["/a.bicep", "/b.bicep"], null, null, null, null, NoRestore: false),
                    token);
                outputCollision.Results.Should().HaveCount(2);
                outputCollision.Results[0].Success.Should().BeTrue();
                outputCollision.Results[1].Success.Should().BeFalse();
                outputCollision.Results[1].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message.Contains("resolve to the output file"));

                var mixedResult = await client.GenerateDocs(
                    new(["/missing", "/main.bicep"], null, null, null, null, NoRestore: false),
                    token);
                mixedResult.Results.Should().HaveCount(2);
                mixedResult.Results[0].Success.Should().BeFalse();
                mixedResult.Results[1].Success.Should().BeTrue();
            });
    }

    [TestMethod]
    public async Task GenerateDocs_rejects_windows_aliased_and_reserved_output_paths()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var root = FileHelper.SaveResultFiles(
            TestContext,
            [new("main.bicep", "metadata name = 'Safe'")]);
        var mainFile = Path.Combine(root, "main.bicep");

        await RunServerTest(
            services => { },
            async (client, token) =>
            {
                var aliasedOutput = await client.GenerateDocs(
                    new([mainFile], null, null, null, "main.bicep.", NoRestore: false),
                    token);
                aliasedOutput.Results.Should().ContainSingle();
                aliasedOutput.Results[0].Success.Should().BeFalse();
                File.ReadAllText(mainFile).Should().Contain("metadata name");

                var reservedOutput = await client.GenerateDocs(
                    new([mainFile], null, null, null, "CON.md", NoRestore: false),
                    token);
                reservedOutput.Results.Should().ContainSingle();
                reservedOutput.Results[0].Success.Should().BeFalse();
                reservedOutput.Results[0].Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");
            });
    }

    [TestMethod]
    public async Task GenerateDocs_returns_structured_write_failures()
    {
        var root = FileHelper.SaveResultFiles(
            TestContext,
            [
                new("main.bicep", "metadata name = 'Example'"),
                new("README.md", "preserve me"),
            ]);
        var outputFile = Path.Combine(root, "README.md");
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var fileExplorer = new WriteFailingFileExplorer(
            new FileSystemFileExplorer(fileSystem),
            "README.md",
            new IOException("write failed"));

        await RunServerTest(
            services => services
                .WithFileSystem(fileSystem)
                .WithFileExplorer(fileExplorer),
            async (client, token) =>
            {
                var response = await client.GenerateDocs(
                    new([Path.Combine(root, "main.bicep")], null, null, null, null, NoRestore: false),
                    token);

                response.Results.Should().ContainSingle();
                response.Results[0].Success.Should().BeFalse();
                response.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS002" &&
                    diagnostic.Message == "write failed");
            });

        File.ReadAllText(outputFile).Should().Be("preserve me");
    }

    [TestMethod]
    public async Task OutputDocs_returns_structured_compilation_exceptions()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'Example'",
        });
        var innerExplorer = new FileSystemFileExplorer(fileSystem);
        var mainFile = IOUri.FromFilePath(fileSystem.Path.GetFullPath("/main.bicep"));
        var explorer = new Mock<IFileExplorer>(MockBehavior.Strict);
        explorer
            .Setup(fileExplorer => fileExplorer.GetDirectory(It.IsAny<IOUri>()))
            .Returns((IOUri uri) => innerExplorer.GetDirectory(uri));
        explorer
            .Setup(fileExplorer => fileExplorer.GetFile(It.IsAny<IOUri>()))
            .Returns((IOUri uri) => uri.Equals(mainFile)
                ? throw new BicepException("compilation failed")
                : innerExplorer.GetFile(uri));

        await RunServerTest(
            services => services
                .WithFileSystem(fileSystem)
                .WithFileExplorer(explorer.Object),
            async (client, token) =>
            {
                var response = await client.OutputDocs(
                    new("/main.bicep", null, null, null, NoRestore: false),
                    token);

                response.Result.Success.Should().BeFalse();
                response.Result.Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message == "compilation failed");
            });
    }

    [TestMethod]
    public async Task Docs_methods_return_structured_path_exceptions()
    {
        var fileSystem = new Mock<System.IO.Abstractions.IFileSystem>(MockBehavior.Strict);
        var path = new Mock<System.IO.Abstractions.IPath>(MockBehavior.Strict);
        fileSystem.SetupGet(system => system.Path).Returns(path.Object);
        path.Setup(systemPath => systemPath.GetFullPath("invalid")).Throws(new ArgumentException("invalid path"));

        await RunServerTest(
            services => services.WithFileSystem(fileSystem.Object),
            async (client, token) =>
            {
                var output = await client.OutputDocs(
                    new("invalid", null, null, null, NoRestore: false),
                    token);
                var generate = await client.GenerateDocs(
                    new(["invalid"], null, null, null, null, NoRestore: false),
                    token);

                output.Result.Success.Should().BeFalse();
                output.Result.Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message == "invalid path");
                generate.Results.Should().ContainSingle();
                generate.Results[0].Success.Should().BeFalse();
                generate.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message == "invalid path");
            });
    }

    [TestMethod]
    public async Task GetDeploymentGraph_returns_deployment_graph()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = """
resource foo 'My.Rp/foo@2020-01-01' = {
  name: 'foo'
}

resource bar 'My.Rp/foo@2020-01-01' existing = {
  name: 'bar'
  dependsOn: [foo]
}

resource baz 'My.Rp/foo@2020-01-01' = {
  name: 'baz'
  dependsOn: [bar]
}
""",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetDeploymentGraph(new("/main.bicep"), token);
                response.Nodes.Should().Equal([
                    new(new(new(4, 0), new(7, 1)), "bar", "My.Rp/foo", true, null),
                    new(new(new(9, 0), new(12, 1)), "baz", "My.Rp/foo", false, null),
                    new(new(new(0, 0), new(2, 1)), "foo", "My.Rp/foo", false, null),
                ]);
                response.Edges.Should().Equal([
                    new("bar", "foo"),
                    new("baz", "bar"),
                ]);
            });
    }

    [TestMethod]
    public async Task GetFileReferences_returns_all_referenced_files()
    {
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                ["/main.bicepparam"] = """
                    using 'main.bicep'

                    param foo = 'foo'
                    """,
                ["/main.bicep"] = """
                    param foo string

                    var test = loadTextContent('invalid.txt')
                    var test2 = loadTextContent('valid.txt')
                    """,
                ["/valid.txt"] = """
                    hello!
                    """,
                ["/bicepconfig.json"] = """
                    {}
                    """,
            });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetFileReferences(new("/main.bicepparam"), token);
                var expectedFilePaths = new[]
                    {
                        "/bicepconfig.json",
                        "/invalid.txt",
                        "/main.bicep",
                        "/main.bicepparam",
                        "/valid.txt",
                    }.Select(fileSystem.Path.GetFullPath);

                response.FilePaths.Should().BeEquivalentTo(expectedFilePaths);
            });
    }

    [TestMethod]
    public async Task CompileParams_returns_a_compilation_result()
    {
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                ["/main.bicepparam"] = """
                    using './main.bicep'

                    param location = externalInput('custom.binding', '__MY_REGION__')
                    param storageAccountType = externalInput('custom.binding', '__UNRESOLVED_BINDING__')
                    """,
                ["/main.bicep"] = """
                    @description('Storage Account type')
                    param storageAccountType string = 'Standard_LRS'
                    
                    @description('The storage account location.')
                    param location string = resourceGroup().location
                    
                    @description('The name of the storage account')
                    param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'
                    
                    resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                      name: storageAccountName
                      location: location
                      sku: {
                        name: storageAccountType
                      }
                      kind: 'StorageV2'
                      properties: {}
                    }
                    """,
            });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.CompileParams(new("/main.bicepparam", []), token);

                response.Parameters.FromJson<JToken>().Should().HaveValueAtPath("$.parameters['location'].expression", "[externalInputs('custom_binding_0')]");
                response.Parameters.FromJson<JToken>().Should().HaveValueAtPath("$.parameters['storageAccountType'].expression", "[externalInputs('custom_binding_1')]");

                response.Parameters.FromJson<JToken>().Should().HaveJsonAtPath("$.externalInputDefinitions['custom_binding_0']", """
                {
                  "kind": "custom.binding",
                  "config": "__MY_REGION__"
                }
                """);

                response.Parameters.FromJson<JToken>().Should().HaveJsonAtPath("$.externalInputDefinitions['custom_binding_1']", """
                {
                  "kind": "custom.binding",
                  "config": "__UNRESOLVED_BINDING__"
                }
                """);

                response.Template.FromJson<JToken>().Should().HaveValueAtPath("$.parameters['location'].type", "string");
                response.Template.FromJson<JToken>().Should().HaveValueAtPath("$.parameters['storageAccountType'].type", "string");
            });
    }

    [TestMethod]
    public async Task GetSnapshot_returns_a_snapshot()
    {
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                ["/main.bicepparam"] = """
                    using './main.bicep'

                    param location = 'eastus'
                    """,
                ["/main.bicep"] = """
                    @description('Storage Account type')
                    param storageAccountType string = 'Standard_LRS'
                    
                    @description('The storage account location.')
                    param location string = resourceGroup().location
                    
                    @description('The name of the storage account')
                    param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'
                    
                    resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                      name: storageAccountName
                      location: location
                      sku: {
                        name: storageAccountType
                      }
                      kind: 'StorageV2'
                      properties: {}
                    }
                    """,
            });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetSnapshot(new("/main.bicepparam", new(
                    TenantId: null,
                    SubscriptionId: "11068ed9-6c31-4a47-8183-4eca6d84bb32",
                    ManagementGroupId: null,
                    ResourceGroup: "myRg",
                    Location: null,
                    DeploymentName: null),
                    null), token);

                response.Snapshot.FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
                    {
                      "predictedResources": [
                        {
                          "id": "/subscriptions/11068ed9-6c31-4a47-8183-4eca6d84bb32/resourceGroups/myRg/providers/Microsoft.Storage/storageAccounts/storepwt7yebfrftwu",
                          "type": "Microsoft.Storage/storageAccounts",
                          "name": "storepwt7yebfrftwu",
                          "apiVersion": "2022-09-01",
                          "location": "eastus",
                          "sku": {
                            "name": "Standard_LRS"
                          },
                          "kind": "StorageV2",
                          "properties": {}
                        }
                      ],
                      "diagnostics": [],
                      "outputs": {}
                    }
                    """));
            });
    }

    [TestMethod]
    public async Task GetSnapshot_returns_a_snapshot_with_external_inputs()
    {
        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                ["/main.bicepparam"] = """
                    using './main.bicep'

                    param location = externalInput('custom.binding', '__MY_REGION__')
                    param storageAccountType = externalInput('custom.binding', '__UNRESOLVED_BINDING__')
                    """,
                ["/main.bicep"] = """
                    @description('Storage Account type')
                    param storageAccountType string = 'Standard_LRS'
                    
                    @description('The storage account location.')
                    param location string = resourceGroup().location
                    
                    @description('The name of the storage account')
                    param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'
                    
                    resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                      name: storageAccountName
                      location: location
                      sku: {
                        name: storageAccountType
                      }
                      kind: 'StorageV2'
                      properties: {}
                    }
                    """,
            });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                ImmutableArray<GetSnapshotRequest.ExternalInputValue> externalInputs = [
                    new("custom.binding", "__MY_REGION__", "Antarctica"),
                ];

                var response = await client.GetSnapshot(new("/main.bicepparam", new(
                    TenantId: null,
                    SubscriptionId: "11068ed9-6c31-4a47-8183-4eca6d84bb32",
                    ManagementGroupId: null,
                    ResourceGroup: "myRg",
                    Location: null,
                    DeploymentName: null),
                    externalInputs), token);

                response.Snapshot.FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
                    {
                      "predictedResources": [
                        {
                          "id": "/subscriptions/11068ed9-6c31-4a47-8183-4eca6d84bb32/resourceGroups/myRg/providers/Microsoft.Storage/storageAccounts/storepwt7yebfrftwu",
                          "type": "Microsoft.Storage/storageAccounts",
                          "name": "storepwt7yebfrftwu",
                          "apiVersion": "2022-09-01",
                          "location": "Antarctica",
                          "sku": {
                            "name": "[externalInputs('custom_binding_1')]"
                          },
                          "kind": "StorageV2",
                          "properties": {}
                        }
                      ],
                      "diagnostics": [],
                      "outputs": {}
                    }
                    """));
            });
    }

    [TestMethod]
    public async Task Format_returns_formatted_content()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = """
param foo string
param bar int = 42

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
name: 'mystorageaccount'
location: 'East US'
sku: {
name: 'Standard_LRS'
}
kind: 'StorageV2'
}
""",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.Format(new("/main.bicep"), token);
                response.Contents.Should().NotBeNull();
                response.Contents.Should().Contain("param foo string");
                response.Contents.Should().Contain("param bar int = 42");
                // The formatted content should have proper indentation
                response.Contents.Should().Contain("  name: 'mystorageaccount'");
                response.Contents.Should().Contain("  location: 'East US'");
            });
    }
}
