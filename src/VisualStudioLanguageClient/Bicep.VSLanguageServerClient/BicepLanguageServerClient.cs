// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer;
using Bicep.VSLanguageServerClient.ContentType;
using Bicep.VSLanguageServerClient.Helpers;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Nerdbank.Streams;
using static Bicep.LanguageServer.Server;

namespace Bicep.VSLanguageServerClient
{
    [Export(typeof(ILanguageClient))]
    [ContentType(BicepContentTypeDefinition.ContentType)]
    public class BicepLanguageServerClient : ILanguageClient, IDisposable
    {
        private Server? _languageServer;
        private readonly IThreadingContext _threadingContext;

        private Server LanguageServer
        {
            get
            {
                if (_languageServer is null)
                {
                    throw new InvalidOperationException($"{nameof(LanguageServer)} called before it's initialized");
                }

                return _languageServer;
            }
            set
            {
                _languageServer = value;
            }
        }

        [ImportingConstructor]
        public BicepLanguageServerClient(
            IThreadingContext threadingContext)
        {
            _threadingContext = threadingContext;
        }

        public string Name => "Bicep Language Server Client";

        public IEnumerable<string>? ConfigurationSections => null;

        public object? InitializationOptions => null;

        public IEnumerable<string>? FilesToWatch => null;

        public bool ShowNotificationOnInitializeFailed => true;

        public event AsyncEventHandler<EventArgs>? StartAsync;

        public event AsyncEventHandler<EventArgs>? StopAsync
        {
            add { }
            remove { }
        }

        public async Task<Connection?> ActivateAsync(CancellationToken token)
        {
            await _threadingContext.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            var (clientStream, serverStream) = FullDuplexStream.CreatePair();

            LanguageServer = new Server(new CreationOptions(), options => options.WithInput(clientStream).WithOutput(serverStream));

            var connection = new Connection(clientStream, clientStream);
            return connection;
        }

        public Task OnLoadedAsync()
        {
            return StartAsync.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            LanguageServer.Dispose();
        }

        public Task<InitializationFailureContext?> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            var initializationFailureContext = new InitializationFailureContext
            {
                // Localize
                FailureMessage = string.Format("Language Server Initialization Failed",
                    Name, initializationState.StatusMessage, initializationState.InitializationException?.ToString())
            };
            return Task.FromResult<InitializationFailureContext?>(initializationFailureContext);
        }
    }
}
