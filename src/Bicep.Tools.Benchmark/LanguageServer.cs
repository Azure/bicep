// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TestFiles;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using Bicep.Core.UnitTests.Utils;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Client;
using System.IO.Pipelines;
using Bicep.LanguageServer;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.LangServer.IntegrationTests;
using System.Reactive.Concurrency;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class LanguageServer
{
    private record BenchmarkData(
        ILanguageClient Client,
        MultipleMessageListener<PublishDiagnosticsParams> DiagsListener);

    private static async Task<BenchmarkData> CreateBenchmarkData()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(typeof(DataSet).Assembly, "Files");
        var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
        var client = await StartServer(fileSystem, diagsListener);

        return new(client, diagsListener);
    }

    private static async Task<ILanguageClient> StartServer(IFileSystem fileSystem, MultipleMessageListener<PublishDiagnosticsParams> diagsListener)
    {
        var clientPipe = new Pipe();
        var serverPipe = new Pipe();

        var server = new Server(options => options
            .WithInput(serverPipe.Reader)
            .WithOutput(clientPipe.Writer)
            .WithServices(services => services
                .AddSingleton<IScheduler>(ImmediateScheduler.Instance) // force work to run on a single thread to make snapshot profiling simpler
                .AddSingleton(fileSystem)));

        var _ = server.RunAsync(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

        var client = LanguageClient.PreInit(options => {
            options
                .WithInput(clientPipe.Reader)
                .WithOutput(serverPipe.Writer)
                .OnPublishDiagnostics(x => diagsListener.AddMessage(x));

            options.RegisterForDisposal(server);
        });

        await client.Initialize(CancellationToken.None);

        return client;
    }

    private BenchmarkData? benchmarkData;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        this.benchmarkData = await CreateBenchmarkData();
    }

    [Benchmark(Description = "Type a document from start to finish")]
    public async Task Type_a_document_from_start_to_finish()
    {
        var (client, diagsListener) = benchmarkData!;

        var dataSet = DataSets.AKS_LF;
        var version = 0;
        var documentUri = DocumentUri.Parse($"file:///{dataSet.Name}/main.bicep");

        client.DidOpenTextDocument(new() {
            TextDocument = new () {
                LanguageId = "bicep",
                Text = "",
                Uri = documentUri,
                Version = version++,
            },
        });
        var diags = await diagsListener.WaitNext();

        for (var i = 0; i < dataSet.Bicep.Length; i++)
        {
            client.DidChangeTextDocument(new() {
                TextDocument = new () {
                    Uri = documentUri,
                    Version = version++,
                },
                ContentChanges = new(new List<TextDocumentContentChangeEvent> {
                    new() {
                        Text = dataSet.Bicep.Substring(0, i),
                    },
                }),
            });
            diags = await diagsListener.WaitNext();
        }

        client.DidCloseTextDocument(new() {
            TextDocument = new () {
                Uri = documentUri,
            },
        });
    }
}
