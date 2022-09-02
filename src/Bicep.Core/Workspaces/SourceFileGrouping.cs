// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public record FileResolutionResult(
        Uri FileUri,
        ErrorBuilderDelegate? ErrorBuilder,
        ISourceFile? File);

    public record UriResolutionResult(
        StatementSyntax Statement,
        Uri? FileUri,
        bool RequiresRestore,
        ErrorBuilderDelegate? ErrorBuilder);

    public record SourceFileGrouping(
        IFileResolver FileResolver,
        Uri EntryFileUri,
        ImmutableDictionary<Uri, FileResolutionResult> FileResultByUri,
        ImmutableDictionary<StatementSyntax, UriResolutionResult> UriResultByModule,
        ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup)
    {
        public IEnumerable<ModuleDeclarationSyntax> GetModulesToRestore()
        {
            var allModules = UriResultByModule.Keys.OfType<ModuleDeclarationSyntax>();

            return allModules.Where(x => UriResultByModule[x].RequiresRestore);
        }

        public BicepFile EntryPoint => (FileResultByUri[EntryFileUri].File as BicepFile)!;

        public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.File).WhereNotNull();

        public ErrorBuilderDelegate? TryGetErrorDiagnostic(StatementSyntax statement)
        {
            var uriResult = UriResultByModule[statement];
            if (uriResult.ErrorBuilder is not null)
            {
                return uriResult.ErrorBuilder;
            }

            var fileResult = FileResultByUri[uriResult.FileUri!];
            return fileResult.ErrorBuilder;
        }

        public ISourceFile? TryGetSourceFile(StatementSyntax statement)
        {
            var uriResult = UriResultByModule[statement];
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
