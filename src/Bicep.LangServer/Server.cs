// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
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
        public class CreationOptions
        {
            public IResourceTypeProvider? ResourceTypeProvider { get; set; }

            public IFileResolver? FileResolver { get; set; }
        }

        private readonly OmnisharpLanguageServer server;

        public Server(PipeReader input, PipeWriter output, CreationOptions creationOptions)
            : this(creationOptions, options => options.WithInput(input).WithOutput(output))
        {
        }

        public Server(Stream input, Stream output, CreationOptions creationOptions)
            : this(creationOptions, options => options.WithInput(input).WithOutput(output))
        {
        }
        
        private Server(CreationOptions creationOptions, Action<LanguageServerOptions> onOptionsFunc)
        {
            BicepDeploymentsInterop.Initialize();
            server = OmniSharp.Extensions.LanguageServer.Server.LanguageServer.PreInit(options =>
            {
                options
                    .WithHandler<BicepTextDocumentSyncHandler>()
                    .WithHandler<BicepDocumentSymbolHandler>()
                    .WithHandler<BicepDefinitionHandler>()
                    .WithHandler<BicepReferencesHandler>()
                    .WithHandler<BicepDocumentHighlightHandler>()
                    .WithHandler<BicepDocumentFormattingHandler>()
                    .WithHandler<BicepRenameHandler>()
                    .WithHandler<BicepHoverHandler>()
                    .WithHandler<BicepCompletionHandler>()
                    .WithHandler<BicepCodeActionHandler>()
                    .WithHandler<BicepDidChangeWatchedFilesHandler>()
#pragma warning disable 0612 // disable 'obsolete' warning for proposed LSP feature
                    .WithHandler<BicepSemanticTokensHandler>()
#pragma warning restore 0612
                    .WithServices(services => RegisterServices(creationOptions, services));

                onOptionsFunc(options);
            });
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await server.Initialize(cancellationToken);

            await server.WaitForExit;
        }

        private static void RegisterServices(CreationOptions creationOptions, IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<IResourceTypeProvider>(services => creationOptions.ResourceTypeProvider ?? new AzResourceTypeProvider());
            services.AddSingleton<IFileResolver>(services => creationOptions.FileResolver ?? new FileResolver());
            services.AddSingleton<IWorkspace, Workspace>();
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider, BicepCompilationProvider>();
            services.AddSingleton<ISymbolResolver, BicepSymbolResolver>();
            services.AddSingleton<ICompletionProvider, BicepCompletionProvider>();
        }
    }
}
