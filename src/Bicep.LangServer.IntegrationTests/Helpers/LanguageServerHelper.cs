// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Pipelines;
using Bicep.Core.Tracing;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Options;
using Bicep.LanguageServer.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LangServer.IntegrationTests
{
    public sealed class LanguageServerHelper : IDisposable
    {
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
        public static async Task<LanguageServerHelper> StartServer(TestContext testContext, Action<LanguageClientOptions>? onClientOptions = null, Action<IServiceCollection>? onRegisterServices = null)
        {
            using var timer = new ExecutionTimer($"Starting language server", logInitial: false);
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            var mockRestoreScheduler = new Mock<IModuleRestoreScheduler>(MockBehavior.Loose);
            var server = new Server(
                BicepLangServerOptions.Default,
                options => options
                    .WithInput(serverPipe.Reader)
                    .WithOutput(clientPipe.Writer)
                    .WithServices(services => services.AddSingleton(mockRestoreScheduler.Object))
                    .WithServices(services => onRegisterServices?.Invoke(services)));
            var _ = server.RunAsync(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

            var client = LanguageClient.PreInit(options =>
            {
                options
                    .WithInput(clientPipe.Reader)
                    .WithOutput(serverPipe.Writer)
                    .OnLogTrace(@params => testContext.WriteLine($"TRACE: {@params.Message} VERBOSE: {@params.Verbose}"))
                    .OnLogMessage(@params => testContext.WriteLine($"{@params.Type}: {@params.Message}"));

                onClientOptions?.Invoke(options);
            });
            await client.Initialize(CancellationToken.None);

            return new(server, client);
        }

        /// <summary>
        /// Starts a language client/server pair that will load the specified Bicep text and wait for the diagnostics to be published.
        /// No further file opening is possible.
        /// </summary>
        public static Task<LanguageServerHelper> StartServerWithText(TestContext testContext, string text, DocumentUri documentUri, Action<IServiceCollection>? onRegisterServices = null)
            => StartServerWithText(testContext, new Dictionary<DocumentUri, string> { [documentUri] = text }, documentUri, onRegisterServices);

        /// <summary>
        /// Starts a language client/server pair that will load the specified Bicep text and wait for the diagnostics to be published.
        /// No further file opening is possible.
        /// </summary>
        public static async Task<LanguageServerHelper> StartServerWithText(TestContext testContext, IReadOnlyDictionary<DocumentUri, string> fileContentsByUri, DocumentUri entryFileUri, Action<IServiceCollection>? onRegisterServices = null)
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            var fileResolver = new InMemoryFileResolver(fileContentsByUri.ToDictionary(x => x.Key.ToUriEncoded(), x => x.Value));
            var fileExplorer = new FileSystemFileExplorer(fileResolver.MockFileSystem);
            var helper = await LanguageServerHelper.StartServer(
                testContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services =>
                {
                    onRegisterServices?.Invoke(services);
                    services.WithFileResolver(fileResolver);
                    services.WithFileExplorer(fileExplorer);
                });

            // send open document notification
            helper.Client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(entryFileUri, fileContentsByUri[entryFileUri], 0));

            testContext.WriteLine($"Opened file {entryFileUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await diagnosticsListener.WaitNext();

            return helper;
        }

        public void Dispose()
        {
            this.Server.Dispose();
            this.Client.Dispose();
        }
    }
}
