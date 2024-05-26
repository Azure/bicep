// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Providers;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

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

        public override string Scheme => ArtifactReferenceSchemes.Local;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, LocalModuleReference reference)
            => artifactType switch
            {
                ArtifactType.Module => RegistryCapabilities.Default,
                ArtifactType.Provider => RegistryCapabilities.Publish,
                _ => throw new UnreachableException(),
            };

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

        public override async Task PublishProvider(LocalModuleReference reference, BinaryData typesTgz)
        {
            var archive = await ProviderV1Archive.Build(typesTgz);

            var fileUri = PathHelper.TryResolveFilePath(reference.ParentModuleUri, reference.Path)!;
            fileResolver.Write(fileUri, archive.ToStream());
        }

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, LocalModuleReference reference)
            => artifactType switch
            {
                ArtifactType.Module => throw new NotSupportedException("Local modules cannot be published."),
                ArtifactType.Provider => Task.FromResult(false),
                _ => throw new UnreachableException(),
            };

        public override string? TryGetDocumentationUri(LocalModuleReference moduleReference) => null;

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, LocalModuleReference moduleReference)
        {
            if (module.TryGetSemanticModel().TryUnwrap() is { } model)
            {
                return Task.FromResult(DescriptionHelper.TryGetFromSemanticModel(model));
            }

            return Task.FromResult<string?>(null);
        }

        public override ResultWithException<SourceArchive> TryGetSource(LocalModuleReference reference)
        {
            return new(new SourceNotAvailableException());
        }
    }
}
