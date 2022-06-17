// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bicep.VSLanguageServerClient.ContentType;
using Bicep.VSLanguageServerClient.ProcessTracker;
using Bicep.VSLanguageServerClient.Threading;
using Bicep.VSLanguageServerClient.Tracing;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using StreamJsonRpc;

namespace Bicep.VSLanguageServerClient
{
    [Export(typeof(ILanguageClient))]
    [ContentType(BicepContentTypeDefinition.ContentType)]
    public class BicepLanguageServerClient : ILanguageClient, ILanguageClientCustomMessage2
    {
        private readonly IProcessTracker _processTracker;
        //private readonly ILanguageClientMiddleLayer _middleLayer;
        private readonly IThreadingContext _threadingContext;

        [ImportingConstructor]
        public BicepLanguageServerClient(IProcessTracker processTracker, IThreadingContext threadingContext)
        {
            _processTracker = processTracker;
            _threadingContext = threadingContext;
            //_middleLayer = new AggregatingMiddleLayer(
            //    new CodeActionMiddleLayer(),
            //    //new HoverMiddleLayer(),
            //    new RemoveSnippetCompletionsMiddleLayer()); ;
        }

        public string Name => "Bicep Language Server";

        // this is allowed to return null, but can't be marked nullabe due to signature
        public virtual IEnumerable<string> ConfigurationSections => null!;

        public virtual object InitializationOptions => new object();

        public IEnumerable<string> FilesToWatch => Array.Empty<string>();

        public bool ShowNotificationOnInitializeFailed => true;

        public event AsyncEventHandler<EventArgs>? StartAsync;
#pragma warning disable 0067
        public event AsyncEventHandler<EventArgs>? StopAsync;
#pragma warning restore 0067

        public async Task<Connection?> ActivateAsync(CancellationToken token)
        {
            await _threadingContext.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            string vsixInstallPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string languageServerExePath = Path.Combine(vsixInstallPath, "Bicep.LangServer.exe");

            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = languageServerExePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process()
            {
                StartInfo = info
            };

            try
            {
                if (process.Start())
                {
                    _processTracker.AddProcess(process);
                    Connection connection = new Connection(new EtwTeeStream(process.StandardOutput.BaseStream, Name),
                        new EtwTeeStream(process.StandardInput.BaseStream, Name));

                    return connection;
                }
            }
            catch (Exception)
            {
                // TODO: log failure
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            await StartAsync.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task<InitializationFailureContext?> OnServerInitializeFailedAsync(ILanguageClientInitializationInfo initializationState)
        {
            return Task.FromResult<InitializationFailureContext?>(new InitializationFailureContext());
        }

        public Task AttachForCustomMessageAsync(JsonRpc rpc)
        {
            return Task.CompletedTask;
        }

        public object MiddleLayer => null!;

        public object CustomMessageTarget => null!;
    }
}
