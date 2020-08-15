using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Server;
using OmnisharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Bicep.LanguageServer
{
    public class Server
    {
        private OmnisharpLanguageServer server;

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
            // using type based registration so dependencies can be injected automatically
            // without manually constructing up the graph
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
            services.AddSingleton<ICompilationProvider, BicepCompilationProvider>();
        }
    }
}