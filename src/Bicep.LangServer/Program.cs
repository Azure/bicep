using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Bicep.LanguageServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = await OmniSharp.Extensions.LanguageServer.Server.LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithHandler<BicepTextDocumentSyncHandler>()
                    .WithHandler<BicepDocumentSymbolHandler>()
                    .WithServices(RegisterServices));

            server.Document.PublishDiagnostics(new PublishDiagnosticsParams());

            await server.WaitForExit;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // using type based registration so dependencies can be injected automatically
            // without manually building up the graph
            services.AddSingleton<ICompilationManager, BicepCompilationManager>();
        }
    }
}