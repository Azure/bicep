// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
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
        ImmutableDictionary<BicepSourceFile, ProviderDescriptorBundle> providerDescriptorBundleBySourceFile,
        ImmutableDictionary<BicepSourceFile, ImmutableDictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> fileUriResultByArtifactReferenceSyntax,
        ImmutableDictionary<BicepSourceFile, ImmutableDictionary<ArtifactReference, Result<Uri, UriResolutionError>>> fileUriResultByArtifactReference,
        ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup)
    {
        FileResolver = fileResolver;
        EntryFileUri = entryFileUri;
        FileResultByUri = fileResultByUri;
        FileUriResultByBicepSourceFileByArtifactReferenceSyntax = fileUriResultByArtifactReferenceSyntax;
        FileUriResultByBicepSourceFileByArtifactReference = fileUriResultByArtifactReference;
        ProvidersToRestoreByFileResult = providerDescriptorBundleBySourceFile;
        SourceFileParentLookup = sourceFileParentLookup;
    }

    public IFileResolver FileResolver { get; }

    public Uri EntryFileUri { get; }

    public ImmutableDictionary<Uri, ResultWithDiagnostic<ISourceFile>> FileResultByUri { get; }

    public ImmutableDictionary<BicepSourceFile, ImmutableDictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> FileUriResultByBicepSourceFileByArtifactReferenceSyntax { get; }

    public ImmutableDictionary<BicepSourceFile, ImmutableDictionary<ArtifactReference, Result<Uri, UriResolutionError>>> FileUriResultByBicepSourceFileByArtifactReference { get; }

    public ImmutableDictionary<BicepSourceFile, ProviderDescriptorBundle> ProvidersToRestoreByFileResult { get; }

    public ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup { get; }

    public IEnumerable<ArtifactResolutionInfo> GetExplicitArtifactsToRestore(bool force = false)
    {
        foreach (var (sourceFile, artifactResults) in FileUriResultByBicepSourceFileByArtifactReferenceSyntax)
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

    public IEnumerable<ArtifactReference> GetImplicitArtifactsToRestore()
    {
        foreach (var (sourceFile, artifactResults) in FileUriResultByBicepSourceFileByArtifactReference)
        {
            foreach (var (artifactReference, result) in artifactResults)
            {
                if (!result.IsSuccess(out _, out var failure) && failure.RequiresRestore)
                {
                    yield return artifactReference;
                }
            }
        }
    }

    public BicepSourceFile EntryPoint
        => (FileResultByUri[EntryFileUri].TryUnwrap() as BicepSourceFile) ?? throw new InvalidOperationException($"{nameof(EntryFileUri)} is not a Bicep source file!");

    public IEnumerable<ISourceFile> SourceFiles => FileResultByUri.Values.Select(x => x.IsSuccess(out var success) ? success : null).WhereNotNull();

    public ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactReferenceSyntax foreignTemplateReference)
    {
        var uriResult = FileUriResultByBicepSourceFileByArtifactReferenceSyntax.Values.Select(d => d.TryGetValue(foreignTemplateReference, out var result) ? result : null).WhereNotNull().First();
        if (!uriResult.IsSuccess(out var fileUri, out var errorBuilder))
        {
            return new(errorBuilder.ErrorBuilder);
        }
        return FileResultByUri[fileUri];
    }

    public ResultWithDiagnostic<ResourceTypesProviderDescriptor> TryGetProviderDescriptor(ProviderDeclarationSyntax providerDeclarationSyntax)
    {
        var providerDescriptorResult = ProvidersToRestoreByFileResult.Values.Select(d => d.ExplicitProviderLookup.TryGetValue(providerDeclarationSyntax, out var providerDescriptor) ? providerDescriptor : null).WhereNotNull().First();
        if (!providerDescriptorResult.IsSuccess(out var providerDescriptor, out var errorBuilder))
        {
            return new(errorBuilder);
        }
        return new(providerDescriptor);
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
