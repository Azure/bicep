// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.ModuleRestore;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Compilation
{
    public class BicepCompilationManager : ICompilationManager
    {
        private readonly IActiveSourceFileSet activeSourceFileSet;
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider provider;
        private readonly IModuleRestoreScheduler scheduler;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly IAuxiliaryFileCache auxiliaryfileCache;

        // represents compilations of open bicep or param files
        private readonly ConcurrentDictionary<DocumentUri, CompilationContextBase> activeContexts = new();

        public BicepCompilationManager(
            ILanguageServerFacade server,
            ICompilationProvider provider,
            IActiveSourceFileSet activeSourceFileSet,
            IModuleRestoreScheduler scheduler,
            ISourceFileFactory sourceFileFactory,
            IAuxiliaryFileCache auxiliaryFileCache)
        {
            this.server = server;
            this.provider = provider;
            this.activeSourceFileSet = activeSourceFileSet;
            this.scheduler = scheduler;
            this.sourceFileFactory = sourceFileFactory;
            this.auxiliaryfileCache = auxiliaryFileCache;
        }

        public void RefreshCompilation(DocumentUri documentUri, bool forceReloadAuxiliaryFiles)
        {
            var compilationContext = this.GetCompilation(documentUri);

            if (compilationContext is null)
            {
                // This check handles the scenario when bicepconfig.json was updated, but we
                // couldn't find an entry for the documentUri in activeContexts.
                // This can happen if bicepconfig.json file was previously invalid, in which case
                // we wouldn't have upserted compilation. This is intentional as it's not possible to
                // compute diagnostics till errors in bicepconfig.json are fixed.
                // When errors are fixed in bicepconfig.json and file is saved, we'll get called into this
                // method again. CompilationContext will be null. We'll get the sourceFile from workspace and
                // upsert compilation.
                if (activeSourceFileSet.TryGetSourceFile(documentUri.ToIOUri()) is BicepFile sourceFile)
                {
                    UpsertCompilationInternal(documentUri, null, sourceFile);
                }

                return;
            }

            if (forceReloadAuxiliaryFiles)
            {
                var auxiliaryFileUris = compilationContext.Compilation.SourceFileGrouping.GetAllReferencedAuxiliaryFileUris();
                this.auxiliaryfileCache.Trim(auxiliaryFileUris);
            }

            // TODO: This may cause race condition if the user is modifying the file at the same time
            // need to make a shallow copy so it counts as a different file even though all the content is identical
            // this was the easiest way to force the compilation to be regenerated
            var shallowCopy = compilationContext.Compilation.SourceFileGrouping.EntryPoint.ShallowClone();
            UpsertCompilationInternal(documentUri, null, shallowCopy);
        }

        public void RefreshAllActiveCompilations(bool forceReloadAuxiliaryFiles)
        {
            foreach (var sourceFile in activeSourceFileSet)
            {
                RefreshCompilation(sourceFile.FileHandle.Uri.ToDocumentUri(), forceReloadAuxiliaryFiles);
            }
        }

        public void OpenCompilation(DocumentUri documentUri, int? version, string fileContents, string languageId)
        {
            if (this.ShouldUpsertCompilation(documentUri, languageId, out var sourceFileType))
            {
                var newFile = this.sourceFileFactory.CreateSourceFile(documentUri.ToIOUri(), fileContents, sourceFileType);
                UpsertCompilationInternal(documentUri, version, newFile);
            }
        }

        public void UpdateCompilation(DocumentUri documentUri, int? version, string fileContents)
        {
            if (this.ShouldUpsertCompilation(documentUri, languageId: null, out var sourceFileType))
            {
                var newFile = this.sourceFileFactory.CreateSourceFile(documentUri.ToIOUri(), fileContents, sourceFileType);
                UpsertCompilationInternal(documentUri, version, newFile);
            }
        }

        private void UpsertCompilationInternal(DocumentUri documentUri, int? version, ISourceFile newFile, bool clearAuxiliaryFileCache = false)
        {
            var (_, removedFiles) = activeSourceFileSet.UpsertSourceFile(newFile);

            var modelLookup = new Dictionary<ISourceFile, ISemanticModel>();
            if (newFile is BicepSourceFile)
            {
                // Do not update compilation if it is an ARM template file, since it cannot be an entrypoint.
                UpdateCompilationInternal(documentUri, version, modelLookup, removedFiles);
            }

            foreach (var (entrypointUri, context) in GetAllSafeActiveContexts())
            {
                // we may see an unsafe context if there was a fatal exception
                if (removedFiles.Any(x => context.Compilation.SourceFileGrouping.SourceFiles.Contains(x)))
                {
                    UpdateCompilationInternal(entrypointUri, null, modelLookup, removedFiles);
                }
            }

            var activeAuxiliaryFileUris = GetAllSafeActiveContexts().SelectMany(x => x.Value.Compilation.SourceFileGrouping.GetAllReferencedAuxiliaryFileUris());
            var inactiveAuxliaryFileUris = auxiliaryfileCache.Keys.Except(activeAuxiliaryFileUris);

            auxiliaryfileCache.Trim(inactiveAuxliaryFileUris);
        }

        public void CloseCompilation(DocumentUri documentUri)
        {
            // close and clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            CloseCompilationInternal(documentUri, 0, []);
        }

        public void RefreshChangedFiles(IEnumerable<DocumentUri> files)
            => HandleFileChanges(files.Select(uri => new FileEvent
            {
                Uri = uri,
                Type = FileChangeType.Changed,
            }));

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents)
        {
            var modifiedSourceFiles = new HashSet<ISourceFile>();
            var modifiedAuxiliaryFileUris = new HashSet<IOUri>();
            var activeAuxiliaryFileUris = auxiliaryfileCache.Keys;

            foreach (var change in fileEvents.Where(x => x.Type is FileChangeType.Changed or FileChangeType.Deleted))
            {
                // There are cases where we may get large numbers of file events in one go (e.g. if the user checks out a git commit).
                // We should bear that in mind when making changes here, to avoid introducing performance bottlenecks.

                var changedFileUri = change.Uri.ToIOUri();
                if (change.Type is FileChangeType.Deleted)
                {
                    // If we don't know definitively that we're deleting a file, we have to assume it's a directory; the file system watcher does not give us any information to differentiate reliably.
                    // We could possibly assume that if the path ends in '.bicep', we've got a file, but this would discount directories ending in '.bicep', however unlikely.
                    var removedDirectoryUri = changedFileUri.Path.EndsWith('/')
                        ? changedFileUri
                        : changedFileUri.WithPath(changedFileUri.Path + '/');

                    var removedSourceFiles = activeSourceFileSet.Where(x => removedDirectoryUri.IsBaseOf(x.FileHandle.Uri));
                    modifiedSourceFiles.UnionWith(removedSourceFiles);

                    var removedAuxiliaryFileUris = activeAuxiliaryFileUris.Where(removedDirectoryUri.IsBaseOf);
                    modifiedAuxiliaryFileUris.UnionWith(removedAuxiliaryFileUris);
                }

                if (activeAuxiliaryFileUris.Contains(changedFileUri))
                {
                    modifiedAuxiliaryFileUris.Add(changedFileUri);
                }

                if (!activeContexts.ContainsKey(change.Uri) &&
                    activeSourceFileSet.TryGetSourceFile(changedFileUri) is { } modifiedSourceFile)
                {
                    // If a file is active in the editor, we will get an explicit textDocument/did* request to update it.
                    // We deliberately avoid clearing the workspace for active files to avoid a race condition.
                    modifiedSourceFiles.Add(modifiedSourceFile);
                }
            }

            activeSourceFileSet.RemoveSourceFiles(modifiedSourceFiles);
            auxiliaryfileCache.Trim(modifiedAuxiliaryFileUris);

            var modelLookup = new Dictionary<ISourceFile, ISemanticModel>();
            foreach (var (entrypointUri, context) in GetAllSafeActiveContexts())
            {
                foreach (var sourceFile in context.Compilation.SourceFileGrouping.EnumerateBicepSourceFiles())
                {
                    if (modifiedAuxiliaryFileUris.Any(sourceFile.IsReferencingAuxiliaryFile))
                    {
                        // Ensure we refresh any source files that reference a modified auxiliary file
                        modifiedSourceFiles.Add(sourceFile);
                    }
                }

                if (context.Compilation.SourceFileGrouping.SourceFiles.Any(modifiedSourceFiles.Contains))
                {
                    UpdateCompilationInternal(entrypointUri, null, modelLookup, modifiedSourceFiles);
                }
            }
        }

        public CompilationContext? GetCompilation(DocumentUri uri) => this.activeContexts.GetValueOrDefault(uri) as CompilationContext;

        private IEnumerable<KeyValuePair<DocumentUri, CompilationContext>> GetAllSafeActiveContexts() =>
            this.activeContexts
                .Where(pair => pair.Value is CompilationContext)
                .Select(pair => new KeyValuePair<DocumentUri, CompilationContext>(pair.Key, (CompilationContext)pair.Value));

        private bool ShouldUpsertCompilation(DocumentUri documentUri, string? languageId, [NotNullWhen(true)] out Type? sourceFileType)
        {
            if (documentUri.Scheme == LangServerConstants.ExternalSourceFileScheme)
            {
                // Don't compile source code from external modules (and therefore also don't show compiler/linter warnings etc.)
                sourceFileType = null;
                return false;
            }

            // We should only upsert compilation when languageId is bicep or the file is already tracked in workspace.
            // When the file is in workspace but languageId is null, the file can be a bicep file or a JSON template
            // being referenced as a bicep module.
            if (LanguageConstants.IsBicepLanguage(languageId))
            {
                sourceFileType = typeof(BicepFile);
                return true;
            }

            if (LanguageConstants.IsParamsLanguage(languageId))
            {
                sourceFileType = typeof(BicepParamFile);
                return true;
            }

            if (this.activeSourceFileSet.TryGetSourceFile(documentUri.ToIOUri()) is { } sourceFile)
            {
                sourceFileType = sourceFile.GetType();
                return true;
            }

            sourceFileType = null;
            return false;
        }

        private ImmutableArray<ISourceFile> CloseCompilationInternal(DocumentUri documentUri, int? version, IEnumerable<Diagnostic> closingDiagnostics)
        {
            this.activeContexts.TryRemove(documentUri, out var removedPotentiallyUnsafeContext);

            this.PublishDocumentDiagnostics(documentUri, version, closingDiagnostics);

            if (removedPotentiallyUnsafeContext is not CompilationContext removedContext)
            {
                return [];
            }

            var closedFiles = removedContext.Compilation.SourceFileGrouping.SourceFiles.ToHashSet();
            foreach (var (_, context) in GetAllSafeActiveContexts())
            {
                closedFiles.ExceptWith(context.Compilation.SourceFileGrouping.SourceFiles);
            }

            activeSourceFileSet.RemoveSourceFiles(closedFiles);

            return [.. closedFiles];
        }

        private CompilationContextBase CreateCompilationContext(IActiveSourceFileSet workspace, DocumentUri documentUri, ImmutableDictionary<ISourceFile, ISemanticModel> modelLookup)
        {
            try
            {
                return this.provider.Create(workspace, documentUri, modelLookup);
            }
            catch (Exception exception)
            {
                if (workspace.TryGetSourceFile(documentUri.ToIOUri()) is not { } sourceFile)
                {
                    // the document is somehow missing from the workspace,
                    // which should not happen since we upsert into the workspace before creating the compilation
                    throw new InvalidOperationException($"Unable to create unsafe compilation context because file '{documentUri}' is missing from the workspace.");
                }

                var fileKind = sourceFile is BicepSourceFile bicepSourceFile
                    ? bicepSourceFile.FileKind
                    : (BicepSourceFileKind?)null;

                return new UnsafeCompilationContext(exception, fileKind);
            }
        }

        private (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpdateCompilationInternal(
            DocumentUri documentUri,
            int? version,
            IDictionary<ISourceFile, ISemanticModel> modelLookup,
            IEnumerable<ISourceFile> removedFiles)
        {
            static IEnumerable<Diagnostic> CreateFatalDiagnostics(Exception exception) => new Diagnostic
            {
                Range = new Range
                {
                    Start = new Position(0, 0),
                    End = new Position(1, 0),
                },
                Severity = DiagnosticSeverity.Error,
                Message = exception.Message,
                Code = new DiagnosticCode("Fatal")
            }.AsEnumerable();

            try
            {
                var potentiallyUnsafeContext = this.activeContexts.AddOrUpdate(
                    documentUri,
                    (documentUri) => CreateCompilationContext(activeSourceFileSet, documentUri, modelLookup.ToImmutableDictionary()),
                    (documentUri, prevPotentiallyUnsafeContext) =>
                    {
                        if (prevPotentiallyUnsafeContext is CompilationContext prevContext)
                        {
                            var sourceDependencies = removedFiles
                                .SelectMany(x => prevContext.Compilation.SourceFileGrouping.GetSourceFilesDependingOn(x))
                                .ToFrozenSet();

                            // check for semantic models that we can safely reuse from the previous compilation
                            foreach (var sourceFile in prevContext.Compilation.SourceFileGrouping.SourceFiles)
                            {
                                if (!modelLookup.ContainsKey(sourceFile) && !sourceDependencies.Contains(sourceFile))
                                {
                                    // if we have a file with no dependencies on the modified file(s), we can reuse the previous model
                                    modelLookup[sourceFile] = prevContext.Compilation.GetSemanticModel(sourceFile);
                                }
                            }
                        }

                        return CreateCompilationContext(activeSourceFileSet, documentUri, modelLookup.ToImmutableDictionary());
                    });

                switch (potentiallyUnsafeContext)
                {
                    case CompilationContext context:
                        foreach (var sourceFile in context.Compilation.SourceFileGrouping.SourceFiles)
                        {
                            // store all the updated models as other compilations may be able to reuse them
                            modelLookup[sourceFile] = context.Compilation.GetSemanticModel(sourceFile);
                        }

                        // this completes immediately
                        var artifactsToRestore = ArtifactHelper.GetValidArtifactReferences(context.Compilation.SourceFileGrouping.GetArtifactsToRestore());
                        this.scheduler.RequestModuleRestore(this, documentUri, artifactsToRestore);

                        var sourceFiles = context.Compilation.SourceFileGrouping.SourceFiles;
                        var output = activeSourceFileSet.UpsertSourceFiles(sourceFiles);

                        // convert all the diagnostics to LSP diagnostics
                        var diagnostics = GetDiagnosticsFromContext(context).ToDiagnostics(context.LineStarts);

                        // publish all the diagnostics
                        this.PublishDocumentDiagnostics(documentUri, version, diagnostics);

                        return output;

                    case UnsafeCompilationContext unsafeContext:
                        // fatal error due to unhandled exception (code defect) while creating the compilation
                        // publish a single fatal error diagnostic to tell the user something horrible has occurred
                        // TODO: Tell user how to create an issue on GitHub.
                        this.PublishDocumentDiagnostics(documentUri, version, CreateFatalDiagnostics(unsafeContext.Exception));

                        return (ImmutableArray<ISourceFile>.Empty, ImmutableArray<ISourceFile>.Empty);

                    default:
                        throw new NotImplementedException($"Unexpected compilation context type '{potentiallyUnsafeContext.GetType().Name}'.");
                }
            }
            catch (Exception exception)
            {
                // this is a fatal error due to a code defect in the compilation upsert logic
                // handling the exception here allows the language server to recover by user typing more text
                // publish a single fatal error diagnostic to tell the user something horrible has occurred
                // TODO: Tell user how to create an issue on GitHub.
                this.PublishDocumentDiagnostics(documentUri, version, CreateFatalDiagnostics(exception));

                return (ImmutableArray<ISourceFile>.Empty, ImmutableArray<ISourceFile>.Empty);
            }
        }

        private static IEnumerable<Core.Diagnostics.IDiagnostic> GetDiagnosticsFromContext(CompilationContext context) =>
            context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

        private void PublishDocumentDiagnostics(DocumentUri uri, int? version, IEnumerable<Diagnostic> diagnostics)
        {
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Version = version,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            });
        }
    }
}
