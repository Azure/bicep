// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.IO.Pipes;
using Bicep.Cli.Rpc;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
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
                result.Stderr.Should().EqualIgnoringNewlines("The 'jsonrpc' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.\n");
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
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
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

                response.FilePaths.Should().Equal([
                    "/bicepconfig.json",
                    "/invalid.txt",
                    "/main.bicep",
                    "/main.bicepparam",
                    "/valid.txt",
                ]);
            });
    }
}
