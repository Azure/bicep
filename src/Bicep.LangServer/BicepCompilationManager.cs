﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer
{
    public class BicepCompilationManager : ICompilationManager
    {
        private readonly ILanguageServer server;
        private readonly ICompilationProvider provider;

        // represents compilations of open bicep files
        private readonly ConcurrentDictionary<Uri, CompilationContext> activeContexts = new ConcurrentDictionary<Uri, CompilationContext>();

        public BicepCompilationManager(ILanguageServer server, ICompilationProvider provider)
        {
            this.server = server;
            this.provider = provider;
        }

        public CompilationContext? UpsertCompilation(Uri uri, long version, string text)
        {
            try
            {
                var context = this.provider.Create(text);

                // there shouldn't be concurrent upsert requests (famous last words...), so a simple overwrite should be sufficient
                this.activeContexts[uri] = context;

                // convert all the errors to LSP diagnostics
                var diagnostics = GetErrorsFromContext(context).ToDiagnostics(context.LineStarts);

                // publish all the errors
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
                    new Diagnostic
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

        public void CloseCompilation(Uri uri)
        {
            // remove the active compilation
            this.activeContexts.TryRemove(uri, out _);

            // clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            PublishDocumentDiagnostics(uri, 0, Enumerable.Empty<Diagnostic>());
        }

        public CompilationContext? GetCompilation(Uri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        // TODO: Remove the lexer part when we stop it from emitting errors
        private IEnumerable<Error> GetErrorsFromContext(CompilationContext context) => context.Compilation.GetSemanticModel().GetAllDiagnostics();

        private void PublishDocumentDiagnostics(Uri uri, long version, IEnumerable<Diagnostic> diagnostics)
        {
            server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Version = version,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            });
        }
    }
}