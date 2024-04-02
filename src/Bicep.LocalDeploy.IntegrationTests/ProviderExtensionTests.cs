// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Pipes;
using System.Text.Json;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Extension;
using Bicep.Extension.Rpc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StreamJsonRpc;

namespace Bicep.LocalDeploy.IntegrationTests;

[TestClass]
public class ProviderExtensionTests : TestBase
{
    private async Task RunExtensionTest(Action<ResourceDispatcherBuilder> registerHandlers, Func<IExtensionRpcProtocol, CancellationToken, Task> testFunc)
    {
        var pipeName = Guid.NewGuid().ToString();
        using var pipeStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

        var testTimeout = TimeSpan.FromMinutes(1);
        var cts = new CancellationTokenSource(testTimeout);

        await Task.WhenAll(
            Task.Run(async () =>
            {
                var extension = new ProviderExtension();

                await extension.RunAsync(["--pipe", pipeName], registerHandlers, cts.Token);
            }),
            Task.Run(async () =>
            {
                try
                {
                    await pipeStream.WaitForConnectionAsync(cts.Token);
                    var client = JsonRpc.Attach<IExtensionRpcProtocol>(ExtensionRpcServer.CreateMessageHandler(pipeStream, pipeStream));
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
        // handlerMock.Setup(x => x.Save(It.IsAny<ExtensibilityOperationRequest>(), It.IsAny<CancellationToken>()))
        //     .Returns<ExtensibilityOperationRequest, CancellationToken>((req, _) =>
        //         Task.FromResult(new ExtensibilityOperationResponse(req.Resource, null, null)));

        handlerMock.Setup(x => x.Save(It.IsAny<ExtensibilityOperationRequest>(), It.IsAny<CancellationToken>()))
            .Returns<ExtensibilityOperationRequest, CancellationToken>((req, _) => 
                Task.FromResult(new ExtensibilityOperationResponse(req.Resource, null, null)));

        await RunExtensionTest(
            builder => builder.AddHandler(handlerMock.Object),
            async (client, token) =>
            {
                var request = """
{
  "import": {
    "provider": "Kubernetes",
    "version": "1.0.0",
    "config": {
      "kubeConfig": "redacted",
      "namespace": "default"
    }
  },
  "resource": {
    "type": "apps/Deployment@v1",
    "properties": {
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
  }
}               
""".FromJson<ExtensibilityOperationRequest>();

                var response = await client.Save(request!, token);

                response.Should().NotBeNull();
                response.Resource!.Type.Should().Be("apps/Deployment@v1");
            });
    }
}
