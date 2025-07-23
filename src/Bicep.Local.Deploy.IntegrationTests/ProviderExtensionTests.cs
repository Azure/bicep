// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Extension;
using Bicep.Local.Extension.Protocol;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using Protocol = Bicep.Local.Extension.Protocol;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class ProviderExtensionTests : TestBase
{
    public enum ChannelMode
    {
        UnixDomainSocket,
        NamedPipe,
    }

    private async Task RunExtensionTest(string[] processArgs, Func<GrpcChannel> channelBuilder, Action<ResourceDispatcherBuilder> registerHandlers, Func<Rpc.BicepExtension.BicepExtensionClient, CancellationToken, Task> testFunc)
    {
        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var extension = new KestrelProviderExtension();

                await extension.RunAsync(processArgs, registerHandlers, cts.Token);
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

    private async Task RunExtensionTest(Func<Mock<IGenericResourceHandler>, Rpc.BicepExtension.BicepExtensionClient, CancellationToken, Task> testFunc)
    {
        var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
        var mockHandler = StrictMock.Of<IGenericResourceHandler>();

        await RunExtensionTest(
            ["--socket", socketPath],
            () => GrpcChannelHelper.CreateDomainSocketChannel(socketPath),
            builder => builder.AddGenericHandler(mockHandler.Object),
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

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var handlerMock = StrictMock.Of<IResourceHandler>();
        handlerMock.SetupGet(x => x.ResourceType).Returns("apps/Deployment");
        handlerMock.SetupCreateOrUpdate(req => new(
                new(req.Type, req.ApiVersion, "Succeeded", identifiers, req.Config, req.Properties),
                null));

        await RunExtensionTest(
            processArgs,
            channelBuilder,
            builder => builder.AddHandler(handlerMock.Object),
            async (client, token) =>
            {
                var request = new Rpc.ResourceSpecification
                {
                    ApiVersion = "v1",
                    Type = "apps/Deployment",
                    Config = """
                        {
                            "kubeConfig": {
                                "type": "string",
                                "defaultValue": "redacted"
                            },
                            "namespace": {
                                "type": "string",
                                "defaultValue": "default"
                            }
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

                response.Should().NotBeNull();
                response.Resource.Should().NotBeNull();
                response.Resource.Type.Should().Be("apps/Deployment");
                response.Resource.Identifiers.Should().NotBeNullOrEmpty();
                var responseIdentifiers = JsonObject.Parse(response.Resource.Identifiers)!.AsObject();
                responseIdentifiers.Should().NotBeNullOrEmpty();
                responseIdentifiers["name"]!.GetValue<string>().Should().Be(identifiers["name"]!.GetValue<string>());
                responseIdentifiers["namespace"]!.GetValue<string>().Should().Be(identifiers["namespace"]!.GetValue<string>());
            });
    }

    [TestMethod]
    public Task Error_details_and_inner_error_can_be_null() => RunExtensionTest(async (handlerMock, client, token) =>
    {
        Protocol.Error error = new("SomeErrorCode", "SomeTarget", "SomeMessage", null, null);
        handlerMock.SetupCreateOrUpdate(req => new(null, new(error)));

        var response = await client.CreateOrUpdateAsync(new()
        {
            Type = "mockResource",
            Properties = """
{}
""",
        }, cancellationToken: token);

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be(error.Code);
        response.ErrorData.Error.Target.Should().Be(error.Target);
        response.ErrorData.Error.Message.Should().Be(error.Message);
        response.ErrorData.Error.Details.Should().BeNullOrEmpty();
        response.ErrorData.Error.InnerError.Should().BeNullOrEmpty();
    });
}
