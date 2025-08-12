// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Extension;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types.Attributes;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using Handlers = Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class ProviderExtensionTests : TestBase
{
    public enum ChannelMode
    {
        UnixDomainSocket,
        NamedPipe,
    }

    private async Task RunExtensionTest(string[] processArgs, Func<GrpcChannel> channelBuilder, Action<IBicepExtensionBuilder> registerHandlers, Func<Rpc.BicepExtension.BicepExtensionClient, CancellationToken, Task> testFunc)
    {
        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var builder = WebApplication.CreateBuilder();

                builder.AddBicepExtensionHost(processArgs);
                registerHandlers(builder.Services
                    .AddBicepExtension(
                        name: "MockExtension",
                        version: "0.0.1",
                        isSingleton: true,
                        typeAssembly: typeof(ProviderExtensionTests).Assembly));

                var app = builder.Build();

                app.MapBicepExtension();

                await app.RunAsync(cts.Token);
            }),
            Task.Run(async () =>
            {
                try
                {
                    var client = new Rpc.BicepExtension.BicepExtensionClient(channelBuilder());

                    await GrpcChannelHelper.WaitForConnectionAsync(client, cts.Token);

                    await testFunc(client, cts.Token);
                }
                finally
                {
                    await cts.CancelAsync();
                }
            }, cts.Token));
    }

    private async Task RunExtensionTest(Func<MockResourceHandler, Rpc.BicepExtension.BicepExtensionClient, CancellationToken, Task> testFunc)
    {
        var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
        var mockHandler = new MockResourceHandler();

        await RunExtensionTest(
            ["--socket", socketPath],
            () => GrpcChannelHelper.CreateDomainSocketChannel(socketPath),
            builder => builder.WithResourceHandler(mockHandler),
            (client, token) => testFunc(mockHandler, client, token));
    }

    private static IEnumerable<object[]> GetDataSets()
    {
        yield return new object[] { ChannelMode.UnixDomainSocket };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Kestrel only supports named pipes on Windows
            yield return new object[] { ChannelMode.NamedPipe };
        }
    }

    [ResourceType("apps/Deployment")]
    public class AppsDeploymentResource
    {

    }

    [TestMethod]
    [DynamicData(nameof(GetDataSets), DynamicDataSourceType.Method)]
    public async Task Save_request_works_as_expected(ChannelMode mode)
    {
        string[] processArgs;
        Func<GrpcChannel> channelBuilder;
        switch (mode)
        {
            case ChannelMode.UnixDomainSocket:
                var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
                processArgs = ["--socket", socketPath];
                channelBuilder = () => GrpcChannelHelper.CreateDomainSocketChannel(socketPath);
                break;
            case ChannelMode.NamedPipe:
                var pipeName = $"{Guid.NewGuid()}.tmp";
                processArgs = ["--pipe", pipeName];
                channelBuilder = () => GrpcChannelHelper.CreateNamedPipeChannel(pipeName);
                break;
            default:
                throw new NotImplementedException();
        }

        var handler = new MockResourceHandler
        {
            OnCreateOrUpdate = (req, _) =>
            {
                req.Type.Should().Be("apps/Deployment");
                req.ApiVersion.Should().Be("v1");
                req.Properties.Should().NotBeNull();
                req.Config.KubeConfig.Should().Be("redacted");
                req.Config.Namespace.Should().Be("default");

                return new MockResourceHandler.ResourceResponse
                {
                    Type = req.Type,
                    ApiVersion = req.ApiVersion,
                    Identifiers = new()
                    {
                        Metadata = req.Properties.Metadata,
                    },
                    Properties = req.Properties,
                };
            }
        };

        await RunExtensionTest(
            processArgs,
            channelBuilder,
            builder => builder.WithResourceHandler(handler),
            async (client, token) =>
            {
                var request = new Rpc.ResourceSpecification
                {
                    ApiVersion = handler.ApiVersion,
                    Type = handler.Type,
                    Config = """
                        {
                            "kubeConfig": "redacted",
                            "namespace": "default"
                        }
                        """,
                    Properties = """
                        {
                          "metadata": {
                            "name": "echo-server"
                          },
                          "spec": {
                            "selector": {
                              "matchLabels": {
                                "app": "echo-server",
                                "version": "0.8.12"
                              }
                            },
                            "replicas": 1,
                            "template": {
                              "metadata": {
                                "labels": {
                                  "app": "echo-server",
                                  "version": "0.8.12"
                                }
                              },
                              "spec": {
                                "containers": [
                                  {
                                    "name": "echo-server",
                                    "image": "ealen/echo-server:0.8.12",
                                    "ports": [
                                      {
                                        "containerPort": 80
                                      }
                                    ]
                                  }
                                ]
                              }
                            }
                          }
                        }
                        """
                };

                var response = await client.CreateOrUpdateAsync(request, cancellationToken: token);

                response.Resource.Type.Should().Be("apps/Deployment");
                response.Resource.Identifiers.FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
                {
                    "metadata": {
                        "name": "echo-server"
                    }
                }
                """));
            });
    }

    [TestMethod]
    public Task Error_details_and_inner_error_can_be_null() => RunExtensionTest(async (handler, client, token) =>
    {
        handler.OnCreateOrUpdate = (req, _) => throw new MockResourceHandler.ResourceErrorException("SomeErrorCode", "SomeMessage", "SomeTarget");

        var response = await client.CreateOrUpdateAsync(new()
        {
            Type = handler.Type,
            ApiVersion = handler.ApiVersion,
            Properties = """
            {
                "metadata": {
                    "name": "echo-server"
                },
                "spec": {}
            }
            """,
            Config = """
            {
                "kubeConfig": "redacted",
                "namespace": "default"
            }
            """
        }, cancellationToken: token);

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("SomeErrorCode");
        response.ErrorData.Error.Target.Should().Be("SomeTarget");
        response.ErrorData.Error.Message.Should().Be("SomeMessage");
        response.ErrorData.Error.Details.Should().BeNullOrEmpty();
        response.ErrorData.Error.InnerError.Should().BeNullOrEmpty();
    });
}
