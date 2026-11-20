// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Artifacts;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph;

/// <summary>
/// An extension that the Bicep configuration loads into every file ("implicitExtensions").
/// </summary>
/// <param name="Dependency">The extension's artifact, or <c>null</c> for a built-in extension.</param>
public record ImplicitExtension(
    string Name,
    ArtifactDependency? Dependency);

public record SourceFileGrouping(
    BicepSourceFile EntryPoint,
    ImmutableArray<ISourceFile> SourceFiles,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup,
    ImmutableDictionary<IArtifactReferenceSyntax, ArtifactDependency> ArtifactLookup,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ImplicitExtension>> ImplicitExtensions,
    ImmutableDictionary<IOUri, ResultWithDiagnosticBuilder<ISourceFile>> SourceFileLookup) : IArtifactFileLookup
{
    public IEnumerable<ArtifactDependency> GetArtifactsToRestore(bool force = false)
    {
        foreach (var (_, dependency) in ArtifactLookup.Where(x => ShouldRestore(x.Value, force)))
        {
            yield return dependency;
        }

        foreach (var (file, extensions) in ImplicitExtensions)
        {
            foreach (var dependency in extensions.Select(x => x.Dependency).WhereNotNull().Where(dependency => ShouldRestore(dependency, force)))
            {
                yield return dependency;
            }
        }
    }

    public static bool ShouldRestore(ArtifactDependency dependency, bool force = false)
        => force || dependency is ArtifactDependency.NotRestored;

    public ResultWithDiagnosticBuilder<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax reference)
    {
        if (!ArtifactLookup[reference].TryGetArtifact().IsSuccess(out var artifact, out var errorBuilder))
        {
            return new(errorBuilder);
        }

        if (artifact is not ModuleArtifact moduleArtifact)
        {
            throw new InvalidOperationException($"Expected a module, but the artifact for '{artifact.Reference.FullyQualifiedReference}' is a {artifact.Reference.Type}.");
        }

        return SourceFileLookup[moduleArtifact.EntryPoint.Uri];
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
