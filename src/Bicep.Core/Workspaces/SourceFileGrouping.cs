// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public class SourceFileGrouping : ISourceFileLookup
    {
        public SourceFileGrouping(IFileResolver fileResolver,
            Uri entryFileUri,
            ImmutableDictionary<Uri, FileResolutionResult> fileResultByUri,
            ImmutableDictionary<ISourceFile, ImmutableDictionary<IArtifactReferenceSyntax, UriResolutionResult>> uriResultByModule,
            ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup)
        {
            FileResolver = fileResolver;
            EntryFileUri = entryFileUri;
            FileResultByUri = fileResultByUri;
            UriResultByModule = uriResultByModule;
            SourceFileParentLookup = sourceFileParentLookup;

            UriResults = uriResultByModule.Values.SelectMany(kvp => kvp).ToImmutableDictionary();
        }

        public IFileResolver FileResolver { get; }

        public Uri EntryFileUri { get; }

        public ImmutableDictionary<Uri, FileResolutionResult> FileResultByUri { get; }

        public ImmutableDictionary<ISourceFile, ImmutableDictionary<IArtifactReferenceSyntax, UriResolutionResult>> UriResultByModule { get; }

        public ImmutableDictionary<IArtifactReferenceSyntax, UriResolutionResult> UriResults { get; }

        public ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup { get; }

        public IEnumerable<ArtifactResolutionInfo> GetModulesToRestore()
            => UriResultByModule.SelectMany(
                kvp => kvp.Value
                    .Where(entry => entry.Value.RequiresRestore)
                    .Select(entry => new ArtifactResolutionInfo(entry.Key, kvp.Key)));

        public BicepSourceFile EntryPoint => (FileResultByUri[EntryFileUri].File as BicepSourceFile)!;

        public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.File).WhereNotNull();

        public ErrorBuilderDelegate? TryGetErrorDiagnostic(IArtifactReferenceSyntax foreignTemplateReference)
        {
            var uriResult = UriResults[foreignTemplateReference];
            if (uriResult.ErrorBuilder is not null)
            {
                return uriResult.ErrorBuilder;
            }

            var fileResult = FileResultByUri[uriResult.FileUri!];
            return fileResult.ErrorBuilder;
        }

        public ISourceFile? TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference)
        {
            var uriResult = UriResults[foreignTemplateReference];
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
