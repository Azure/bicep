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
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Json;
using Bicep.Core.Semantics;
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
    public async Task RenderDocs_returns_rendered_documentation_without_writing_files()
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
                var response = await client.RenderDocs(
                    new(["/main.bicep"], null, null, null, NoRestore: false),
                    token);

                response.Results.Should().ContainSingle();
                response.Results[0].Success.Should().BeTrue();
                response.Results[0].Path.Should().Be(fileSystem.Path.GetFullPath("/main.bicep"));
                response.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Level == "Warning" &&
                    diagnostic.Code == "no-unused-params");
                response.Results[0].Contents.Should().ContainAll("# RPC Module", "Rendered through JSON-RPC.", "`value`");
                fileSystem.File.Exists("/README.md").Should().BeFalse();
            });
    }

    [TestMethod]
    public async Task RenderDocs_renders_multiple_modules_in_request_order()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/first/main.bicep"] = "metadata name = 'First'",
            ["/second/main.bicep"] = "metadata name = 'Second'",
            ["/third/main.bicep"] = "metadata name = 'Third'",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.RenderDocs(
                    new(
                        ["/second/main.bicep", "/third/main.bicep", "/first/main.bicep"],
                        null,
                        null,
                        null,
                        NoRestore: false),
                    token);

                response.Results.Should().HaveCount(3);
                response.Results.Should().OnlyContain(result => result.Success);
                response.Results[0].Contents.Should().Contain("# Second");
                response.Results[1].Contents.Should().Contain("# Third");
                response.Results[2].Contents.Should().Contain("# First");
            });
    }

    [TestMethod]
    public async Task RenderDocs_continues_after_a_failure_among_successes()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/valid/main.bicep"] = "metadata name = 'Valid'",
            ["/invalid/main.bicep"] = "param value invalidType",
            ["/other/main.bicep"] = "metadata name = 'Other'",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.RenderDocs(
                    new(
                        ["/valid/main.bicep", "/invalid/main.bicep", "/other/main.bicep"],
                        null,
                        null,
                        null,
                        NoRestore: false),
                    token);

                response.Results.Should().HaveCount(3);
                response.Results[0].Success.Should().BeTrue();
                response.Results[0].Contents.Should().Contain("# Valid");
                response.Results[1].Success.Should().BeFalse();
                response.Results[1].Contents.Should().BeNull();
                response.Results[1].Diagnostics.Should().Contain(diagnostic => diagnostic.Level == "Error");
                response.Results[2].Success.Should().BeTrue();
                response.Results[2].Contents.Should().Contain("# Other");
            });
    }

    [TestMethod]
    public async Task RenderDocs_custom_template_supports_includes_and_custom_values()
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
                var response = await client.RenderDocs(
                    new(
                        ["/main.bicep"],
                        "/template.scriban",
                        "/",
                        new() { ["owner"] = "Platform" },
                        NoRestore: true),
                    token);

                response.Results.Should().ContainSingle();
                response.Results[0].Success.Should().BeTrue();
                response.Results[0].Contents.Should().Be("Header RPC Module Platform\n");
            });
    }

    [TestMethod]
    public async Task RenderDocs_applies_configuration_and_request_overrides()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'RPC Config'",
            ["/module/examples/default/main.bicep"] = "metadata name = 'ignored'",
            ["/template.scriban"] = "{{ module.name }}|{{ custom.owner }}|{{ module.usageExamples.size }}",
            ["/bicepconfig.json"] = """
                {
                  "documentation": {
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
                }
                """,
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var configured = await client.RenderDocs(
                    new(["/module/main.bicep"], null, null, null, NoRestore: false),
                    token);
                var overridden = await client.RenderDocs(
                    new(
                        ["/module/main.bicep"],
                        null,
                        null,
                        new() { ["owner"] = "Request" },
                        NoRestore: false),
                    token);

                // The template file and example settings come from bicepconfig.json, not the request.
                configured.Results[0].Success.Should().BeTrue();
                configured.Results[0].Contents.Should().Be("RPC Config|Config|0\n");
                overridden.Results[0].Success.Should().BeTrue();
                overridden.Results[0].Contents.Should().Be("RPC Config|Request|0\n");
            });
    }

    [TestMethod]
    public async Task RenderDocs_uses_discovered_bicep_configuration()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'RPC defaults'",
            ["/bicepconfig.json"] = """
                {
                  "documentation": {
                    "output": {
                      "file": "RPC.md"
                    }
                  }
                }
                """,
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.RenderDocs(
                    new(["/module/main.bicep"], null, null, null, NoRestore: false),
                    token);

                response.Results.Should().ContainSingle();
                response.Results[0].Success.Should().BeTrue();
                response.Results[0].Contents.Should().Contain("# RPC defaults");

                // The configured output file is never written; the client owns the filesystem.
                fileSystem.File.Exists("/module/RPC.md").Should().BeFalse();
            });
    }

    [TestMethod]
    public async Task Docs_methods_never_write_files()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = "metadata name = 'No Writes'",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var before = fileSystem.AllFiles.OrderBy(file => file, StringComparer.Ordinal).ToArray();

                var rendered = await client.RenderDocs(
                    new(["/module/main.bicep"], null, null, null, NoRestore: false),
                    token);
                var model = await client.GetDocsModel(
                    new(["/module/main.bicep"], NoRestore: false),
                    token);

                rendered.Results[0].Success.Should().BeTrue();
                model.Results[0].Success.Should().BeTrue();
                fileSystem.AllFiles.OrderBy(file => file, StringComparer.Ordinal).Should().Equal(before);
            });
    }

    [TestMethod]
    public async Task RenderDocs_returns_structured_failures()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'Disabled'",
            ["/main.txt"] = "not bicep",
            ["/invalid.scriban"] = "{{ if module.name }}",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var invalidExtension = await client.RenderDocs(
                    new(["/main.txt"], null, null, null, NoRestore: false),
                    token);
                invalidExtension.Results[0].Success.Should().BeFalse();
                invalidExtension.Results[0].Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var invalidPath = await client.RenderDocs(
                    new(["invalid\0path"], null, null, null, NoRestore: false),
                    token);
                invalidPath.Results[0].Success.Should().BeFalse();
                invalidPath.Results[0].Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var missingPath = await client.RenderDocs(
                    new(["/missing.bicep"], null, null, null, NoRestore: false),
                    token);
                missingPath.Results[0].Success.Should().BeFalse();
                missingPath.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message.Contains("does not exist"));

                var invalidTemplate = await client.RenderDocs(
                    new(["/main.bicep"], "/invalid.scriban", null, null, NoRestore: false),
                    token);
                invalidTemplate.Results[0].Success.Should().BeFalse();
                invalidTemplate.Results[0].Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS003");

                var missingTemplateRoot = await client.RenderDocs(
                    new(["/main.bicep"], null, "/missing", null, NoRestore: false),
                    token);
                missingTemplateRoot.Results[0].Success.Should().BeFalse();
                missingTemplateRoot.Results[0].Diagnostics.Should().ContainSingle(diagnostic => diagnostic.Code == "DOCS001");

                var mixedResult = await client.RenderDocs(
                    new(["/missing.bicep", "/main.bicep"], null, null, null, NoRestore: false),
                    token);
                mixedResult.Results.Should().HaveCount(2);
                mixedResult.Results[0].Success.Should().BeFalse();
                mixedResult.Results[1].Success.Should().BeTrue();
            });
    }

    [TestMethod]
    public async Task RenderDocs_returns_structured_compilation_exceptions()
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
                var response = await client.RenderDocs(
                    new(["/main.bicep"], null, null, null, NoRestore: false),
                    token);

                response.Results[0].Success.Should().BeFalse();
                response.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
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
                var rendered = await client.RenderDocs(
                    new(["invalid"], null, null, null, NoRestore: false),
                    token);
                var model = await client.GetDocsModel(
                    new(["invalid"], NoRestore: false),
                    token);

                rendered.Results.Should().ContainSingle();
                rendered.Results[0].Success.Should().BeFalse();
                rendered.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message == "invalid path");
                model.Results.Should().ContainSingle();
                model.Results[0].Success.Should().BeFalse();
                model.Results[0].Model.Should().BeNull();
                model.Results[0].Diagnostics.Should().ContainSingle(diagnostic =>
                    diagnostic.Code == "DOCS001" &&
                    diagnostic.Message == "invalid path");
            });
    }

    [TestMethod]
    public async Task Docs_methods_pass_request_cancellation_to_generation()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/main.bicep"] = "metadata name = 'Cancellation'",
        });
        var generator = new CancellationObservingDocumentationGenerator();

        await RunServerTest(
            services => services
                .WithFileSystem(fileSystem)
                .AddSingleton<IBicepDocumentationGenerator>(generator),
            async (client, token) =>
            {
                var rendered = await client.RenderDocs(
                    new(["/main.bicep"], null, null, null, NoRestore: false),
                    token);

                rendered.Results[0].Success.Should().BeTrue();
                generator.BuildObserved.Should().BeTrue();
                generator.RenderObserved.Should().BeTrue();

                generator.Reset();

                var model = await client.GetDocsModel(
                    new(["/main.bicep"], NoRestore: false),
                    token);

                model.Results[0].Success.Should().BeTrue();
                generator.BuildObserved.Should().BeTrue();
                generator.RenderObserved.Should().BeFalse();
            });
    }

    [TestMethod]
    public async Task GetDocsModel_returns_the_typed_documentation_model()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/module/main.bicep"] = """
                metadata name = 'Model Module'
                metadata description = 'Exercises the documentation model.'

                @description('The deployment location.')
                @allowed(['westeurope', 'eastus'])
                param location string = 'westeurope'

                @description('Retention in days.')
                @minValue(1)
                @maxValue(365)
                param retentionDays int

                @description('Storage settings.')
                param settings settingsType

                @export()
                @description('Settings for storage.')
                type settingsType = {
                  @description('The account name.')
                  @minLength(3)
                  @maxLength(24)
                  accountName: string
                }

                @export()
                @description('The default prefix.')
                var defaultPrefix = 'stg'

                @export()
                @description('Builds a resource name.')
                func buildName(prefix string, suffix string) string => '${prefix}${suffix}'

                module child './child.bicep' = {
                  name: 'child'
                  params: {
                    accountName: settings.accountName
                  }
                }

                @description('The resolved location.')
                output resolvedLocation string = '${location}${retentionDays}${defaultPrefix}'
                """,
            ["/module/child.bicep"] = """
                metadata description = 'The child module.'

                param accountName string
                output name string = accountName
                """,
            ["/module/examples/default/main.bicep"] = "metadata name = 'Default example'",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetDocsModel(
                    new(["/module/main.bicep"], NoRestore: false),
                    token);

                response.Results.Should().ContainSingle();
                response.Results[0].Success.Should().BeTrue();

                var model = response.Results[0].Model;
                model.Should().NotBeNull();
                model!.Name.Should().Be("Model Module");
                model.Description.Should().Be("Exercises the documentation model.");
                model.Path.Should().Be(fileSystem.Path.GetFullPath("/module/main.bicep"));
                model.TargetScope.Should().Be("resourceGroup");

                var location = model.Parameters.Single(parameter => parameter.Name == "location");
                location.TypeName.Should().Be("string");
                location.IsRequired.Should().BeFalse();
                location.IsSecure.Should().BeFalse();
                location.Description.Should().Be("The deployment location.");
                location.DefaultValue.Should().Be("'westeurope'");
                location.AllowedValues.Should().Equal("eastus", "westeurope");

                var retention = model.Parameters.Single(parameter => parameter.Name == "retentionDays");
                retention.IsRequired.Should().BeTrue();
                retention.MinValue.Should().Be(1);
                retention.MaxValue.Should().Be(365);

                var settings = model.Parameters.Single(parameter => parameter.Name == "settings");
                settings.NestedProperties.Should().ContainSingle();
                settings.NestedProperties[0].Name.Should().Be("accountName");
                settings.NestedProperties[0].MinLength.Should().Be(3);
                settings.NestedProperties[0].MaxLength.Should().Be(24);

                model.Outputs.Should().ContainSingle();
                model.Outputs[0].Name.Should().Be("resolvedLocation");
                model.Outputs[0].TypeName.Should().Be("string");

                model.ExportedTypes.Should().ContainSingle(export => export.Name == "settingsType");
                model.ExportedVariables.Should().ContainSingle(export => export.Name == "defaultPrefix");
                model.ExportedFunctions.Should().ContainSingle();
                model.ExportedFunctions[0].Name.Should().Be("buildName");
                model.ExportedFunctions[0].ReturnTypeName.Should().Be("string");
                model.ExportedFunctions[0].Parameters.Select(parameter => parameter.Name)
                    .Should().Equal("prefix", "suffix");

                model.References.Should().ContainSingle();
                model.References[0].SymbolicName.Should().Be("child");
                model.References[0].Path.Should().Be("./child.bicep");
                model.References[0].Description.Should().Be("The child module.");

                model.UsageExamples.Should().ContainSingle();
                model.UsageExamples[0].Contents.Should().Contain("Default example");
            });
    }

    [TestMethod]
    public async Task GetDocsModel_resolves_configuration_for_each_module()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["/configured/main.bicep"] = "metadata name = 'Configured'",
            ["/configured/examples/default/main.bicep"] = "metadata name = 'Configured example'",
            ["/configured/bicepconfig.json"] = """
                {
                  "documentation": {
                    "template": {
                      "values": {
                        "owner": "Config"
                      }
                    },
                    "examples": {
                      "sources": []
                    }
                  }
                }
                """,
            ["/defaults/main.bicep"] = "metadata name = 'Defaults'",
            ["/defaults/examples/default/main.bicep"] = "metadata name = 'Default example'",
        });

        await RunServerTest(
            services => services.WithFileSystem(fileSystem),
            async (client, token) =>
            {
                var response = await client.GetDocsModel(
                    new(["/configured/main.bicep", "/defaults/main.bicep"], NoRestore: false),
                    token);

                response.Results.Should().HaveCount(2);
                response.Results.Should().OnlyContain(result => result.Success);

                // Configuration is resolved per module: example discovery is disabled and custom
                // values are supplied only for the first module.
                response.Results[0].Model!.Custom.Should().Contain(
                    new KeyValuePair<string, string>("owner", "Config"));
                response.Results[0].Model!.UsageExamples.Should().BeEmpty();

                response.Results[1].Model!.Custom.Should().BeEmpty();
                response.Results[1].Model!.UsageExamples.Should().ContainSingle();
            });
    }

    [TestMethod]
    public async Task GetDocsModel_continues_after_a_failure_among_successes()
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
                var response = await client.GetDocsModel(
                    new(["/invalid/main.bicep", "/valid/main.bicep"], NoRestore: false),
                    token);

                response.Results.Should().HaveCount(2);
                response.Results[0].Success.Should().BeFalse();
                response.Results[0].Model.Should().BeNull();
                response.Results[0].Diagnostics.Should().Contain(diagnostic => diagnostic.Level == "Error");
                response.Results[1].Success.Should().BeTrue();
                response.Results[1].Model!.Name.Should().Be("Valid");
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

    private sealed class CancellationObservingDocumentationGenerator : IBicepDocumentationGenerator
    {
        public bool BuildObserved { get; private set; }

        public bool RenderObserved { get; private set; }

        public void Reset()
        {
            BuildObserved = false;
            RenderObserved = false;
        }

        public BicepDocumentationModel BuildModel(
            Compilation compilation,
            IReadOnlyDictionary<string, string>? customValues = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.CanBeCanceled.Should().BeTrue();
            BuildObserved = true;

            return new(
                "Cancellation",
                null,
                compilation.SourceFileGrouping.EntryPoint.FileHandle.Uri.GetFilePath(),
                "resourceGroup",
                ImmutableSortedDictionary<string, string>.Empty,
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                []);
        }

        public string Render(
            BicepDocumentationModel model,
            BicepDocumentationGenerationOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.CanBeCanceled.Should().BeTrue();
            RenderObserved = true;

            return "# Cancellation\n";
        }
    }
}
