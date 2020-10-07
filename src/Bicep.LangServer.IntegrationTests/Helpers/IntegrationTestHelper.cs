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

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        public static async Task<ILanguageClient> StartServerWithClientConnection(Action<LanguageClientOptions> onClientOptions)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            var server = new Server(serverPipe.Reader, clientPipe.Writer, () => TestResourceTypeProvider.Create());
            var _ = server.Run(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

            var client = LanguageClient.PreInit(options => 
            {   
                options
                    .WithInput(clientPipe.Reader)
                    .WithOutput(serverPipe.Writer);

                onClientOptions(options);
            });
            await client.Initialize(CancellationToken.None);

            return client;
        }

        public static async Task<T> WithTimeout<T>(Task<T> task, int timeout = 10000)
        {
            var completed = await Task.WhenAny(
                task,
                Task.Delay(timeout)
            );

            if (task != completed)
            {
                Assert.Fail($"Timed out waiting for task to complete after {timeout}ms");
            }

            return await task;
        }

        public static async Task<ILanguageClient> StartServerWithText(string text, DocumentUri uri, Action<LanguageClientOptions>? onClientOptions = null)
        {
            var diagnosticsPublished = new TaskCompletionSource<PublishDiagnosticsParams>();
            var client = await IntegrationTestHelper.StartServerWithClientConnection(options =>
            {
                onClientOptions?.Invoke(options);
                options.OnPublishDiagnostics(p => diagnosticsPublished.SetResult(p));
            });

            // send open document notification
            client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(uri, text, 0));

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await IntegrationTestHelper.WithTimeout(diagnosticsPublished.Task);

            return client;
        }
    }
}
