// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Redirects;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
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
        private readonly RedirectingArtifactResolver referenceResolver;
        private readonly IRemoteArtifactRestorer restorer;
        private readonly IActiveSourceFileLookup workspace;
        private readonly ISourceFileFactory sourceFileFactory;

        private readonly Dictionary<IOUri, ResultWithDiagnosticBuilder<ISourceFile>> fileResultByUri;
        private readonly Dictionary<IArtifactReferenceSyntax, ArtifactDependency> artifactLookup;
        private readonly Dictionary<ISourceFile, HashSet<ImplicitExtension>> implicitExtensions;
        private readonly bool forceRestore;

        private SourceFileGroupingBuilder(
            IFileExplorer fileExplorer,
            RedirectingArtifactResolver referenceResolver,
            IRemoteArtifactRestorer restorer,
            IActiveSourceFileLookup workspace,
            ISourceFileFactory sourceFileFactory,
            bool forceModulesRestore = false)
        {
            this.fileExplorer = fileExplorer;
            this.referenceResolver = referenceResolver;
            this.restorer = restorer;
            this.workspace = workspace;
            this.sourceFileFactory = sourceFileFactory;
            this.artifactLookup = [];
            this.implicitExtensions = [];
            this.fileResultByUri = [];
            this.forceRestore = forceModulesRestore;
        }

        private SourceFileGroupingBuilder(
            IFileExplorer fileExplorer,
            RedirectingArtifactResolver referenceResolver,
            IRemoteArtifactRestorer restorer,
            IActiveSourceFileLookup workspace,
            ISourceFileFactory sourceFileFactory,
            SourceFileGrouping current,
            bool forceArtifactRestore = false)
        {
            this.fileExplorer = fileExplorer;
            this.referenceResolver = referenceResolver;
            this.restorer = restorer;
            this.workspace = workspace;
            this.sourceFileFactory = sourceFileFactory;
            this.artifactLookup = current.ArtifactLookup.Where(x => x.Value is ArtifactDependency.Loaded).ToDictionary();
            this.implicitExtensions = current.ImplicitExtensions.ToDictionary(x => x.Key, x => x.Value.ToHashSet());
            this.fileResultByUri = current.SourceFileLookup.ToDictionary();
            this.forceRestore = forceArtifactRestore;
        }

        public static SourceFileGrouping Build(IFileExplorer fileExplorer, RedirectingArtifactResolver referenceResolver, IRemoteArtifactRestorer restorer, IActiveSourceFileLookup workspace, ISourceFileFactory sourceFileFactory, IOUri entryFileUri, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileExplorer, referenceResolver, restorer, workspace, sourceFileFactory, forceModulesRestore);

            return builder.Build(entryFileUri);
        }

        public static SourceFileGrouping Rebuild(IFileExplorer fileExplorer, RedirectingArtifactResolver referenceResolver, IRemoteArtifactRestorer restorer, IActiveSourceFileLookup workspace, ISourceFileFactory sourceFileFactory, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(fileExplorer, referenceResolver, restorer, workspace, sourceFileFactory, current);

            var sourceFilesRequiringRestore = new HashSet<ISourceFile>();
            foreach (var (syntax, dependency) in current.ArtifactLookup.Where(x => SourceFileGrouping.ShouldRestore(x.Value)))
            {
                builder.artifactLookup.Remove(syntax);
                sourceFilesRequiringRestore.Add(dependency.ReferencingFile);
            }

            foreach (var (file, extensions) in current.ImplicitExtensions)
            {
                foreach (var extension in extensions.Where(x => x.Dependency is { } dependency && SourceFileGrouping.ShouldRestore(dependency)))
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
            var fileResult = this.PopulateRecursive(entryFileUri, ModuleTemplateKind.SourceFile, sourceFileExplorerBuild);

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

        private ResultWithDiagnosticBuilder<ISourceFile> GetFileResolutionResult(IOUri fileUri, ModuleTemplateKind kind)
        {
            if (workspace.TryGetSourceFile(fileUri) is { } sourceFile)
            {
                return new(sourceFile);
            }

            if (!this.fileExplorer.GetFile(fileUri).TryReadAllText().IsSuccess(out var fileContents, out var failureBuilder))
            {
                return new(failureBuilder);
            }


            sourceFile = kind == ModuleTemplateKind.TemplateSpec
                ? this.sourceFileFactory.CreateTemplateSpecFile(fileUri, fileContents)
                : this.sourceFileFactory.CreateSourceFile(fileUri, fileContents);

            return new(sourceFile);
        }

        private ResultWithDiagnosticBuilder<ISourceFile> GetFileResolutionResultWithCaching(IOUri fileUri, ModuleTemplateKind kind)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, kind);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private ResultWithDiagnosticBuilder<ISourceFile> PopulateRecursive(IOUri fileUri, ModuleTemplateKind kind, FrozenSet<ISourceFile>? sourceFileExplorerBuild)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, kind);
            if (fileResult.TryUnwrap() is BicepSourceFile bicepSource)
            {
                PopulateRecursive(bicepSource, sourceFileExplorerBuild);
            }

            return fileResult;
        }

        private void PopulateRecursive(BicepSourceFile file, FrozenSet<ISourceFile>? sourceFileExplorerBuild)
        {
            var config = file.LoadConfiguration();
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

                    artifactLookup[restorable] = GetArtifactDependency(file, restorable);

                    // recursion not needed for extension declarations
                    continue;
                }

                var dependency = GetArtifactDependency(file, restorable);
                artifactLookup[restorable] = dependency;
                if (dependency is not ArtifactDependency.Loaded { Artifact: ModuleArtifact moduleArtifact })
                {
                    // recursion not possible
                    continue;
                }

                var artifactUri = moduleArtifact.EntryPoint.Uri;

                // recurse into child modules, to ensure we have an exhaustive list of restorable artifacts for the full compilation
                if (!fileResultByUri.TryGetValue(artifactUri, out var childResult) ||
                    (childResult.IsSuccess(out var childFile) && sourceFileExplorerBuild is not null && sourceFileExplorerBuild.Contains(childFile)))
                {
                    // only recurse if we've not seen this file before - to avoid infinite loops
                    childResult = PopulateRecursive(artifactUri, moduleArtifact.Kind, sourceFileExplorerBuild);
                }
                fileResultByUri[artifactUri] = childResult;
            }
        }

        private ImplicitExtension GetImplicitExtension(string extensionName, BicepSourceFile referencingFile, IBicepConfiguration config)
        {
            if (!config.Extensions.TryGetExtensionSource(extensionName).IsSuccess(out var extensionEntry, out var errorBuilder))
            {
                // The extension is listed in "implicitExtensions" but has no entry in "extensions".
                return new(extensionName, new ArtifactDependency.Failed(referencingFile, null, null, x => x.InvalidExtension_ImplicitExtensionMissingConfig(config.ConfigFileUri, extensionName)));
            }

            if (extensionEntry.BuiltIn)
            {
                return new(extensionName, null);
            }

            if (!referenceResolver.TryResolve(referencingFile.FileHandle.Uri, ArtifactType.Extension, extensionEntry.Value).IsSuccess(out var artifactReference, out errorBuilder))
            {
                // reference is not valid
                return new(extensionName, new ArtifactDependency.Failed(referencingFile, null, null, errorBuilder));
            }

            return new(extensionName, GetArtifactDependency(referencingFile, null, artifactReference));
        }

        private ArtifactDependency GetArtifactDependency(BicepSourceFile referencingFile, IArtifactReferenceSyntax referenceSyntax)
        {
            if (!ArtifactReferenceText.TryGet(referencingFile, referenceSyntax).IsSuccess(out var referenceText, out var errorBuilder) ||
                !referenceResolver.TryResolve(referencingFile.FileHandle.Uri, referenceSyntax.GetArtifactType(), referenceText).IsSuccess(out var artifactReference, out errorBuilder))
            {
                // artifact reference is not valid
                return new ArtifactDependency.Failed(referencingFile, referenceSyntax, null, errorBuilder);
            }

            return GetArtifactDependency(referencingFile, referenceSyntax, artifactReference);
        }

        private ArtifactDependency GetArtifactDependency(BicepSourceFile referencingFile, IArtifactReferenceSyntax? referenceSyntax, ArtifactReference artifactReference) => artifactReference switch
        {
            // Local files are loaded directly; they are never restored.
            LocalArtifactReference local => LocalArtifactLoader.TryOpen(local, referencingFile.LoadFeatures().CacheRootDirectory).IsSuccess(out var artifact, out var errorBuilder)
                ? new ArtifactDependency.Loaded(referencingFile, referenceSyntax, artifact)
                : new ArtifactDependency.Failed(referencingFile, referenceSyntax, local, errorBuilder),
            RemoteArtifactReference remote => restorer.GetRestoreState(remote) switch
            {
                // A recent restore failed. Report it rather than retrying, even when a restore is forced.
                RemoteArtifactRestoreState.RestoreFailed failed => new ArtifactDependency.Failed(referencingFile, referenceSyntax, remote, failed.Error),
                RemoteArtifactRestoreState.Restored restored when !forceRestore => new ArtifactDependency.Loaded(referencingFile, referenceSyntax, restored.Artifact),
                _ => new ArtifactDependency.NotRestored(referencingFile, referenceSyntax, remote),
            },
            _ => throw new InvalidOperationException($"Unexpected artifact reference type '{artifactReference.GetType().Name}'."),
        };

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetArtifactReferenceDeclarations(sourceFile)
                    .Select(x => this.artifactLookup.TryGetValue(x))
                    .OfType<ArtifactDependency.Loaded>()
                    .Select(dependency => dependency.Artifact)
                    .OfType<ModuleArtifact>()
                    .Select(artifact => this.fileResultByUri.TryGetValue(artifact.EntryPoint.Uri)?.TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (statement, dependency) in artifactLookup)
            {
                if (statement.GetArtifactType() == ArtifactType.Module &&
                    dependency is ArtifactDependency.Loaded { Artifact: ModuleArtifact artifact } &&
                    fileResultByUri[artifact.EntryPoint.Uri].IsSuccess(out var sourceFile) &&
                    cycles.TryGetValue(sourceFile, out var cycle))
                {
                    DiagnosticBuilderDelegate error = cycle switch
                    {
                        { Length: 1 } when cycle[0] is BicepParamFile paramFile => x => x.CyclicParametersSelfReference(),
                        { Length: 1 } => x => x.CyclicModuleSelfReference(),
                        // the error message is generic so it should work for either bicep module or params
                        _ => x => x.CyclicFile(cycle.Select(u => u.FileHandle.Uri.ToString())),
                    };

                    // overwrite to add the cycle error
                    artifactLookup[statement] = new ArtifactDependency.Failed(dependency.ReferencingFile, dependency.Syntax, artifact.Reference, error);
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
