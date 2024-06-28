// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension;
using Bicep.Local.Extension.Protocol;
using Bicep.Local.Extension.Rpc;
using FluentAssertions;
using Grpc.Core;
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
    private async Task RunExtensionTest(Action<ResourceDispatcherBuilder> registerHandlers, Func<BicepExtension.BicepExtensionClient, CancellationToken, Task> testFunc)
    {
        var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");

        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var extension = new KestrelProviderExtension();

                await extension.RunAsync(["--socket", socketPath], registerHandlers, cts.Token);
            }),
            Task.Run(async () =>
            {
                try
                {
                    var channel = GrpcChannelHelper.CreateChannel(socketPath);
                    var client = new BicepExtension.BicepExtensionClient(channel);

                    await GrpcChannelHelper.WaitForConnectionAsync(client, cts.Token);

                    await testFunc(client, cts.Token);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
                finally
                {
                    await cts.CancelAsync();
                }
            }, cts.Token));
    }

    [TestMethod]
    public async Task Save_request_works_as_expected()
    {
        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var handlerMock = StrictMock.Of<IResourceHandler>();
        handlerMock.SetupGet(x => x.ResourceType).Returns("apps/Deployment");

        handlerMock.Setup(x => x.CreateOrUpdate(It.IsAny<Protocol.ResourceRequestBody>(), It.IsAny<CancellationToken>()))
            .Returns<Protocol.ResourceRequestBody, CancellationToken>((req, _) =>
                Task.FromResult(new Protocol.ResourceResponseBody(null, identifiers, req.Type, "Succeeded", req.Properties)));

        await RunExtensionTest(
            builder => builder.AddHandler(handlerMock.Object),
            async (client, token) =>
            {
                var request = new Extension.Rpc.ResourceRequestBody
                {
                    ApiVersion = "v1",
                    Type = "apps/Deployment",
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
                response.Type.Should().Be("apps/Deployment");
                response.Identifiers.Should().Be(identifiers.ToJson());
            });
    }
}
