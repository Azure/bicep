// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LspDiagnostic = OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic;

namespace Bicep.LanguageServer.BicepConfig
{
    public class BicepConfigLifecycleManager : IBicepConfigLifecycleManager
    {
        private readonly IBicepConfigurationManager bicepConfigurationManager;
        private readonly FileChangeEpoch fileChangeEpoch;
        private readonly ILanguageServerFacade server;

        public BicepConfigLifecycleManager(IBicepConfigurationManager bicepConfigurationManager,
                           FileChangeEpoch fileChangeEpoch,
                           ILanguageServerFacade server)
        {
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.fileChangeEpoch = fileChangeEpoch;
            this.server = server;
        }

        public void HandleBicepConfigOpenEvent(DocumentUri documentUri)
            => PublishConfigDiagnostics(documentUri);

        // Configuration is read from disk, so unsaved edits don't affect it; saving is reported as a file change.
        public void HandleBicepConfigChangeEvent(DocumentUri documentUri)
        {
        }

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            // The client may report the save before the watched-file event, so start a new epoch now to publish current diagnostics.
            // Compilations are refreshed when the watched-file event arrives.
            fileChangeEpoch.StartNew();
            PublishConfigDiagnostics(documentUri);
        }

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
        {
            // Clear squiggles when the file is closed.
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>()
            });
        }

        private void PublishConfigDiagnostics(DocumentUri documentUri)
        {
            var documentFileUri = documentUri.ToIOUri();
            var chain = bicepConfigurationManager.GetConfigurationChain(documentFileUri);
            var diagnosticsPerFile = chain.EnumerateDiagnosticsPerFile().ToList();

            // Publish for every file in the chain, including files without diagnostics, so the client clears stale squiggles.
            // Diagnostics that don't belong to a file (for example, an invalid environment variable) are shown in Bicep files instead.
            foreach (var (fileUri, diagnostics) in diagnosticsPerFile)
            {
                Publish(DocumentUri.From(fileUri.ToUri()), diagnostics);
            }

            // The opened document may not be part of the chain (for example, a base file opened on its own); clear its squiggles too.
            if (!diagnosticsPerFile.Any(entry => entry.Key.Equals(documentFileUri)))
            {
                Publish(documentUri, []);
            }
        }

        private void Publish(DocumentUri documentUri, IEnumerable<IDiagnostic> diagnostics)
            => server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>(diagnostics.Select(d => new LspDiagnostic
                {
                    Severity = ToLspSeverity(d.Level),
                    Code = new DiagnosticCode(d.Code),
                    Message = d.Message,
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 0),
                    Source = "bicep"
                }))
            });

        private static DiagnosticSeverity ToLspSeverity(DiagnosticLevel level) => level switch
        {
            DiagnosticLevel.Error => DiagnosticSeverity.Error,
            DiagnosticLevel.Warning => DiagnosticSeverity.Warning,
            DiagnosticLevel.Info => DiagnosticSeverity.Information,
            _ => DiagnosticSeverity.Hint
        };
    }
}
