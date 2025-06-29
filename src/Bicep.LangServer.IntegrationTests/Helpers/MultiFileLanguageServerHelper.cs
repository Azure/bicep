// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using Bicep.Core.SourceGraph;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    public sealed class MultiFileLanguageServerHelper : IDisposable
    {
        private readonly ConcurrentDictionary<DocumentUri, MultipleMessageListener<PublishDiagnosticsParams>> notificationRouter;

        public Server Server { get; }

        public ILanguageClient Client { get; }

        private MultiFileLanguageServerHelper(Server server, ILanguageClient client, ConcurrentDictionary<DocumentUri, MultipleMessageListener<PublishDiagnosticsParams>> notificationRouter)
        {
            this.Server = server;
            this.Client = client;
            this.notificationRouter = notificationRouter;
        }

        public static async Task<MultiFileLanguageServerHelper> StartLanguageServer(TestContext testContext, Action<IServiceCollection>? onRegisterServices = null)
        {
            var notificationRouter = new ConcurrentDictionary<DocumentUri, MultipleMessageListener<PublishDiagnosticsParams>>();
            var helper = await LanguageServerHelper.StartServer(
                testContext,
                onClientOptions: options =>
                {
                    options.OnPublishDiagnostics(p =>
                    {
                        testContext.WriteLine($"Received {p.Diagnostics.Count()} diagnostic(s).");

                        if (notificationRouter.TryGetValue(p.Uri, out var completionSource))
                        {
                            completionSource.AddMessage(p);
                            return;
                        }

                        throw new AssertFailedException($"Task completion source was not registered for document uri '{p.Uri}'.");
                    });
                },
                onRegisterServices);

            return new(helper.Server, helper.Client, notificationRouter);
        }

        public async Task<PublishDiagnosticsParams> OpenFileOnceAsync(TestContext testContext, string text, DocumentUri documentUri)
        {
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            // this is why this method is called *Once
            this.notificationRouter.TryAdd(documentUri, diagsListener).Should().BeTrue("because nothing should have registered a diagnostics listener for this test before it ran");

            // send the notification
            this.Client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, text, 0));
            testContext.WriteLine($"Opened file {documentUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            return await diagsListener.WaitNext();
        }

        public async Task<PublishDiagnosticsParams> OpenFileOnceAsync(TestContext testContext, LanguageClientFile file)
            => await OpenFileOnceAsync(testContext, file.Text, file.Uri);

        public async Task<PublishDiagnosticsParams> ChangeFileAsync(TestContext testContext, string text, DocumentUri documentUri, int version)
        {
            // OpenFileOnceAsync should have already been called on this file
            this.notificationRouter.TryGetValue(documentUri, out var diagsListener).Should().BeTrue("because a diagnostics listener should have already been registered");

            // send the notification
            this.Client.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(documentUri, text, version));
            testContext.WriteLine($"Changed file {documentUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            return await diagsListener!.WaitNext();
        }

        public async Task<PublishDiagnosticsParams> WaitForDiagnostics(DocumentUri documentUri)
        {
            // OpenFileOnceAsync should have already been called on this file
            this.notificationRouter.TryGetValue(documentUri, out var diagsListener).Should().BeTrue("because a diagnostics listener should have already been registered");

            return await diagsListener!.WaitNext();
        }

        public void ChangeWatchedFile(Uri fileUri, FileChangeType changeType = FileChangeType.Changed)
        {
            this.Client.DidChangeWatchedFiles(new()
            {
                Changes = new Container<FileEvent>(new FileEvent
                {
                    Uri = fileUri,
                    Type = changeType,
                }),
            });
        }

        public async Task ChangeFileAsync(TestContext testContext, BicepFile file, int version)
            => await ChangeFileAsync(testContext, file.ProgramSyntax.ToString(), file.Uri, version);

        public void Dispose()
        {
            this.Server.Dispose();
            this.Client.Dispose();
        }
    }
}
