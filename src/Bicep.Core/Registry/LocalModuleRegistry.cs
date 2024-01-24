// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;

namespace Bicep.Core.Registry
{
    public class LocalModuleRegistry : ArtifactRegistry<LocalModuleReference>
    {
        private readonly IFileResolver fileResolver;
        private readonly Uri parentModuleUri;
        private readonly BicepCompiler? bicepCompiler;

        public LocalModuleRegistry(IFileResolver fileResolver, Uri parentModuleUri, BicepCompiler? bicepCompiler)
        {
            this.fileResolver = fileResolver;
            this.parentModuleUri = parentModuleUri;
            this.bicepCompiler = bicepCompiler;
        }

        public override string Scheme => ModuleReferenceSchemes.Local;

        public override RegistryCapabilities GetCapabilities(LocalModuleReference reference) => RegistryCapabilities.Default;

        public override ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? alias, string reference)
        {
            if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Provider)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }
            if (!LocalModuleReference.TryParse(reference, parentModuleUri).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }


        public override ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(LocalModuleReference reference)
        {
            var localUri = fileResolver.TryResolveFilePath(reference.ParentModuleUri, reference.Path);
            if (localUri is null)
            {
                return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ParentModuleUri.LocalPath));
            }

            return new(localUri);
        }

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<LocalModuleReference> references)
        {
            // local modules are already present on the file system
            // and do not require init
            return Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>>(ImmutableDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>.Empty);
        }

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<LocalModuleReference> references)
        {
            // local modules are already present on the file system, there's no cache concept for this one
            // we do nothing
            return Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>>(ImmutableDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>.Empty);
        }

        public override bool IsArtifactRestoreRequired(LocalModuleReference reference) => false;

        public override Task PublishModule(LocalModuleReference moduleReference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Local modules cannot be published.");

        public override Task PublishProvider(LocalModuleReference reference, BinaryData typesTgz)
            => throw new NotSupportedException("Local providers cannot be published.");

        public override Task<bool> CheckArtifactExists(LocalModuleReference reference) => throw new NotSupportedException("Local modules cannot be published.");

        public override string? TryGetDocumentationUri(LocalModuleReference moduleReference) => null;

        public override async Task<string?> TryGetDescription(LocalModuleReference moduleReference)
        {
            try
            {
                if (this.TryGetLocalArtifactEntryPointUri(moduleReference).IsSuccess(out var localUri) && this.bicepCompiler is not null)
                {
                    var compilation = await this.bicepCompiler.CreateCompilation(localUri, skipRestore: true);
                    if (compilation.SourceFileGrouping.FileResultByUri.TryGetValue(localUri, out var result) &&
                        result.IsSuccess(out var source) &&
                        compilation.GetSemanticModel(source) is { } semanticModel)
                    {
                        return DescriptionHelper.TryGetFromSemanticModel(semanticModel);
                    }
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        public override ResultWithException<SourceArchive> TryGetSource(LocalModuleReference reference)
        {
            return new(new SourceNotAvailableException());
        }
    }
}
