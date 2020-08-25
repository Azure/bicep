// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;

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
