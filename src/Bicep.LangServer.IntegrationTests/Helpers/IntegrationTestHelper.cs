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
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Utils;
using System.Collections.Generic;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.Navigation;

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        public static async Task<ILanguageClient> StartServerWithClientConnectionAsync(Action<LanguageClientOptions> onClientOptions, IResourceTypeProvider? resourceTypeProvider = null, IFileResolver? fileResolver = null)
        {
            resourceTypeProvider ??= TestResourceTypeProvider.Create();
            fileResolver ??= new InMemoryFileResolver(new Dictionary<Uri, string>());

            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            var server = new Server(
                serverPipe.Reader,
                clientPipe.Writer,
                new Server.CreationOptions
                {
                    ResourceTypeProvider = resourceTypeProvider,
                    FileResolver = fileResolver,
                });
            var _ = server.RunAsync(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Not an issue in test code.")]
        public static async Task<T> WithTimeoutAsync<T>(Task<T> task, int timeout = 10000)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Not an issue in test code.")]
        public static async Task EnsureTaskDoesntCompleteAsync<T>(Task<T> task, int timeout = 10000)
        {
            var completed = await Task.WhenAny(
                task,
                Task.Delay(timeout)
            );

            if (task == completed)
            {
                Assert.Fail($"Expected task to not complete, but it completed!");
            }
        }

        public static async Task<ILanguageClient> StartServerWithTextAsync(string text, DocumentUri documentUri, Action<LanguageClientOptions>? onClientOptions = null, IResourceTypeProvider? resourceTypeProvider = null, IFileResolver? fileResolver = null)
        {
            var diagnosticsPublished = new TaskCompletionSource<PublishDiagnosticsParams>();
            fileResolver ??= new InMemoryFileResolver(new Dictionary<Uri, string> { [documentUri.ToUri()] = text, });
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                options =>
                {
                    onClientOptions?.Invoke(options);
                    options.OnPublishDiagnostics(p => diagnosticsPublished.SetResult(p));
                },
                resourceTypeProvider: resourceTypeProvider,
                fileResolver: fileResolver);

            // send open document notification
            client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, text, 0));

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await IntegrationTestHelper.WithTimeoutAsync(diagnosticsPublished.Task);

            return client;
        }

        public static Position GetPosition(ImmutableArray<int> lineStarts, SyntaxBase syntax)
        {
            if (syntax is InstanceFunctionCallSyntax instanceFunctionCall)
            {
                // get identifier span otherwise syntax.Span returns the position from the starting position of the whole expression.
                // e.g. in an instance function call such as: az.resourceGroup(), syntax.Span position starts at 'az',
                // whereas instanceFunctionCall.Name.Span the position will start in resourceGroup() which is what it should be in this
                // case.
                return PositionHelper.GetPosition(lineStarts, instanceFunctionCall.Name.Span.Position);
            }

            if (syntax is ITopLevelDeclarationSyntax declaration)
            {
                return PositionHelper.GetPosition(lineStarts, declaration.Keyword.Span.Position);
            }

            return PositionHelper.GetPosition(lineStarts, syntax.Span.Position);
        }
    }
}
