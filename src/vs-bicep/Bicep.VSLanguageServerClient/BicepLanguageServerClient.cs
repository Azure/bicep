// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bicep.VSLanguageServerClient.ContentType;
using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using Bicep.VSLanguageServerClient.ProcessLauncher;
using Bicep.VSLanguageServerClient.ProcessTracker;
using Bicep.VSLanguageServerClient.Telemetry;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using StreamJsonRpc;

namespace Bicep.VSLanguageServerClient
{
    [Export(typeof(ILanguageClient))]
    [ContentType(BicepContentTypeDefinition.ContentType)]
    public class BicepLanguageServerClient : ILanguageClient, ILanguageClientCustomMessage2
    {
        private IClientProcess? process;
        private readonly ILanguageClientMiddleLayer middleLayer;
        private readonly IProcessTracker processTracker;
        private readonly TelemetrySession TelemetrySession;

        [ImportingConstructor]
        public BicepLanguageServerClient(IProcessTracker processTracker)
        {
            this.processTracker = processTracker;
            this.TelemetrySession = TelemetryService.DefaultSession;
            var setupConfiguration = new SetupConfiguration();
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer(setupConfiguration.GetInstanceForCurrentProcess().GetInstallationVersion());
            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer();
            var gotoDefintionMiddleLayer = new HandleGotoDefintionMiddleLayer();
            middleLayer = new AggregatingMiddleLayer(gotoDefintionMiddleLayer, handleSnippetCompletionsMiddleLayer, updateFormatSettingsMiddleLayer);
        }

        public string Name => "Bicep Language Server";

        public virtual IEnumerable<string> ConfigurationSections => Enumerable.Empty<string>();

        public virtual object InitializationOptions => new object();

        public IEnumerable<string> FilesToWatch => Enumerable.Empty<string>();

        public bool ShowNotificationOnInitializeFailed => true;

        public event AsyncEventHandler<EventArgs>? StartAsync;
#pragma warning disable 0067  // event is never used
        public event AsyncEventHandler<EventArgs>? StopAsync;
#pragma warning restore 0067 // event is never used

        public async Task<Connection?> ActivateAsync(CancellationToken token)
        {
            string vsixInstallPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string languageServerExePath = Path.Combine(vsixInstallPath, "Bicep.LangServer.exe");
            
            var launchServerArguments = $" --contentType {BicepContentTypeDefinition.ContentType}" +
                $" --lcid {Thread.CurrentThread.CurrentUICulture.LCID}";

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            process = ClientProcessLauncher.CreateClientProcess(languageServerExePath, launchServerArguments, null, null);

            processTracker.AddProcess(process.Process);

            Debug.WriteLine($"Started {BicepContentTypeDefinition.ContentType} server with process ID {process.Process.Id}");

            var connection = new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
            var telemetryEvent = new TelemetryEvent("vs/bicep/clientInitialization");
            telemetryEvent.Properties["status"] = connection is null ? "failed" : "succeeded";

            TelemetrySession.PostEvent(telemetryEvent);

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

        public object MiddleLayer => middleLayer;

        public object CustomMessageTarget => new TelemetryCustomMessageTarget(TelemetrySession);
    }
}
