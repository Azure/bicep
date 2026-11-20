// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.LanguageServer.Compilation;
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
        private readonly ICompilationManager compilationManager;
        private readonly IBicepConfigurationManager bicepConfigurationManager;
        private readonly ILanguageServerFacade server;

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           IBicepConfigurationManager bicepConfigurationManager,
                           ILanguageServerFacade server)
        {
            this.compilationManager = compilationManager;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.server = server;
        }

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents)
        {
            var invalidated = false;
            foreach (var fileEvent in fileEvents)
            {
                invalidated |= bicepConfigurationManager.InvalidateFile(fileEvent.Uri.ToIOUri());
            }

            if (invalidated)
            {
                // We shouldn't need to reload auxiliary files if a configuration file has changed.
                compilationManager.RefreshAllActiveCompilations(forceReloadAuxiliaryFiles: false);
            }
        }

        public void HandleBicepConfigOpenEvent(DocumentUri documentUri)
            => PublishConfigDiagnostics(documentUri);

        // Configuration is read from disk, so unsaved edits don't affect it; saving is reported as a file change.
        public void HandleBicepConfigChangeEvent(DocumentUri documentUri)
        {
        }

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            bicepConfigurationManager.InvalidateFile(documentUri.ToIOUri());
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
