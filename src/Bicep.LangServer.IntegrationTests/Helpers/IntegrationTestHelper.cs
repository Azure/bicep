using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        public static async Task<ILanguageClient> StartServerWithClientConnection(Action<LanguageClientOptions> onClientOptions)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            var server = new Server(serverPipe.Reader, clientPipe.Writer);
            var _ = server.Run(CancellationToken.None); // do not wait on this async method, or you'll be waiting a long time!

            var client = LanguageClient.PreInit(options => 
            {   
                options
                    .WithInput(clientPipe.Reader)
                    .WithOutput(serverPipe.Writer);

                onClientOptions(options);
            });
            await client.Initialize(CancellationToken.None);

            return client;
        }

        public static async Task<T> WithTimeout<T>(Task<T> task, int timeout = 60000)
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
    }
}