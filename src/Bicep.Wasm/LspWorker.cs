// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Bicep.Wasm;

public class LspWorker
{
    private readonly IJSRuntime jsRuntime;
    private readonly Server server;
    private readonly PipeWriter inputWriter;
    private readonly PipeReader outputReader;

    public LspWorker(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
        var inputPipe = new Pipe();
        var outputPipe = new Pipe();

        this.server = new(
            options => options
                .WithInput(inputPipe.Reader)
                .WithOutput(outputPipe.Writer)
                .WithServices(services => services
                    .AddSingleton<IScheduler>(ImmediateScheduler.Instance)
                    .RegisterStubs()));

        this.inputWriter = inputPipe.Writer;
        this.outputReader = outputPipe.Reader;
    }

    public async Task RunAsync()
    {
        await Task.WhenAll(
            server.RunAsync(CancellationToken.None),
            ProcessInputStreamAsync(CancellationToken.None),
            jsRuntime.InvokeAsync<object>("LspInitialized", DotNetObjectReference.Create(this)).AsTask());
    }

    [JSInvokable("SendLspDataAsync")]
    public async Task SendMessage(string message)
    {
        await inputWriter.WriteAsync(Encoding.UTF8.GetBytes(message));
        await inputWriter.FlushAsync();
    }

    public async Task ReceiveMessage(string message)
    {
        await jsRuntime.InvokeVoidAsync("ReceiveLspDataAsync", message);
    }

    private async Task ProcessInputStreamAsync(CancellationToken cancellationToken)
    {
        do
        {
            var result = await outputReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;

            var message = Encoding.UTF8.GetString(buffer.Slice(buffer.Start, buffer.End));
            await ReceiveMessage(message);
            outputReader.AdvanceTo(buffer.End, buffer.End);

            // Stop reading if there's no more data coming.
            if (result.IsCompleted && buffer.IsEmpty)
            {
                break;
            }
        } while (!cancellationToken.IsCancellationRequested);
    }
}
