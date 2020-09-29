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
        private readonly ResourceTypeRegistrar resourceTypeRegistrar;

        public Server(ResourceTypeRegistrar resourceTypeRegistrar, PipeReader input, PipeWriter output)
            : this(resourceTypeRegistrar, options => options.WithInput(input).WithOutput(output))
        {
        }

        public Server(ResourceTypeRegistrar resourceTypeRegistrar, Stream input, Stream output)
            : this(resourceTypeRegistrar, options => options.WithInput(input).WithOutput(output))
        {
        }
        
        private Server(ResourceTypeRegistrar resourceTypeRegistrar, Action<LanguageServerOptions> onOptionsFunc)
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
                    .WithServices(services => RegisterServices(resourceTypeRegistrar, services));

                onOptionsFunc(options);
            });
            this.resourceTypeRegistrar = resourceTypeRegistrar;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            await server.WaitForExit;
        }

        private static void RegisterServices(ResourceTypeRegistrar resourceTypeRegistrar, IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider>(provider => new BicepCompilationProvider(resourceTypeRegistrar));
            services.AddSingleton<ISymbolResolver, BicepSymbolResolver>();
            services.AddSingleton<ICompletionProvider, BicepCompletionProvider>();
        }
    }
}
