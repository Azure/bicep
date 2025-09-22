// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.SourceGraph
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileExplorer fileExplorer;
        private readonly IModuleDispatcher dispatcher;
        private readonly IActiveSourceFileLookup workspace;
        private readonly ISourceFileFactory sourceFileFactory;

        private readonly Dictionary<IOUri, ResultWithDiagnosticBuilder<ISourceFile>> fileResultByUri;
        private readonly Dictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> artifactLookup;
        private readonly Dictionary<ISourceFile, HashSet<ImplicitExtension>> implicitExtensions;
        private readonly bool forceRestore;

        private SourceFileGroupingBuilder(
            IFileExplorer fileExplorer,
            IModuleDispatcher moduleDispatcher,
            IActiveSourceFileLookup workspace,
            ISourceFileFactory sourceFileFactory,
            bool forceModulesRestore = false)
        {
            this.fileExplorer = fileExplorer;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.sourceFileFactory = sourceFileFactory;
            this.artifactLookup = [];
            this.implicitExtensions = [];
            this.fileResultByUri = [];
            this.forceRestore = forceModulesRestore;
        }

        private SourceFileGroupingBuilder(
            IFileExplorer fileExplorer,
            IModuleDispatcher moduleDispatcher,
            IActiveSourceFileLookup workspace,
            ISourceFileFactory sourceFileFactory,
            SourceFileGrouping current,
            bool forceArtifactRestore = false)
        {
            this.fileExplorer = fileExplorer;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.sourceFileFactory = sourceFileFactory;
            this.artifactLookup = current.ArtifactLookup.Where(x => x.Value.Result.IsSuccess()).ToDictionary();
            this.implicitExtensions = current.ImplicitExtensions.ToDictionary(x => x.Key, x => x.Value.ToHashSet());
            this.fileResultByUri = current.SourceFileLookup.ToDictionary();
            this.forceRestore = forceArtifactRestore;
        }

        public static SourceFileGrouping Build(IFileExplorer fileExplorer, IModuleDispatcher moduleDispatcher, IActiveSourceFileLookup workspace, ISourceFileFactory sourceFileFactory, IOUri entryFileUri, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileExplorer, moduleDispatcher, workspace, sourceFileFactory, forceModulesRestore);

            return builder.Build(entryFileUri);
        }

        public static SourceFileGrouping Rebuild(IFileExplorer fileExplorer, IModuleDispatcher moduleDispatcher, IActiveSourceFileLookup workspace, ISourceFileFactory sourceFileFactory, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(fileExplorer, moduleDispatcher, workspace, sourceFileFactory, current);

            var sourceFilesRequiringRestore = new HashSet<ISourceFile>();
            foreach (var (syntax, artifact) in current.ArtifactLookup.Where(x => SourceFileGrouping.ShouldRestore(x.Value)))
            {
                builder.artifactLookup.Remove(syntax);
                sourceFilesRequiringRestore.Add(artifact.ReferencingFile);
            }

            foreach (var (file, extensions) in current.ImplicitExtensions)
            {
                foreach (var extension in extensions.Where(x => x.Artifact is { } artifact && SourceFileGrouping.ShouldRestore(artifact)))
                {
                    builder.implicitExtensions[file].Remove(extension);
                    sourceFilesRequiringRestore.Add(file);
                }
            }

            // Rebuild source files that contain external artifact references restored during the initial build.
            var sourceFileExplorerBuild = sourceFilesRequiringRestore
                .SelectMany(current.GetSourceFilesDependingOn)
                .ToFrozenSet();

            return builder.Build(current.EntryPoint.FileHandle.Uri, sourceFileExplorerBuild);
        }

        private SourceFileGrouping Build(IOUri entryFileUri, FrozenSet<ISourceFile>? sourceFileExplorerBuild = null)
        {
            var fileResult = this.PopulateRecursive(entryFileUri, null, sourceFileExplorerBuild);

            if (!fileResult.IsSuccess(out var entryFile, out var errorBuilder))
            {
                var diagnostic = errorBuilder(ForDocumentStart());

                throw new DiagnosticException(diagnostic);
            }

            if (entryFile is not BicepSourceFile bicepSourceFile)
            {
                throw new InvalidOperationException($"Unexpected entry source file {entryFile.FileHandle.Uri}");
            }

            var sourceFileGraph = this.ReportFailuresForCycles();

            return new(
                bicepSourceFile,
                [.. fileResultByUri.Values.Select(x => x.TryUnwrap()).WhereNotNull()],
                sourceFileGraph.InvertLookup().ToImmutableDictionary(),
                artifactLookup.ToImmutableDictionary(),
                implicitExtensions.ToImmutableDictionary(x => x.Key, x => x.Value.ToImmutableHashSet()),
                fileResultByUri.ToImmutableDictionary());
        }

        private ResultWithDiagnosticBuilder<ISourceFile> GetFileResolutionResult(IOUri fileUri, ArtifactReference? moduleReference)
        {
            if (workspace.TryGetSourceFile(fileUri) is { } sourceFile)
            {
                return new(sourceFile);
            }

            if (!this.fileExplorer.GetFile(fileUri).TryReadAllText().IsSuccess(out var fileContents, out var failureBuilder))
            {
                return new(failureBuilder);
            }


            sourceFile = moduleReference is TemplateSpecModuleReference
                ? this.sourceFileFactory.CreateTemplateSpecFile(fileUri, fileContents)
                : this.sourceFileFactory.CreateSourceFile(fileUri, fileContents);

            return new(sourceFile);
        }

        private ResultWithDiagnosticBuilder<ISourceFile> GetFileResolutionResultWithCaching(IOUri fileUri, ArtifactReference? reference)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, reference);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private ResultWithDiagnosticBuilder<ISourceFile> PopulateRecursive(IOUri fileUri, ArtifactReference? reference, FrozenSet<ISourceFile>? sourceFileExplorerBuild)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, reference);
            if (fileResult.TryUnwrap() is BicepSourceFile bicepSource)
            {
                PopulateRecursive(bicepSource, sourceFileExplorerBuild);
            }

            return fileResult;
        }

        private void PopulateRecursive(BicepSourceFile file, FrozenSet<ISourceFile>? sourceFileExplorerBuild)
        {
            var config = file.Configuration;
            implicitExtensions[file] = [];

            // process "implicit" extensions (extensions defined in bicepconfig.json)
            foreach (var extensionName in config.ImplicitExtensions.GetImplicitExtensionNames())
            {
                var implicitExtension = GetImplicitExtension(extensionName, file, config);
                implicitExtensions[file].Add(implicitExtension);
            }

            // process all artifact references
            foreach (var restorable in GetArtifactReferences(file.ProgramSyntax))
            {
                if (restorable.Path is NoneLiteralSyntax)
                {
                    continue;
                }

                if (restorable is ExtensionDeclarationSyntax extensionDeclaration)
                {
                    var isBuiltInExtension = extensionDeclaration.SpecificationString switch
                    {
                        IdentifierSyntax identifier => config.Extensions.IsSysOrBuiltIn(identifier.IdentifierName),
                        _ => false,
                    };

                    if (isBuiltInExtension)
                    {
                        // built-in extension - no restoration required
                        continue;
                    }

                    artifactLookup[restorable] = GetArtifactRestoreResult(file, restorable);

                    // recursion not needed for extension declarations
                    continue;
                }

                var resolutionInfo = GetArtifactRestoreResult(file, restorable);
                artifactLookup[restorable] = resolutionInfo;
                if (!resolutionInfo.Result.IsSuccess(out var artifactFileHandle))
                {
                    // recursion not possible
                    continue;
                }

                var artifactUri = artifactFileHandle.Uri;

                // recurse into child modules, to ensure we have an exhaustive list of restorable artifacts for the full compilation
                if (!fileResultByUri.TryGetValue(artifactUri, out var childResult) ||
                    (childResult.IsSuccess(out var childFile) && sourceFileExplorerBuild is not null && sourceFileExplorerBuild.Contains(childFile)))
                {
                    // only recurse if we've not seen this file before - to avoid infinite loops
                    childResult = PopulateRecursive(artifactUri, resolutionInfo.Reference, sourceFileExplorerBuild);
                }
                fileResultByUri[artifactUri] = childResult;
            }
        }

        private ImplicitExtension GetImplicitExtension(string extensionName, BicepSourceFile referencingFile, RootConfiguration config)
        {
            if (!config.Extensions.TryGetExtensionSource(extensionName).IsSuccess(out var extensionEntry, out var errorBuilder))
            {
                return new(extensionName, null, new(referencingFile, null, null, new(errorBuilder), RequiresRestore: false));
            }

            if (extensionEntry.BuiltIn)
            {
                return new(extensionName, extensionEntry, null);
            }

            if (!dispatcher.TryGetArtifactReference(referencingFile, ArtifactType.Extension, extensionEntry.Value).IsSuccess(out var artifactReference, out errorBuilder))
            {
                // reference is not valid
                return new(extensionName, extensionEntry, new(referencingFile, null, null, new(errorBuilder), RequiresRestore: false));
            }

            var (result, requiresRestore) = GetArtifactRestoreResult(artifactReference);
            return new(extensionName, extensionEntry, new(referencingFile, null, artifactReference, result, RequiresRestore: requiresRestore));
        }

        private ArtifactResolutionInfo GetArtifactRestoreResult(BicepSourceFile referencingFile, IArtifactReferenceSyntax referenceSyntax)
        {
            if (!dispatcher.TryGetArtifactReference(referencingFile, referenceSyntax).IsSuccess(out var artifactReference, out var errorBuilder))
            {
                // artifact reference is not valid
                return new(referencingFile, referenceSyntax, null, new(errorBuilder), RequiresRestore: false);
            }

            var (result, requiresRestore) = GetArtifactRestoreResult(artifactReference);

            return new(referencingFile, referenceSyntax, artifactReference, result, RequiresRestore: requiresRestore);
        }

        private (ResultWithDiagnosticBuilder<IFileHandle> result, bool requiresRestore) GetArtifactRestoreResult(ArtifactReference artifactReference)
        {
            if (!dispatcher.TryGetLocalArtifactEntryPointFileHandle(artifactReference).IsSuccess(out var artifactFileHandle, out var artifactGetPathFailureBuilder))
            {
                // invalid artifact reference - exit early to show the user the diagnostic
                return (new(artifactGetPathFailureBuilder), requiresRestore: false);
            }

            if (forceRestore)
            {
                //override the status to force restore
                return (new(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference)), requiresRestore: true);
            }

            var restoreStatus = dispatcher.GetArtifactRestoreStatus(artifactReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (new(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference)), requiresRestore: true);
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    restoreErrorBuilder ??= new(x => x.ArtifactRestoreFailed(artifactReference.FullyQualifiedReference));
                    return (new(restoreErrorBuilder), requiresRestore: false);
                default:
                    break;
            }

            return (new(artifactFileHandle), requiresRestore: false);
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetArtifactReferenceDeclarations(sourceFile)
                    .Select(x => this.artifactLookup.TryGetValue(x)?.Result.TryUnwrap())
                    .WhereNotNull()
                    .Select(fileHandle => this.fileResultByUri.TryGetValue(fileHandle.Uri)?.TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (statement, info) in artifactLookup)
            {
                if (statement.GetArtifactType() == ArtifactType.Module &&
                    info.Result.IsSuccess(out var fileHandle) &&
                    fileResultByUri[fileHandle.Uri].IsSuccess(out var sourceFile) &&
                    cycles.TryGetValue(sourceFile, out var cycle))
                {
                    ResultWithDiagnosticBuilder<IFileHandle> result = cycle switch
                    {
                        { Length: 1 } when cycle[0] is BicepParamFile paramFile => new(x => x.CyclicParametersSelfReference()),
                        { Length: 1 } => new(x => x.CyclicModuleSelfReference()),
                        // the error message is generic so it should work for either bicep module or params
                        _ => new(x => x.CyclicFile(cycle.Select(u => u.FileHandle.Uri.ToString()))),
                    };

                    // overwrite to add the cycle error
                    artifactLookup[statement] = info with { Result = result };
                }
            }

            return sourceFileGraph;
        }

        private static IEnumerable<IArtifactReferenceSyntax> GetArtifactReferenceDeclarations(ISourceFile sourceFile) => sourceFile switch
        {
            BicepSourceFile x => GetArtifactReferences(x.ProgramSyntax),
            _ => [],
        };

        private static IEnumerable<IArtifactReferenceSyntax> GetArtifactReferences(ProgramSyntax program)
            => program.Declarations.OfType<IArtifactReferenceSyntax>();
    }
}
