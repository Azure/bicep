// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    
    public record ArtifactResolutionInfo(
        IArtifactReferenceSyntax DeclarationSyntax,
        ISourceFile ParentTemplateFile);

    public record FileResolutionResult(
        Uri FileUri,
        ErrorBuilderDelegate? ErrorBuilder,
        ISourceFile? File);

    public record UriResolutionResult(
        Uri? FileUri,
        bool RequiresRestore,
        ErrorBuilderDelegate? ErrorBuilder);

    public record SourceFileGrouping(
        IFileResolver FileResolver,
        Uri EntryFileUri,
        ImmutableDictionary<Uri, FileResolutionResult> FileResultByUri,
        ImmutableDictionary<ISourceFile, ImmutableDictionary<IArtifactReferenceSyntax, UriResolutionResult>> UriResultByArtifactReference,
        ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup) : ISourceFileLookup
    {
        public IEnumerable<ArtifactResolutionInfo> GetModulesToRestore()
            => UriResultByArtifactReference.SelectMany(
                kvp => kvp.Value
                    .Where(entry => entry.Value.RequiresRestore)
                    .Select(entry => new ArtifactResolutionInfo(entry.Key, kvp.Key)));

        public BicepSourceFile EntryPoint => (FileResultByUri[EntryFileUri].File as BicepSourceFile)!;

        public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.File).WhereNotNull();

        public ErrorBuilderDelegate? TryGetErrorDiagnostic(IArtifactReferenceSyntax foreignTemplateReference)
        {
            var uriResult = UriResultByArtifactReference.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
            if (uriResult.ErrorBuilder is not null)
            {
                return uriResult.ErrorBuilder;
            }

            var fileResult = FileResultByUri[uriResult?.FileUri!];
            return fileResult.ErrorBuilder;
        }

        public ISourceFile? TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference)
        {
            var uriResult = UriResultByArtifactReference.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
            if (uriResult.FileUri is null)
            {
                return null;
            }

            var fileResult = FileResultByUri.TryGetValue(uriResult.FileUri);
            return fileResult?.File;
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
}
