// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Telemetry;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using static Bicep.Core.Diagnostics.DisabledDiagnosticsCache;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer
{
    public class BicepCompilationManager : ICompilationManager
    {
        public const string LinterEnabledSetting = "core.enabled";

        private readonly IWorkspace workspace;
        private readonly ILanguageServerFacade server;
        private readonly ICompilationProvider provider;
        private readonly IFileResolver fileResolver;
        private readonly IModuleRestoreScheduler scheduler;
        private readonly IConfigurationManager configurationManager;
        private readonly ITelemetryProvider TelemetryProvider;
        private readonly ILinterRulesProvider LinterRulesProvider;
        private readonly IBicepAnalyzer bicepAnalyzer;

        // represents compilations of open bicep files
        private readonly ConcurrentDictionary<DocumentUri, CompilationContext> activeContexts = new ConcurrentDictionary<DocumentUri, CompilationContext>();

        public BicepCompilationManager(
            ILanguageServerFacade server,
            ICompilationProvider provider,
            IWorkspace workspace,
            IFileResolver fileResolver,
            IModuleRestoreScheduler scheduler,
            IConfigurationManager configurationManager,
            ITelemetryProvider telemetryProvider,
            ILinterRulesProvider LinterRulesProvider,
            IBicepAnalyzer bicepAnalyzer)
        {
            this.server = server;
            this.provider = provider;
            this.workspace = workspace;
            this.fileResolver = fileResolver;
            this.scheduler = scheduler;
            this.configurationManager = configurationManager;
            this.TelemetryProvider = telemetryProvider;
            this.LinterRulesProvider = LinterRulesProvider;
            this.bicepAnalyzer = bicepAnalyzer;
        }

        public void RefreshCompilation(DocumentUri documentUri)
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
                // method again. CompilationContext will be null. We'll get the souceFile from workspace and
                // upsert compulation.
                if (workspace.TryGetSourceFile(documentUri.ToUri(), out ISourceFile? sourceFile) && sourceFile is BicepFile)
                {
                    UpsertCompilationInternal(documentUri, null, sourceFile);
                }
                return;
            }

            // TODO: This may cause race condition if the user is modifying the file at the same time
            // need to make a shallow copy so it counts as a different file even though all the content is identical
            // this was the easiest way to force the compilation to be regenerated
            var shallowCopy = new BicepFile(compilationContext.Compilation.SourceFileGrouping.EntryPoint);
            UpsertCompilationInternal(documentUri, null, shallowCopy);
        }

        public void UpsertCompilation(DocumentUri documentUri, int? version, string fileContents, string? languageId = null, bool triggeredByFileOpenEvent = false)
        {
            if (this.ShouldUpsertCompilation(documentUri, languageId))
            {
                var newFile = SourceFileFactory.CreateSourceFile(documentUri.ToUri(), fileContents);
                UpsertCompilationInternal(documentUri, version, newFile, triggeredByFileOpenEvent: triggeredByFileOpenEvent);
            }
        }

        private void UpsertCompilationInternal(DocumentUri documentUri, int? version, ISourceFile newFile, bool triggeredByFileOpenEvent = false)
        {
            var (_, removedFiles) = workspace.UpsertSourceFile(newFile);

            var modelLookup = new Dictionary<ISourceFile, ISemanticModel>();
            if (newFile is BicepFile)
            {
                // Do not update compilation if it is an ARM template file, since it cannot be an entrypoint.
                UpdateCompilationInternal(documentUri, version, modelLookup, removedFiles, triggeredByFileOpenEvent);
            }

            foreach (var (entrypointUri, context) in activeContexts)
            {
                if (removedFiles.Any(x => context.Compilation.SourceFileGrouping.SourceFiles.Contains(x)))
                {
                    UpdateCompilationInternal(entrypointUri, null, modelLookup, removedFiles, triggeredByFileOpenEvent);
                }
            }
        }

        public void CloseCompilation(DocumentUri documentUri)
        {
            // close and clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            CloseCompilationInternal(documentUri, 0, Enumerable.Empty<Diagnostic>());
        }

        public void HandleFileChanges(IEnumerable<FileEvent> fileEvents)
        {
            var removedFiles = new HashSet<ISourceFile>();
            foreach (var change in fileEvents.Where(x => x.Type == FileChangeType.Changed || x.Type == FileChangeType.Deleted))
            {
                if (activeContexts.ContainsKey(change.Uri))
                {
                    // We should expect an explicit request to update this file if it's open. Otherwise we may
                    // overwrite the 'dirty' copy of the file with a clean one.
                    continue;
                }

                // We treat both updates and deletes as 'removes' to force the new SourceFile to be reloaded from disk
                if (workspace.TryGetSourceFile(change.Uri.ToUri(), out var removedFile))
                {
                    removedFiles.Add(removedFile);
                }
                else if (change.Type == FileChangeType.Deleted)
                {
                    // If we don't know definitively that we're deleting a file, we have to assume it's a directory; the file system watcher does not give us any information to differentiate reliably.
                    // We could possibly assume that if the path ends in '.bicep', we've got a file, but this would discount directories ending in '.bicep', however unlikely.
                    var subdirRemovedFiles = workspace.GetSourceFilesForDirectory(change.Uri.ToUri());
                    removedFiles.UnionWith(subdirRemovedFiles);
                }
            }

            workspace.RemoveSourceFiles(removedFiles);

            var modelLookup = new Dictionary<ISourceFile, ISemanticModel>();
            foreach (var (entrypointUri, context) in activeContexts)
            {
                if (removedFiles.Any(x => context.Compilation.SourceFileGrouping.SourceFiles.Contains(x)))
                {
                    UpdateCompilationInternal(entrypointUri, null, modelLookup, removedFiles);
                }
            }
        }

        public CompilationContext? GetCompilation(DocumentUri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        private bool ShouldUpsertCompilation(DocumentUri documentUri, string? languageId = null)
        {
            // We should only upsert compilation when languageId is bicep or the file is already tracked in workspace.
            // When the file is in workspace but languageId is null, the file can be a bicep file or a JSON template
            // being referenced as a bicep module.
            return string.Equals(languageId, LanguageConstants.LanguageId, StringComparison.OrdinalIgnoreCase) ||
                this.workspace.TryGetSourceFile(documentUri.ToUri(), out var _);
        }

        private ImmutableArray<ISourceFile> CloseCompilationInternal(DocumentUri documentUri, int? version, IEnumerable<Diagnostic> closingDiagnostics)
        {
            this.activeContexts.TryRemove(documentUri, out var removedContext);

            this.PublishDocumentDiagnostics(documentUri, version, closingDiagnostics);

            if (removedContext == null)
            {
                return ImmutableArray<ISourceFile>.Empty;
            }

            var closedFiles = removedContext.Compilation.SourceFileGrouping.SourceFiles.ToHashSet();
            foreach (var (_, context) in activeContexts)
            {
                closedFiles.ExceptWith(context.Compilation.SourceFileGrouping.SourceFiles);
            }

            workspace.RemoveSourceFiles(closedFiles);

            return closedFiles.ToImmutableArray();
        }

        private (ImmutableArray<ISourceFile> added, ImmutableArray<ISourceFile> removed) UpdateCompilationInternal(DocumentUri documentUri, int? version, IDictionary<ISourceFile, ISemanticModel> modelLookup, IEnumerable<ISourceFile> removedFiles, bool triggeredByFileOpenEvent = false)
        {
            try
            {
                var context = this.activeContexts.AddOrUpdate(
                    documentUri,
                    (documentUri) => this.provider.Create(workspace, documentUri, modelLookup.ToImmutableDictionary(), bicepAnalyzer),
                    (documentUri, prevContext) =>
                    {
                        var sourceDependencies = removedFiles
                            .SelectMany(x => prevContext.Compilation.SourceFileGrouping.GetFilesDependingOn(x))
                            .ToImmutableHashSet();

                        // check for semantic models that we can safely reuse from the previous compilation
                        foreach (var sourceFile in prevContext.Compilation.SourceFileGrouping.SourceFiles)
                        {
                            if (!modelLookup.ContainsKey(sourceFile) && !sourceDependencies.Contains(sourceFile))
                            {
                                // if we have a file with no dependencies on the modified file(s), we can reuse the previous model
                                modelLookup[sourceFile] = prevContext.Compilation.GetSemanticModel(sourceFile);
                            }
                        }

                        return this.provider.Create(workspace, documentUri, modelLookup.ToImmutableDictionary(), bicepAnalyzer);
                    });

                foreach (var sourceFile in context.Compilation.SourceFileGrouping.SourceFiles)
                {
                    // store all the updated models as other compilations may be able to reuse them
                    modelLookup[sourceFile] = context.Compilation.GetSemanticModel(sourceFile);
                }

                // this completes immediately
                this.scheduler.RequestModuleRestore(this, documentUri, context.Compilation.SourceFileGrouping.GetModulesToRestore());

                var sourceFiles = context.Compilation.SourceFileGrouping.SourceFiles;
                var output = workspace.UpsertSourceFiles(sourceFiles);

                // convert all the diagnostics to LSP diagnostics
                var diagnostics = GetDiagnosticsFromContext(context).ToDiagnostics(context.LineStarts);

                if (triggeredByFileOpenEvent)
                {
                    var model = context.Compilation.GetEntrypointSemanticModel();
                    SendTelemetryOnBicepFileOpen(model, documentUri.ToUri(), model.Configuration, sourceFiles, diagnostics);
                }

                // publish all the diagnostics
                this.PublishDocumentDiagnostics(documentUri, version, diagnostics);

                return output;
            }
            catch (Exception exception)
            {
                // this is a fatal error likely due to a code defect

                // publish a single fatal error diagnostic to tell the user something horrible has occurred
                // TODO: Tell user how to create an issue on GitHub.
                var fatalError = new Diagnostic
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

                this.PublishDocumentDiagnostics(documentUri, version, fatalError.AsEnumerable());

                return (ImmutableArray<ISourceFile>.Empty, ImmutableArray<ISourceFile>.Empty);
            }
        }

        private void SendTelemetryOnBicepFileOpen(SemanticModel semanticModel, DocumentUri documentUri, RootConfiguration configuration, IEnumerable<ISourceFile> sourceFiles, IEnumerable<Diagnostic> diagnostics)
        {
            // Telemetry on linter state on bicep file open
            var telemetryEvent = GetLinterStateTelemetryOnBicepFileOpen(configuration);
            TelemetryProvider.PostEvent(telemetryEvent);

            // Telemetry on open bicep file and the referenced modules
            telemetryEvent = GetTelemetryAboutSourceFiles(semanticModel, documentUri.ToUri(), sourceFiles, diagnostics);

            if (telemetryEvent is not null)
            {
                TelemetryProvider.PostEvent(telemetryEvent);
            }
        }

        public BicepTelemetryEvent? GetTelemetryAboutSourceFiles(SemanticModel semanticModel, Uri uri, IEnumerable<ISourceFile> sourceFiles, IEnumerable<Diagnostic> diagnostics)
        {
            var mainFile = sourceFiles.First(x => x.FileUri == uri) as BicepFile;

            if (mainFile is null)
            {
                return null;
            }

            Dictionary<string, string> properties = GetTelemetryPropertiesForMainFile(semanticModel, mainFile, diagnostics);

            var referencedFiles = sourceFiles.Where(x => x.FileUri != uri);
            var propertiesFromReferencedFiles = GetTelemetryPropertiesForReferencedFiles(referencedFiles);

            properties = properties.Concat(propertiesFromReferencedFiles).ToDictionary(s => s.Key, s => s.Value);

            return BicepTelemetryEvent.CreateBicepFileOpen(properties);
        }

        private Dictionary<string, string> GetTelemetryPropertiesForMainFile(SemanticModel sematicModel, BicepFile bicepFile, IEnumerable<Diagnostic> diagnostics)
        {
            Dictionary<string, string> properties = new();

            var declarationsInMainFile = bicepFile.ProgramSyntax.Declarations;
            properties.Add("Modules", declarationsInMainFile.Count(x => x is ModuleDeclarationSyntax).ToString());
            properties.Add("Parameters", declarationsInMainFile.Count(x => x is ParameterDeclarationSyntax).ToString());
            properties.Add("Resources", sematicModel.DeclaredResources.Length.ToString());
            properties.Add("Variables", declarationsInMainFile.Count(x => x is VariableDeclarationSyntax).ToString());

            var localPath = bicepFile.FileUri.LocalPath;

            try
            {
                if (File.Exists(localPath))
                {
                    var fileInfo = new FileInfo(bicepFile.FileUri.LocalPath);
                    properties.Add("FileSizeInBytes", fileInfo.Length.ToString());
                }
            }
            catch (Exception)
            {
                // We should not throw in this case since it will block compilation.
                properties.Add("FileSizeInBytes", string.Empty);
            }

            properties.Add("LineCount", bicepFile.LineStarts.Length.ToString());
            properties.Add("Errors", diagnostics.Count(x => x.Severity == DiagnosticSeverity.Error).ToString());
            properties.Add("Warnings", diagnostics.Count(x => x.Severity == DiagnosticSeverity.Warning).ToString());

            var disableNextLineDirectiveEndPositionAndCodes = bicepFile.DisabledDiagnosticsCache.GetDisableNextLineDiagnosticDirectivesCache().Values;
            properties.Add("DisableNextLineCount", disableNextLineDirectiveEndPositionAndCodes.Count().ToString());
            properties.Add("DisableNextLineCodes", GetDiagnosticCodesWithCount(disableNextLineDirectiveEndPositionAndCodes));

            return properties;
        }

        private string GetDiagnosticCodesWithCount(IEnumerable<DisableNextLineDirectiveEndPositionAndCodes> disableNextLineDirectiveEndPositionAndCodes)
        {
            var diagnosticsCodesMap = new Dictionary<string, int>();

            foreach (var disableNextLineDirectiveEndPositionAndCode in disableNextLineDirectiveEndPositionAndCodes)
            {
                var diagnosticCodes = disableNextLineDirectiveEndPositionAndCode.diagnosticCodes.Distinct();
                foreach (string diagnosticCode in diagnosticCodes)
                {
                    if (diagnosticsCodesMap.ContainsKey(diagnosticCode))
                    {
                        diagnosticsCodesMap[diagnosticCode] += 1;
                    }
                    else
                    {
                        diagnosticsCodesMap.Add(diagnosticCode, 1);
                    }
                }
            }

            return JsonConvert.SerializeObject(diagnosticsCodesMap);
        }

        private Dictionary<string, string> GetTelemetryPropertiesForReferencedFiles(IEnumerable<ISourceFile> sourceFiles)
        {
            Dictionary<string, string> properties = new();
            int modules = 0;
            int parameters = 0;
            int resources = 0;
            int variables = 0;
            int lineCount = 0;
            int disableNextLineDirectivesCount = 0;
            List<DisableNextLineDirectiveEndPositionAndCodes> disableNextLineDirectiveEndPositionAndCodesInReferencedFiles = new List<DisableNextLineDirectiveEndPositionAndCodes>();

            foreach (var sourceFile in sourceFiles)
            {
                if (sourceFile is BicepFile bicepFile)
                {
                    var declarations = bicepFile.ProgramSyntax.Declarations;
                    modules += declarations.Count(x => x is ModuleDeclarationSyntax);
                    parameters += declarations.Count(x => x is ParameterDeclarationSyntax);
                    resources += declarations.Count(x => x is ResourceDeclarationSyntax);
                    variables += declarations.Count(x => x is VariableDeclarationSyntax);
                    lineCount += bicepFile.LineStarts.Length;

                    var disableNextLineDirectiveEndPositionAndCodes = bicepFile.DisabledDiagnosticsCache.GetDisableNextLineDiagnosticDirectivesCache().Values;
                    disableNextLineDirectivesCount += disableNextLineDirectiveEndPositionAndCodes.Count();
                    disableNextLineDirectiveEndPositionAndCodesInReferencedFiles.AddRange(disableNextLineDirectiveEndPositionAndCodes);
                }
            }

            properties.Add("ModulesInReferencedFiles", modules.ToString());
            properties.Add("ParentResourcesInReferencedFiles", resources.ToString());
            properties.Add("ParametersInReferencedFiles", parameters.ToString());
            properties.Add("VariablesInReferencedFiles", variables.ToString());
            properties.Add("LineCountOfReferencedFiles", lineCount.ToString());

            properties.Add("DisableNextLineCountInReferencedFiles", disableNextLineDirectivesCount.ToString());
            properties.Add("DisableNextLineCodesInReferencedFiles", GetDiagnosticCodesWithCount(disableNextLineDirectiveEndPositionAndCodesInReferencedFiles));

            return properties;
        }

        public BicepTelemetryEvent GetLinterStateTelemetryOnBicepFileOpen(RootConfiguration configuration)
        {
            bool linterEnabledSettingValue = configuration.Analyzers.GetValue(LinterEnabledSetting, true);
            Dictionary<string, string> properties = new();

            properties.Add("enabled", linterEnabledSettingValue.ToString().ToLowerInvariant());

            if (linterEnabledSettingValue)
            {
                foreach (var kvp in LinterRulesProvider.GetLinterRules())
                {
                    string linterRuleDiagnosticLevelValue = configuration.Analyzers.GetValue(kvp.Value, "warning");

                    properties.Add(kvp.Key, linterRuleDiagnosticLevelValue);
                }
            }

            return BicepTelemetryEvent.CreateLinterStateOnBicepFileOpen(properties);
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
