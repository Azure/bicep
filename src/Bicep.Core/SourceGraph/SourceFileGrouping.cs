// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph;

public record ImplicitExtension(
    string Name,
    ExtensionConfigEntry? Config,
    ArtifactResolutionInfo? Artifact);

public record ArtifactResolutionInfo(
    BicepSourceFile ReferencingFile,
    IArtifactReferenceSyntax? Syntax,
    ArtifactReference? Reference,
    ResultWithDiagnosticBuilder<IFileHandle> Result,
    bool RequiresRestore);

public record SourceFileGrouping(
    BicepSourceFile EntryPoint,
    ImmutableArray<ISourceFile> SourceFiles,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup,
    ImmutableDictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> ArtifactLookup,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ImplicitExtension>> ImplicitExtensions,
    ImmutableDictionary<Uri, ResultWithDiagnosticBuilder<ISourceFile>> SourceFileLookup) : IArtifactFileLookup
{
    public IEnumerable<ArtifactResolutionInfo> GetArtifactsToRestore(bool force = false)
    {
        var artifacts = ArtifactLookup.Values.Concat(ImplicitExtensions.Values.SelectMany(x => x).Select(x => x.Artifact));

        foreach (var (_, artifact) in ArtifactLookup.Where(x => ShouldRestore(x.Value, force)))
        {
            yield return artifact;
        }

        foreach (var (file, extensions) in ImplicitExtensions)
        {
            foreach (var artifact in extensions.Select(x => x.Artifact).WhereNotNull().Where(artifact => ShouldRestore(artifact, force)))
            {
                yield return artifact;
            }
        }
    }

    public static bool ShouldRestore(ArtifactResolutionInfo artifact, bool force = false)
        => force || (!artifact.Result.IsSuccess() && artifact.RequiresRestore);

    public ResultWithDiagnosticBuilder<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax reference)
    {
        if (!ArtifactLookup[reference].Result.IsSuccess(out var fileHandle, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        return SourceFileLookup[fileHandle.Uri.ToUri()];
    }

    public FrozenSet<ISourceFile> GetSourceFilesDependingOn(ISourceFile sourceFile)
    {
        var filesToCheck = new Queue<ISourceFile>(new[] { sourceFile });
        var knownFiles = new HashSet<ISourceFile>();

        while (filesToCheck.TryDequeue(out var current))
        {
            knownFiles.Add(current);

            if (SourceFileParentLookup.TryGetValue(current, out var parents))
            {
                foreach (var parent in parents.Where(x => !knownFiles.Contains(x)))
                {
                    knownFiles.Add(parent);
                    filesToCheck.Enqueue(parent);
                }
            }
        }

        return knownFiles.ToFrozenSet();
    }

    public IEnumerable<BicepSourceFile> EnumerateBicepSourceFiles() => this.SourceFiles.OfType<BicepSourceFile>();

    public FrozenSet<IOUri> GetAllReferencedAuxiliaryFileUris()
    {
        var fileUris = new HashSet<IOUri>();

        foreach (var sourceFile in this.EnumerateBicepSourceFiles())
        {
            fileUris.UnionWith(sourceFile.GetReferencedAuxiliaryFileUris());
        }

        return fileUris.ToFrozenSet();
    }
}
