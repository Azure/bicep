// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
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

        public Server(PipeReader input, PipeWriter output, Func<IResourceTypeProvider> resourceTypeProviderBuilder)
            : this(resourceTypeProviderBuilder, options => options.WithInput(input).WithOutput(output))
        {
        }

        public Server(Stream input, Stream output, Func<IResourceTypeProvider> resourceTypeProviderBuilder)
            : this(resourceTypeProviderBuilder, options => options.WithInput(input).WithOutput(output))
        {
        }
        
        private Server(Func<IResourceTypeProvider> resourceTypeProviderBuilder, Action<LanguageServerOptions> onOptionsFunc)
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
                    .WithHandler<BicepCodeActionHandler>()
#pragma warning disable 0612 // disable 'obsolete' warning for proposed LSP feature
                    .WithHandler<BicepSemanticTokensHandler>()
#pragma warning restore 0612
                    .WithServices(services => RegisterServices(resourceTypeProviderBuilder, services));

                onOptionsFunc(options);
            });
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            await server.WaitForExit;
        }

        private static void RegisterServices(Func<IResourceTypeProvider> resourceTypeProviderBuilder, IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<IResourceTypeProvider>(services => resourceTypeProviderBuilder());
            services.AddSingleton<IFileResolver, FileResolver>();
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider, BicepCompilationProvider>();
            services.AddSingleton<ISymbolResolver, BicepSymbolResolver>();
            services.AddSingleton<ICompletionProvider, BicepCompletionProvider>();
        }
    }
}
