// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Utils;

namespace Bicep.Core.Workspaces;

public record ImplicitProvider(
    string Name,
    ProviderConfigEntry? Config,
    ArtifactResolutionInfo? Artifact);

public record ArtifactResolutionInfo(
    BicepSourceFile Origin,
    IArtifactReferenceSyntax? Syntax,
    ArtifactReference? Reference,
    ResultWithDiagnostic<Uri> Result,
    bool RequiresRestore);

public record SourceFileGrouping(
    BicepSourceFile EntryPoint,
    ImmutableArray<ISourceFile> SourceFiles,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup,
    ImmutableDictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> ArtifactLookup,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ImplicitProvider>> ImplicitProviders,
    ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> SourceFileLookup) : IArtifactFileLookup
{
    public IEnumerable<ArtifactResolutionInfo> GetArtifactsToRestore(bool force = false)
    {
        var artifacts = ArtifactLookup.Values.Concat(ImplicitProviders.Values.SelectMany(x => x).Select(x => x.Artifact));

        foreach (var (_, artifact) in ArtifactLookup.Where(x => ShouldRestore(x.Value, force)))
        {
            yield return artifact;
        }

        foreach (var (file, providers) in ImplicitProviders)
        {
            foreach (var artifact in providers.Select(x => x.Artifact).WhereNotNull().Where(artifact => ShouldRestore(artifact, force)))
            {
                yield return artifact;
            }
        }
    }

    public static bool ShouldRestore(ArtifactResolutionInfo artifact, bool force = false)
        => force || (!artifact.Result.IsSuccess() && artifact.RequiresRestore);

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

        return [.. knownFiles];
    }
}
