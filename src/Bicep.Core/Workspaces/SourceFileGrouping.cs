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
    public interface IArtifactResolutionInfo
    {
        IForeignTemplateReference ForeignTemplateReference { get; }
        ISourceFile ParentTemplateFile { get; }
    }
    public record ModuleSourceResolutionInfo(
        IForeignTemplateReference ModuleDeclaration,
        ISourceFile ModuleParentTemplateFile) : IArtifactResolutionInfo
    {
        public IForeignTemplateReference ForeignTemplateReference => ModuleDeclaration;
        public ISourceFile ParentTemplateFile => ModuleParentTemplateFile;
    }

    public record ProviderSourceResolutionInfo(
        IForeignTemplateReference ProviderDeclaration,
        ISourceFile ProviderParentTemplateFile) : IArtifactResolutionInfo
    {
        public IForeignTemplateReference ForeignTemplateReference => ProviderDeclaration;
        public ISourceFile ParentTemplateFile => ProviderParentTemplateFile;
    };
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
        ImmutableDictionary<ISourceFile, ImmutableDictionary<IForeignTemplateReference, UriResolutionResult>> UriResultByModule,
        ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup) : ISourceFileLookup
    {
        public IEnumerable<ModuleSourceResolutionInfo> GetModulesToRestore()
            => UriResultByModule.SelectMany(
                kvp => kvp.Value
                    .Where(entry => entry.Value.RequiresRestore)
                    .Select(entry => new ModuleSourceResolutionInfo(entry.Key, kvp.Key)));


        public IEnumerable<IArtifactResolutionInfo> GetArtifactsToRestore() =>
            GetModulesToRestore().AsEnumerable<IArtifactResolutionInfo>();

        public BicepSourceFile EntryPoint => (FileResultByUri[EntryFileUri].File as BicepSourceFile)!;

        public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.File).WhereNotNull();

        public ErrorBuilderDelegate? TryGetErrorDiagnostic(IForeignTemplateReference foreignTemplateReference)
        {
            var uriResult = UriResultByModule.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
            if (uriResult.ErrorBuilder is not null)
            {
                return uriResult.ErrorBuilder;
            }

            var fileResult = FileResultByUri[uriResult?.FileUri!];
            return fileResult.ErrorBuilder;
        }

        public ISourceFile? TryGetSourceFile(IForeignTemplateReference foreignTemplateReference)
        {
            var uriResult = UriResultByModule.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
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
