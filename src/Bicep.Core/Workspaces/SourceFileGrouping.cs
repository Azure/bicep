// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Utils;

namespace Bicep.Core.Workspaces;

public record ArtifactResolutionInfo(
    IArtifactReferenceSyntax DeclarationSyntax,
    BicepSourceFile SourceFile);

public record UriResolutionError(
    DiagnosticBuilder.ErrorBuilderDelegate ErrorBuilder,
    bool RequiresRestore);

public class SourceFileGrouping : IArtifactFileLookup
{
    public SourceFileGrouping(IFileResolver fileResolver,
        Uri entryFileUri,
        ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> fileResultByUri,
        ImmutableDictionary<BicepSourceFile, ImmutableDictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> fileUriResultByArtifactReference,
        ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup)
    {
        FileResolver = fileResolver;
        EntryFileUri = entryFileUri;
        FileResultByUri = fileResultByUri;
        FileUriResultByArtifactReference = fileUriResultByArtifactReference;
        SourceFileParentLookup = sourceFileParentLookup;
    }

    public IFileResolver FileResolver { get; }

    public Uri EntryFileUri { get; }

    public ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> FileResultByUri { get; }

    public ImmutableDictionary<BicepSourceFile, ImmutableDictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> FileUriResultByArtifactReference { get; }

    public ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup { get; }

    public IEnumerable<ArtifactResolutionInfo> GetArtifactsToRestore(bool force = false)
    {
        foreach (var (sourceFile, artifactResults) in FileUriResultByArtifactReference)
        {
            foreach (var (syntax, result) in artifactResults)
            {
                if (force || !result.IsSuccess(out _, out var failure) && failure.RequiresRestore)
                {
                    yield return new(syntax, sourceFile);
                }
            }
        }
    }

    public BicepSourceFile EntryPoint
        => (FileResultByUri[EntryFileUri].TryUnwrap() as BicepSourceFile) ?? throw new InvalidOperationException($"{nameof(EntryFileUri)} is not a Bicep source file!");

    public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.IsSuccess(out var success) ? success : null).WhereNotNull();

    public ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference)
        => TryGetResourceTypesFileUri(foreignTemplateReference).IsSuccess(out var fileUri, out var errorBuilder) ? FileResultByUri[fileUri] : new(errorBuilder);

    public ResultWithDiagnostic<Uri> TryGetResourceTypesFileUri(IArtifactReferenceSyntax foreignTemplateReference)
    {
        var uriResult = FileUriResultByArtifactReference.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
        if (!uriResult.IsSuccess(out var fileUri, out var error))
        {
            return new(error.ErrorBuilder);
        }

        return new(fileUri);
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
