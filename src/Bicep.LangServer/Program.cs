using System;
using System.Threading;
using System.Threading.Tasks;
<<<<<<< HEAD
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
=======
>>>>>>> origin/master

namespace Bicep.LanguageServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // the server uses JSON-RPC over stdin & stdout to communicate,
            // so be careful not to use console for logging!
            var server = new Server(Console.OpenStandardInput(), Console.OpenStandardOutput());

            await server.Run(CancellationToken.None);
        }
    }
}