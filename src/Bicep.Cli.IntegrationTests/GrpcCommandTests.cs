// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.IO.Pipes;
using Bicep.Cli.Rpc;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Local.Extension.Rpc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using System.Security.Principal;
using System.Security.Permissions;
using Grpc.Core;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class GrpcCommandTests : TestBase
{
    private static async Task RunServerTest(Action<IServiceCollection> registerAction, Func<Bicep.Rpc.RpcClient, CancellationToken, Task> testFunc)
    {
        var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");

        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var result = await Bicep(registerAction, cts.Token, "grpc", "--socket", socketPath);
                result.ExitCode.Should().Be(0);
                result.Stderr.Should().EqualIgnoringNewlines("The 'grpc' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.\n");
                result.Stdout.Should().Be("");
            }),
            Task.Run(async () =>
            {
                try
                {
                    while (!File.Exists(socketPath))
                    {
                        await Task.Delay(100, cts.Token);
                    }

                    var channel = GrpcChannelHelper.CreateChannel(socketPath);
                    var client = new Bicep.Rpc.RpcClient(channel);
 
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
                var response = await client.VersionAsync(new(), new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
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
                var response = await client.CompileAsync(new() { Path = "/main.bicep" }, new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
                response.Contents.FromJson<JToken>().Should().HaveValueAtPath("$['$schema']", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");
                response.Contents.FromJson<JToken>().Should().HaveJsonAtPath("$.parameters['foo']", """
                {
                  "type": "string"
                }
                """);

                response = await client.CompileAsync(new() { Path = "/main.bicepparam" }, new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
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
                var response = await client.GetMetadataAsync(new() { Path = "/main.bicep" }, new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
                response.ToJToken().Should().DeepEqual(JObject.Parse("""
{
  "metadata": [
    {
      "name": "description",
      "value": "my file"
    }
  ],
  "parameters": [
    {
      "range": {
        "start": {
          "line": 2,
          "char": 0
        },
        "end": {
          "line": 3,
          "char": 16
        }
      },
      "name": "foo",
      "type": {
        "name": "string"
      },
      "description": "foo param",
      "hasDescription": true
    },
    {
      "range": {
        "start": {
          "line": 5,
          "char": 0
        },
        "end": {
          "line": 7,
          "char": 1
        }
      },
      "name": "inlineType",
      "type": {
        "name": "{ sdf: string }"
      },
      "description": "",
      "hasDescription": false
    },
    {
      "range": {
        "start": {
          "line": 9,
          "char": 0
        },
        "end": {
          "line": 9,
          "char": 23
        }
      },
      "name": "declaredType",
      "type": {
        "range": {
          "start": {
            "line": 11,
            "char": 0
          },
          "end": {
            "line": 15,
            "char": 1
          }
        },
        "name": "asdf"
      },
      "description": "",
      "hasDescription": false
    }
  ],
  "outputs": [
    {
      "range": {
        "start": {
          "line": 17,
          "char": 0
        },
        "end": {
          "line": 18,
          "char": 23
        }
      },
      "name": "bar",
      "type": {
        "name": "string"
      },
      "description": "bar output",
      "hasDescription": true
    }
  ],
  "exports": [
    {
      "range": {
        "start": {
          "line": 11,
          "char": 0
        },
        "end": {
          "line": 15,
          "char": 1
        }
      },
      "name": "asdf",
      "kind": "TypeAlias",
      "description": "asdf type",
      "hasDescription": true
    }
  ]
}
"""));
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
                var response = await client.GetDeploymentGraphAsync(new() { Path = "/main.bicep" }, new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
                response.ToJToken().Should().DeepEqual(JObject.Parse("""
{
  "nodes": [
    {
      "range": {
        "start": {
          "line": 4,
          "char": 0
        },
        "end": {
          "line": 7,
          "char": 1
        }
      },
      "name": "bar",
      "type": "My.Rp/foo",
      "isExisting": true,
      "relativePath": "",
      "hasRelativePath": true
    },
    {
      "range": {
        "start": {
          "line": 9,
          "char": 0
        },
        "end": {
          "line": 12,
          "char": 1
        }
      },
      "name": "baz",
      "type": "My.Rp/foo",
      "isExisting": false,
      "relativePath": "",
      "hasRelativePath": true
    },
    {
      "range": {
        "start": {
          "line": 0,
          "char": 0
        },
        "end": {
          "line": 2,
          "char": 1
        }
      },
      "name": "foo",
      "type": "My.Rp/foo",
      "isExisting": false,
      "relativePath": "",
      "hasRelativePath": true
    }
  ],
  "edges": [
    {
      "source": "bar",
      "target": "foo"
    },
    {
      "source": "baz",
      "target": "bar"
    }
  ]
}
"""));
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
                var response = await client.GetFileReferencesAsync(new() { Path = "/main.bicepparam" }, new CallOptions().WithWaitForReady(true).WithCancellationToken(token));
                response.ToJToken().Should().DeepEqual(JObject.Parse("""
{
  "filePaths": [
    "/bicepconfig.json",
    "/invalid.txt",
    "/main.bicep",
    "/main.bicepparam",
    "/valid.txt"
  ]
}
"""));
            });
    }
}
