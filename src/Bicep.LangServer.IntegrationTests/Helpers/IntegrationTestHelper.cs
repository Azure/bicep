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
using Bicep.LanguageServer.Utils;
using System.Collections.Generic;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Linq;
using Bicep.Core.UnitTests;

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        private const int DefaultTimeout = 20000;

        public static readonly ISnippetsProvider SnippetsProvider = new SnippetsProvider(TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager);

        public static async Task<ILanguageClient> StartServerWithClientConnectionAsync(TestContext testContext, Action<LanguageClientOptions> onClientOptions, Server.CreationOptions? creationOptions = null)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                SnippetsProvider = creationOptions.SnippetsProvider ?? SnippetsProvider,
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string>())
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

            return client;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Not an issue in test code.")]
        public static async Task<T> WithTimeoutAsync<T>(Task<T> task, int timeout = DefaultTimeout)
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
        public static async Task EnsureTaskDoesntCompleteAsync<T>(Task<T> task, int timeout = DefaultTimeout)
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

        public static async Task<ILanguageClient> StartServerWithTextAsync(TestContext testContext, string text, DocumentUri documentUri, Action<LanguageClientOptions>? onClientOptions = null, Server.CreationOptions? creationOptions = null)
        {
            var diagnosticsPublished = new TaskCompletionSource<PublishDiagnosticsParams>();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string> { [documentUri.ToUri()] = text, })
            };
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
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
            client.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, text, 0));

            testContext.WriteLine($"Opened file {documentUri}.");

            // notifications don't produce responses,
            // but our server should send us diagnostics when it receives the notification
            await IntegrationTestHelper.WithTimeoutAsync(diagnosticsPublished.Task);

            return client;
        }

        public static Position GetPosition(ImmutableArray<int> lineStarts, SyntaxBase syntax)
        {
            if (syntax is ISymbolReference reference)
            {
                // get identifier span otherwise syntax.Span returns the position from the starting position of the whole expression.
                // e.g. in an instance function call such as: az.resourceGroup(), syntax.Span position starts at 'az',
                // whereas instanceFunctionCall.Name.Span the position will start in resourceGroup() which is what it should be in this
                // case.
                return PositionHelper.GetPosition(lineStarts, reference.Name.Span.Position);
            }

            if (syntax is ITopLevelDeclarationSyntax declaration)
            {
                return PositionHelper.GetPosition(lineStarts, declaration.Keyword.Span.Position);
            }

            return PositionHelper.GetPosition(lineStarts, syntax.Span.Position);
        }
    }
}
