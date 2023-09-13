// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces;

public record ArtifactResolutionInfo(
    IArtifactReferenceSyntax DeclarationSyntax,
    ISourceFile ParentTemplateFile);

public record UriResolutionError(
    DiagnosticBuilder.ErrorBuilderDelegate ErrorBuilder,
    bool RequiresRestore);

public record SourceFileGrouping(
    IFileResolver FileResolver,
    Uri EntryFileUri,
    ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> FileResultByUri,
    ImmutableDictionary<ISourceFile, ImmutableDictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> UriResultByArtifactReference,
    ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup) : ISourceFileLookup
{
    public IEnumerable<ArtifactResolutionInfo> GetModulesToRestore()
    {
        foreach (var (sourceFile, moduleResults) in UriResultByArtifactReference)
        {
            foreach (var (syntax, result) in moduleResults)
            {
                if (!result.IsSuccess(out _, out var failure) && failure.RequiresRestore)
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
    {
        var uriResult = UriResultByArtifactReference.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
        if (!uriResult.IsSuccess(out var fileUri, out var error))
        {
            return new(error.ErrorBuilder);
        }

        return FileResultByUri[fileUri];
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