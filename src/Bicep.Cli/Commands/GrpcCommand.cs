// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Rpc;
using Bicep.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Cli.Commands;

public class GrpcCommand : ICommand
{
    private readonly BicepCompiler compiler;
    private readonly IOContext io;

    public GrpcCommand(BicepCompiler compiler, IOContext io)
    {
        this.compiler = compiler;
        this.io = io;
    }

    public async Task<int> RunAsync(GrpcArguments args, CancellationToken cancellationToken)
    {
        await io.Error.WriteLineAsync("The 'grpc' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(options =>
        {
            if (args.Socket is {} socket)
            {
                options.ListenUnixSocket(socket, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            }
            else
            {
                throw new InvalidOperationException("Neither pipe nor socket has been specified");
            }
        });

        builder.Services.AddGrpc();
        builder.Services.AddSingleton(compiler);
        var app = builder.Build();
        app.MapGrpcService<RpcImpl>();

        await app.StartAsync(cancellationToken);
        await WaitForCancellation(cancellationToken);
        await app.StopAsync(cancellationToken);

        return 0;
    }

    private static async Task WaitForCancellation(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            // don't throw - continue
        }
    }
}
