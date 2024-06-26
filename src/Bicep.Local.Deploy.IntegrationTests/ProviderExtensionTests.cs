// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Pipes;
using System.Text.Json;
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
                finally
                {
                    await cts.CancelAsync();
                }
            }, cts.Token));
    }

    [TestMethod]
    public async Task Save_request_works_as_expected()
    {
        var handlerMock = StrictMock.Of<IResourceHandler>();
        handlerMock.SetupGet(x => x.ResourceType).Returns("apps/Deployment@v1");

        handlerMock.Setup(x => x.Save(It.IsAny<Protocol.ExtensibilityOperationRequest>(), It.IsAny<CancellationToken>()))
            .Returns<Protocol.ExtensibilityOperationRequest, CancellationToken>((req, _) =>
                Task.FromResult(new Protocol.ExtensibilityOperationResponse(req.Resource, null, null)));

        await RunExtensionTest(
            builder => builder.AddHandler(handlerMock.Object),
            async (client, token) =>
            {
                var request = new Extension.Rpc.ResourceRequestBody
                {
                    ApiVersion = "v1",
                    Type = "Microsoft.Resources/deployments",
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

                response.ResultCase.Should().Be(ResourceResponse.ResultOneofCase.Response);
                response.Response.Should().NotBeNull();
                response.Response.Type.Should().Be("apps/Deployment@v1");
            });
    }
}
