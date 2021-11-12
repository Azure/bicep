// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System;
using Bicep.LangServer.IntegrationTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.UnitTests.Utils;
using System.Collections.Generic;
using Bicep.Core.FileSystem;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Linq;
using Bicep.Core.UnitTests;

namespace Bicep.LangServer.IntegrationTests
{
    public class LanguageServerHelper : IDisposable
    {
        public static readonly ISnippetsProvider SnippetsProvider = new SnippetsProvider(TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager);

        public Server Server { get; }
        public ILanguageClient Client { get; }

        private LanguageServerHelper(Server server, ILanguageClient client)
        {
            Server = server;
            Client = client;
        }

        public static async Task<LanguageServerHelper> StartServerWithClientConnectionAsync(TestContext testContext, Action<LanguageClientOptions> onClientOptions, Server.CreationOptions? creationOptions = null)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                SnippetsProvider = creationOptions.SnippetsProvider ?? SnippetsProvider,
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string>()),
                ModuleRestoreScheduler = creationOptions.ModuleRestoreScheduler ?? BicepTestConstants.ModuleRestoreScheduler
            };

            var server = new Server(serverPipe.Reader, clientPipe.Writer, creationOptions);
            var _ = server.RunAsync(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

            var client = LanguageClient.PreInit(options =>
            {
                options
                    .WithInput(clientPipe.Reader)
                    .WithOutput(serverPipe.Writer)
                    .OnInitialize((client, request, cancellationToken) => { testContext.WriteLine("Language client initializing."); return Task.CompletedTask; })
                    .OnInitialized((client, request, response, cancellationToken) => { testContext.WriteLine("Language client initialized."); return Task.CompletedTask; })
                    .OnStarted((client, cancellationToken) => { testContext.WriteLine("Language client started."); return Task.CompletedTask; })
                    .OnLogTrace(@params => testContext.WriteLine($"TRACE: {@params.Message} VERBOSE: {@params.Verbose}"))
                    .OnLogMessage(@params => testContext.WriteLine($"{@params.Type}: {@params.Message}"));

                onClientOptions(options);
            });
            await client.Initialize(CancellationToken.None);

            testContext.WriteLine("LanguageClient initialize finished.");

            return new(server, client);
        }

        public static async Task<LanguageServerHelper> StartServerWithTextAsync(TestContext testContext, string text, DocumentUri documentUri, Action<LanguageClientOptions>? onClientOptions = null, Server.CreationOptions? creationOptions = null)
        {
            var diagnosticsPublished = new TaskCompletionSource<PublishDiagnosticsParams>();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string> { [documentUri.ToUri()] = text, }),
                ModuleRestoreScheduler = creationOptions.ModuleRestoreScheduler ?? BicepTestConstants.ModuleRestoreScheduler
            };
            var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                testContext,
                options =>
                {
                    onClientOptions?.Invoke(options);
                    options.OnPublishDiagnostics(p =>
                    {
                        testContext.WriteLine($"Received {p.Diagnostics.Count()} diagnostic(s).");
                        diagnosticsPublished.SetResult(p);
                    });
                },
                creationOptions);

            // send open document notification
            helper.Client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, text, 0));

            testContext.WriteLine($"Opened file {documentUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await IntegrationTestHelper.WithTimeoutAsync(diagnosticsPublished.Task);

            return helper;
        }

        public void Dispose()
        {
            Server.Dispose();
            Client.Dispose();
        }
    }
}
