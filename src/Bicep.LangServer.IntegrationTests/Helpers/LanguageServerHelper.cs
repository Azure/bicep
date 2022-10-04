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
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Linq;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;

namespace Bicep.LangServer.IntegrationTests
{
    public sealed class LanguageServerHelper : IDisposable
    {
        public static readonly ISnippetsProvider SnippetsProvider = new SnippetsProvider(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager, BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.ModuleDispatcher, BicepTestConstants.LinterAnalyzer);

        public Server Server { get; }
        public ILanguageClient Client { get; }

        private LanguageServerHelper(Server server, ILanguageClient client)
        {
            Server = server;
            Client = client;
        }

        /// <summary>
        /// Creates and initializes a new language server/client pair without loading any files. This is recommended when you need to open multiple files using the language server.
        /// </summary>
        /// <param name="testContext">The test context</param>
        /// <param name="onClientOptions">The client options</param>
        /// <param name="creationOptions">The server creation options</param>
        public static async Task<LanguageServerHelper> StartServerWithClientConnectionAsync(TestContext testContext, Action<LanguageClientOptions> onClientOptions, Server.CreationOptions? creationOptions = null)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                FeatureProviderFactory = creationOptions.FeatureProviderFactory ?? BicepTestConstants.FeatureProviderFactory,
                SnippetsProvider = creationOptions.SnippetsProvider ?? SnippetsProvider,
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string>()),
                ModuleRestoreScheduler = creationOptions.ModuleRestoreScheduler ?? BicepTestConstants.ModuleRestoreScheduler
            };

            var server = new Server(
                creationOptions,
                options => options
                    .WithInput(serverPipe.Reader)
                    .WithOutput(clientPipe.Writer));
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

        /// <summary>
        /// Starts a language client/server pair that will load the specified Bicep text and wait for the diagnostics to be published.
        /// No further file opening is possible.
        /// </summary>
        /// <param name="testContext">The test context</param>
        /// <param name="text">The bicep text</param>
        /// <param name="documentUri">The document URI of the Bicep text</param>
        /// <param name="onClientOptions">The additional client options</param>
        /// <param name="creationOptions">The server creation options</param>
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
            this.Server.Dispose();
            this.Client.Dispose();
        }
    }
}
