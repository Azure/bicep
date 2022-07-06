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
using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using Bicep.VSLanguageServerClient.ProcessLauncher;
using Bicep.VSLanguageServerClient.ProcessTracker;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using StreamJsonRpc;

namespace Bicep.VSLanguageServerClient
{
    [Export(typeof(ILanguageClient))]
    [ContentType(BicepContentTypeDefinition.ContentType)]
    public class BicepLanguageServerClient : ILanguageClient, ILanguageClientCustomMessage2
    {
        private IClientProcess? _process;
        private readonly ILanguageClientMiddleLayer _middleLayer;
        private readonly IProcessTracker _processTracker;

        [ImportingConstructor]
        public BicepLanguageServerClient(IProcessTracker processTracker)
        {
            _processTracker = processTracker;

            var setupConfiguration = new SetupConfiguration();
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer(setupConfiguration.GetInstanceForCurrentProcess().GetInstallationVersion());
            var updateColorizationMappingsMiddleLayer = new UpdateColorizationMappingsMiddleLayer();
            _middleLayer = new AggregatingMiddleLayer(handleSnippetCompletionsMiddleLayer, updateColorizationMappingsMiddleLayer);
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
            string vsixInstallPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string languageServerExePath = Path.Combine(vsixInstallPath, "Bicep.LangServer.exe");
            
            var launchServerArguments = $" --contentType {BicepContentTypeDefinition.ContentType}" +
                $" --lcid {Thread.CurrentThread.CurrentUICulture.LCID}";

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _process = ClientProcessLauncher.CreateClientProcess(languageServerExePath, launchServerArguments, null, null);

            _processTracker.AddProcess(_process.Process);

            Debug.WriteLine($"Started {BicepContentTypeDefinition.ContentType} server with process ID {_process.Process.Id}");

            Connection connection = new Connection(_process.StandardOutput.BaseStream,
                                                   _process.StandardInput.BaseStream);

            return connection;
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

        public object MiddleLayer => _middleLayer;

        public object CustomMessageTarget => null!;
    }
}
