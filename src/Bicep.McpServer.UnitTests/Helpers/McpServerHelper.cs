// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Pipelines;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Logging;
using Bicep.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Bicep.McpServer.UnitTests.Helpers;

public sealed class McpServerHelper : IAsyncDisposable
{
    public IHost Server { get; }
    public McpClient Client { get; }

    private McpServerHelper(IHost server, McpClient client)
    {
        Server = server;
        Client = client;
    }

    /// <summary>
    /// Creates and initializes a new MCP server using stream transport.
    /// </summary>
    public static async Task<McpServerHelper> StartServer(TestContext testContext, Action<IServiceCollection>? onRegisterServices = null)
    {
        var clientPipe = new Pipe();
        var serverPipe = new Pipe();

        var builder = Host.CreateEmptyApplicationBuilder(settings: null);
        builder.Services
            .AddBicepMcpServer()
            .WithStreamServerTransport(
                inputStream: serverPipe.Reader.AsStream(),
                outputStream: clientPipe.Writer.AsStream());

        onRegisterServices?.Invoke(builder.Services);

        var server = builder.Build();
        var _ = server.RunAsync(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new TestContextLoggerProvider(testContext)));

        var client = await McpClient.CreateAsync(
            clientTransport: new StreamClientTransport(
                serverInput: serverPipe.Writer.AsStream(),
                serverOutput: clientPipe.Reader.AsStream()),
            loggerFactory: loggerFactory,
            cancellationToken: CancellationToken.None);

        return new McpServerHelper(server, client);
    }

    public async ValueTask DisposeAsync()
    {
        await Server.StopAsync();
        await Client.DisposeAsync();
    }
}
