// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.LanguageServer
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await RunWithCancellationAsync(async cancellationToken =>
            {
                // the server uses JSON-RPC over stdin & stdout to communicate,
                // so be careful not to use console for logging!
                var server = new Server(
                    Console.OpenStandardInput(),
                    Console.OpenStandardOutput(),
                    new Server.CreationOptions
                    {
                        ResourceTypeProvider = AzResourceTypeProvider.CreateWithAzTypes(),
                        FileResolver = new FileResolver()
                    });

                await server.RunAsync(cancellationToken);
            });

        private static async Task RunWithCancellationAsync(Func<CancellationToken, Task> runFunc)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                cancellationTokenSource.Cancel();
                e.Cancel = true;
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                cancellationTokenSource.Cancel();
            };

            try
            {
                await runFunc(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == cancellationTokenSource.Token)
            {
                // this is expected - no need to rethrow
            }
        }
    }
}
