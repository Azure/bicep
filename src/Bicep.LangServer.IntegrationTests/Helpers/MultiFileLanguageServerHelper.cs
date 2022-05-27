// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading.Tasks;
using Bicep.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using System.Linq;
using Bicep.LangServer.IntegrationTests.Helpers;
using System;
using FluentAssertions;

namespace Bicep.LangServer.IntegrationTests
{
    public sealed class MultiFileLanguageServerHelper : IDisposable
    {
        private readonly ConcurrentDictionary<DocumentUri, TaskCompletionSource<PublishDiagnosticsParams>> notificationRouter;

        public Server Server { get; }

        public ILanguageClient Client { get; }

        private MultiFileLanguageServerHelper(Server server, ILanguageClient client, ConcurrentDictionary<DocumentUri, TaskCompletionSource<PublishDiagnosticsParams>> notificationRouter)
        {
            this.Server = server;
            this.Client = client;
            this.notificationRouter = notificationRouter;
        }

        public static async Task<MultiFileLanguageServerHelper> StartLanguageServer(TestContext testContext, Server.CreationOptions? creationOptions = null)
        {
            var notificationRouter = new ConcurrentDictionary<DocumentUri, TaskCompletionSource<PublishDiagnosticsParams>>();
            var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(
                testContext,
                onClientOptions: options =>
                {
                    options.OnPublishDiagnostics(p =>
                    {
                        testContext.WriteLine($"Received {p.Diagnostics.Count()} diagnostic(s).");

                        if (notificationRouter.TryGetValue(p.Uri, out var completionSource))
                        {
                            completionSource.SetResult(p);
                            return;
                        }

                        throw new AssertFailedException($"Task completion source was not registered for document uri '{p.Uri}'.");
                    });
                },
                creationOptions: creationOptions);

            return new(helper.Server, helper.Client, notificationRouter);
        }

        public async Task OpenFileOnceAsync(TestContext testContext, string text, DocumentUri documentUri)
        {
            var completionSource = new TaskCompletionSource<PublishDiagnosticsParams>();

            // this is why this method is called *Once
            this.notificationRouter.TryAdd(documentUri, completionSource).Should().BeTrue("because nothing should have registered a completion source for this test before it ran");

            // send the notification
            this.Client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, text, 0));
            testContext.WriteLine($"Opened file {documentUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await IntegrationTestHelper.WithTimeoutAsync(completionSource.Task);
        }

        public void Dispose()
        {
            this.Server.Dispose();
            this.Client.Dispose();
        }
    }
}
