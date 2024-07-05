// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Providers;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public record LocalModuleEntity(ProviderPackage Provider);

    public class LocalModuleRegistry : ExternalArtifactRegistry<LocalModuleReference, LocalModuleEntity>
    {
        private readonly IFeatureProvider featureProvider;
        private readonly Uri parentModuleUri;

        public LocalModuleRegistry(IFileResolver fileResolver, IFileSystem fileSystem, IFeatureProvider featureProvider, Uri parentModuleUri)
            : base(fileResolver, fileSystem)
        {
            this.featureProvider = featureProvider;
            this.parentModuleUri = parentModuleUri;
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

            if (!LocalModuleReference.TryParse(artifactType, reference, parentModuleUri).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }


        public override ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(LocalModuleReference reference)
        {
            if (reference.ArtifactType == ArtifactType.Provider)
            {
                return new(GetTypesTgzUri(reference));
            }

            var localUri = FileResolver.TryResolveFilePath(reference.ParentModuleUri, reference.Path);
            if (localUri is null)
            {
                return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ParentModuleUri.LocalPath));
            }

            return new(localUri);
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<LocalModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                if (reference.ArtifactType == ArtifactType.Provider)
                {
                    if (TryReadContent(reference) is not { } binaryData)
                    {
                        statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, $"Failed to find {reference.FullyQualifiedReference}"));
                        continue;
                    }

                    var package = ProviderV1Archive.Read(binaryData);
                    await this.WriteArtifactContentToCacheAsync(reference, new(package));
                }
            }

            return statuses;
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<LocalModuleReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override bool IsArtifactRestoreRequired(LocalModuleReference reference)
        {
            if (reference.ArtifactType != ArtifactType.Provider)
            {
                return false;
            }

            return !this.FileResolver.FileExists(this.GetTypesTgzUri(reference));
        }

        public override Task PublishModule(LocalModuleReference moduleReference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Local modules cannot be published.");

        public override async Task PublishProvider(LocalModuleReference reference, ProviderPackage provider)
        {
            var archive = await ProviderV1Archive.Build(provider);

            var fileUri = PathHelper.TryResolveFilePath(reference.ParentModuleUri, reference.Path)!;
            FileResolver.Write(fileUri, archive.ToStream());
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

        public override Uri? TryGetProviderBinary(LocalModuleReference reference)
            => GetProviderBinUri(reference);

        protected override void WriteArtifactContentToCache(LocalModuleReference reference, LocalModuleEntity entity)
        {
            if (entity.Provider.LocalDeployEnabled)
            {
                if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                {
                    throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute provider extension \"{reference}\".");
                }

                if (entity.Provider.Binaries.SingleOrDefault(x => x.Architecture.Name == architecture.Name) is not { } binary)
                {
                    throw new InvalidOperationException($"The provider extension \"{reference}\" does not support architecture {architecture.Name}.");
                }

                var binaryUri = GetProviderBinUri(reference);
                this.FileResolver.Write(binaryUri, binary.Data.ToStream());
                if (!OperatingSystem.IsWindows())
                {
                    this.FileSystem.File.SetUnixFileMode(binaryUri.LocalPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }
            }

            var typesUri = this.GetTypesTgzUri(reference);
            this.FileResolver.Write(typesUri, entity.Provider.Types.ToStream());
        }

        protected override string GetArtifactDirectoryPath(LocalModuleReference reference)
        {
            if (TryReadContent(reference) is not { } binaryData)
            {
                throw new InvalidOperationException($"Failed to resolve file path for {reference.FullyQualifiedReference}");
            }

            // Provider packages are unpacked to '~/.bicep/local/sha256_<digest>'.
            // We must use '_' as a separator here because Windows does not allow ':' in file paths.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, binaryData, separator: '_');

            return FileSystem.Path.Combine(
                this.featureProvider.CacheRootDirectory,
                "local",
                digest);
        }

        private BinaryData? TryReadContent(LocalModuleReference reference)
        {
            if (FileResolver.TryResolveFilePath(reference.ParentModuleUri, reference.Path) is not { } fileUri ||
                FileResolver.TryReadAsBinaryData(fileUri).TryUnwrap() is not { } binaryData)
            {
                return null;
            }

            return binaryData;
        }

        private Uri GetTypesTgzUri(LocalModuleReference reference) => GetFileUri(reference, "types.tgz");

        private Uri GetProviderBinUri(LocalModuleReference reference) => GetFileUri(reference, "provider.bin");

        protected override Uri GetArtifactLockFileUri(LocalModuleReference reference) => GetFileUri(reference, "lock");

        private Uri GetFileUri(LocalModuleReference reference, string path)
            => new(FileSystem.Path.Combine(this.GetArtifactDirectoryPath(reference), path), UriKind.Absolute);
    }
}
