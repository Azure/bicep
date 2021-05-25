// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
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
        private readonly IWorkspace workspace;
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider provider;

        // represents compilations of open bicep files
        private readonly ConcurrentDictionary<DocumentUri, CompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, CompilationContext>();

        public BicepCompilationManager(ILanguageServerFacade server, ICompilationProvider provider, IWorkspace workspace)
        {
            this.server = server;
            this.provider = provider;
            this.workspace = workspace;
        }

        public void UpsertCompilation(DocumentUri documentUri, int? version, string fileContents)
        {
            var newSyntaxTree = SyntaxTree.Create(documentUri.ToUri(), fileContents);
            var firstChanges = workspace.UpsertSyntaxTrees(newSyntaxTree.AsEnumerable());
            var secondChanges = UpdateCompilationInternal(documentUri, version);
            
            var addedTrees = firstChanges.added.Concat(secondChanges.added);
            var removedTrees = firstChanges.removed.Concat(secondChanges.removed);

            foreach (var (entrypointUri, context) in activeContexts)
            {
                if (removedTrees.Any(x => context.Compilation.SyntaxTreeGrouping.SyntaxTrees.Contains(x)))
                {
                    UpdateCompilationInternal(entrypointUri, null);
                }
            }
        }

        public void CloseCompilation(DocumentUri documentUri)
        {
            // close and clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            CloseCompilationInternal(documentUri, 0, Enumerable.Empty<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic>());
        }

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents)
        {
            var removedTrees = new HashSet<SyntaxTree>();
            foreach (var change in fileEvents.Where(x => x.Type == FileChangeType.Changed || x.Type == FileChangeType.Deleted))
            {
                if (activeContexts.ContainsKey(change.Uri))
                {
                    // We should expect an explicit request to update this file if it's open. Otherwise we may
                    // overwrite the 'dirty' copy of the file with a clean one.
                    continue;
                }

                // We treat both updates and deletes as 'removes' to force the new SyntaxTree to be reloaded from disk
                if (workspace.TryGetSyntaxTree(change.Uri.ToUri(), out var removedTree))
                {
                    removedTrees.Add(removedTree);
                }
                else if (change.Type == FileChangeType.Deleted)
                {
                    // If we don't know definitively that we're deleting a file, we have to assume it's a directory; the file system watcher does not give us any information to differentiate reliably.
                    // We could possibly assume that if the path ends in '.bicep', we've got a file, but this would discount directories ending in '.bicep', however unlikely.
                    var subdirRemovedTrees = workspace.GetSyntaxTreesForDirectory(change.Uri.ToUri());
                    removedTrees.UnionWith(subdirRemovedTrees);
                }
            }

            workspace.RemoveSyntaxTrees(removedTrees);
            foreach (var (entrypointUri, context) in activeContexts)
            {
                if (removedTrees.Any(x => context.Compilation.SyntaxTreeGrouping.SyntaxTrees.Contains(x)))
                {
                    UpdateCompilationInternal(entrypointUri, null);
                }
            }
        }

        public CompilationContext? GetCompilation(DocumentUri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        private ImmutableArray<SyntaxTree> CloseCompilationInternal(DocumentUri documentUri, int? version, IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> closingDiagnostics)
        {
            this.activeContexts.TryRemove(documentUri, out var removedContext);

            this.PublishDocumentDiagnostics(documentUri, version, closingDiagnostics);

            if (removedContext == null)
            {
                return ImmutableArray<SyntaxTree>.Empty;
            }

            var closedSyntaxTrees = removedContext.Compilation.SyntaxTreeGrouping.SyntaxTrees.ToHashSet();
            foreach (var (entrypointUri, context) in activeContexts)
            {
                closedSyntaxTrees.ExceptWith(context.Compilation.SyntaxTreeGrouping.SyntaxTrees);
            }

            workspace.RemoveSyntaxTrees(closedSyntaxTrees);

            return closedSyntaxTrees.ToImmutableArray();
        }

        private (ImmutableArray<SyntaxTree> added, ImmutableArray<SyntaxTree> removed) UpdateCompilationInternal(DocumentUri documentUri, int? version)
        {
            try
            {
                var context = this.provider.Create(workspace, documentUri);

                var output = workspace.UpsertSyntaxTrees(context.Compilation.SyntaxTreeGrouping.SyntaxTrees);

                // there shouldn't be concurrent upsert requests (famous last words...), so a simple overwrite should be sufficient
                this.activeContexts[documentUri] = context;

                // convert all the diagnostics to LSP diagnostics
                var diagnostics = GetDiagnosticsFromContext(context).ToDiagnostics(context.LineStarts);

                // publish all the diagnostics
                this.PublishDocumentDiagnostics(documentUri, version, diagnostics);

                return output;
            }
            catch (Exception exception)
            {
                // this is a fatal error likely due to a code defect

                // publish a single fatal error diagnostic to tell the user something horrible has occurred
                // TODO: Tell user how to create an issue on GitHub.
                var fatalError = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic
                {
                    Range = new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(1, 0),
                    },
                    Severity = DiagnosticSeverity.Error,
                    Message = exception.Message,
                    Code = new DiagnosticCode("Fatal")
                };
                
                // the file is no longer in a state that can be parsed
                // clear all info to prevent cascading failures elsewhere
                var closedTrees = CloseCompilationInternal(documentUri, version, fatalError.AsEnumerable());

                return (ImmutableArray<SyntaxTree>.Empty, closedTrees);
            }
        }

        // TODO: Remove the lexer part when we stop it from emitting errors
        private IEnumerable<Core.Diagnostics.IDiagnostic> GetDiagnosticsFromContext(CompilationContext context) => context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

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
