// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Utils;

namespace Bicep.Core.Workspaces;

public record ArtifactResolutionInfo(
    BicepSourceFile Origin,
    IArtifactReferenceSyntax Syntax,
    ArtifactReference? Reference,
    ResultWithDiagnostic<Uri> Result,
    bool RequiresRestore);

public record SourceFileGrouping(
    BicepSourceFile EntryPoint,
    ImmutableArray<ISourceFile> SourceFiles,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup,
    ImmutableDictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> ArtifactLookup,
    ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> SourceFileLookup) : IArtifactFileLookup
{
    public IEnumerable<ArtifactResolutionInfo> GetArtifactsToRestore(bool force = false)
    {
        foreach (var (syntax, artifact) in ArtifactLookup)
        {
            if (force || !artifact.Result.IsSuccess(out _, out var failure) && artifact.RequiresRestore)
            {
                yield return artifact;
            }
        }
    }

    public ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax reference)
    {
        if (!ArtifactLookup[reference].Result.IsSuccess(out var fileUri, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        return SourceFileLookup[fileUri];
    }

    public ImmutableHashSet<ISourceFile> GetFilesDependingOn(ISourceFile sourceFile)
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

        return knownFiles.ToImmutableHashSet();
    }
}
