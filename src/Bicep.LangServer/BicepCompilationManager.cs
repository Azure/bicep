// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer
{
    public class BicepCompilationManager : ICompilationManager
    {
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider provider;

        // represents compilations of open bicep files
        private readonly ConcurrentDictionary<DocumentUri, CompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, CompilationContext>();

        public BicepCompilationManager(ILanguageServerFacade server, ICompilationProvider provider)
        {
            this.server = server;
            this.provider = provider;
        }

        public CompilationContext? UpsertCompilation(DocumentUri uri, int? version, string text)
        {
            try
            {
                var context = this.provider.Create(uri, text);

                // there shouldn't be concurrent upsert requests (famous last words...), so a simple overwrite should be sufficient
                this.activeContexts[uri] = context;

                // convert all the diagnostics to LSP diagnostics
                var diagnostics = GetDiagnosticsFromContext(context).ToDiagnostics(context.LineStarts);

                // publish all the diagnostics
                this.PublishDocumentDiagnostics(uri, version, diagnostics);

                return context;
            }
            catch (Exception exception)
            {
                // this is a fatal error likely due to a code defect
                
                // the file is no longer in a state that can be parsed
                // clear all info to prevent cascading failures elsewhere
                this.activeContexts.TryRemove(uri, out _);

                // publish a single fatal error diagnostic to tell the user something horrible has occurred
                // TODO: Tell user how to create an issue on GitHub.
                this.PublishDocumentDiagnostics(uri, version, new[]
                {
                    new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic
                    {
                        Range = new Range
                        {
                            Start = new Position(0, 0),
                            End = new Position(1, 0),
                        },
                        Severity = DiagnosticSeverity.Error,
                        Message = exception.Message,
                        Code = new DiagnosticCode("Fatal")
                    }
                });

                return null;
            }
        }

        public void CloseCompilation(DocumentUri uri)
        {
            // remove the active compilation
            this.activeContexts.TryRemove(uri, out _);

            // clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            PublishDocumentDiagnostics(uri, 0, Enumerable.Empty<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic>());
        }

        public CompilationContext? GetCompilation(DocumentUri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        // TODO: Remove the lexer part when we stop it from emitting errors
        private IEnumerable<Core.Diagnostics.Diagnostic> GetDiagnosticsFromContext(CompilationContext context) => context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        private void PublishDocumentDiagnostics(DocumentUri uri, int? version, IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> diagnostics)
        {
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Version = version,
                Diagnostics = new Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic>(diagnostics)
            });
        }
    }
}
