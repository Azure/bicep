// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Utils;
using Bicep.Core.Navigation;
using System.IO.Pipelines;
using Bicep.LanguageServer;
using Bicep.Core.FileSystem;
using System.Collections.Generic;
using Bicep.LanguageServer.Snippets;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.UnitTests;
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Client;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Bicep.Core.UnitTests.FileSystem;

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        private const int DefaultTimeout = 30000;

        public static async Task<ILanguageClient> StartServerWithClientConnectionAsync(TestContext testContext, Action<LanguageClientOptions> onClientOptions, Server.CreationOptions? creationOptions = null)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            creationOptions ??= new Server.CreationOptions();
            creationOptions = creationOptions with
            {
                SnippetsProvider = creationOptions.SnippetsProvider ??
                    new SnippetsProvider(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager, BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.ModuleDispatcher, BicepTestConstants.LinterAnalyzer),
                FileResolver = creationOptions.FileResolver ?? new InMemoryFileResolver(new Dictionary<Uri, string>())
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
