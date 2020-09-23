// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Server;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Bicep.LanguageServer
{
    public class Server
    {
        private readonly OmnisharpLanguageServer server;

        public Server(PipeReader input, PipeWriter output)
            : this(options => options.WithInput(input).WithOutput(output))
        {
        }

        public Server(Stream input, Stream output)
            : this(options => options.WithInput(input).WithOutput(output))
        {
        }
        
        private Server(Action<LanguageServerOptions> onOptionsFunc)
        {
            server = OmniSharp.Extensions.LanguageServer.Server.LanguageServer.PreInit(options =>
            {
                options
                    .WithHandler<BicepTextDocumentSyncHandler>()
                    .WithHandler<BicepDocumentSymbolHandler>()
                    .WithHandler<BicepDefinitionHandler>()
                    .WithHandler<BicepReferencesHandler>()
                    .WithHandler<BicepDocumentHighlightHandler>()
                    .WithHandler<BicepRenameHandler>()
                    .WithHandler<BicepHoverHandler>()
                    .WithHandler<BicepCompletionHandler>()
#pragma warning disable 0612 // disable 'obsolete' warning for proposed LSP feature
                    .WithHandler<BicepSemanticTokensHandler>()
#pragma warning restore 0612
                    .WithServices(RegisterServices);

                onOptionsFunc(options);
            });
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            await server.WaitForExit;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // We don't want to recreate this every compilation.
            // I'm not using DI for it here, because I think it makes sense to set this up for the CLI at the same time.
            var resourceTypeRegistrar = new ResourceTypeRegistrar(new AzResourceTypeProvider());

            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider>(provider => new BicepCompilationProvider(resourceTypeRegistrar));
            services.AddSingleton<ISymbolResolver, BicepSymbolResolver>();
            services.AddSingleton<ICompletionProvider, BicepCompletionProvider>();
        }
    }
}
