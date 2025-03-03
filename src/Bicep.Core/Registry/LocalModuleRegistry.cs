// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public record LocalModuleEntity(ExtensionPackage Package);

    public class LocalModuleRegistry : ExternalArtifactRegistry<LocalModuleReference, LocalModuleEntity>
    {
        public LocalModuleRegistry(IFileResolver fileResolver)
            : base(fileResolver)
        {
        }

        public override string Scheme => ArtifactReferenceSchemes.Local;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, LocalModuleReference reference)
            => artifactType switch
            {
                ArtifactType.Module => RegistryCapabilities.Default,
                ArtifactType.Extension => RegistryCapabilities.Publish,
                _ => throw new UnreachableException(),
            };

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? alias, string reference)
        {
            if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
            {
                return new(x => x.UnsupportedArtifactType(artifactType));
            }

            if (!LocalModuleReference.TryParse(referencingFile, artifactType, reference).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }


        public override ResultWithDiagnosticBuilder<Uri> TryGetLocalArtifactEntryPointUri(LocalModuleReference reference)
        {
            var localUri = FileResolver.TryResolveFilePath(reference.ReferencingFile.Uri, reference.Path);
            if (localUri is null)
            {
                return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ReferencingFile.FileHandle.Uri));
            }

            if (reference.ArtifactType == ArtifactType.Extension)
            {
                if (this.TryGetTypesTgzFile(reference) is not { } tgzFile)
                {
                    return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ReferencingFile.FileHandle.Uri));
                }

                return new(tgzFile.Uri.ToUri());
            }

            return new(localUri);
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<LocalModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            foreach (var reference in references)
            {
                if (reference.ArtifactType == ArtifactType.Extension)
                {
                    if (TryReadContent(reference) is not { } binaryData)
                    {
                        statuses.Add(reference, x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, $"Failed to find {reference.FullyQualifiedReference}"));
                        continue;
                    }

                    var package = ExtensionV1Archive.Read(binaryData);
                    await this.WriteArtifactContentToCacheAsync(reference, new(package));
                }
            }

            return statuses;
        }

        public override Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<LocalModuleReference> references) =>
            base.InvalidateArtifactsCacheInternal(references);

        public override bool IsArtifactRestoreRequired(LocalModuleReference reference) =>
            reference.ArtifactType == ArtifactType.Extension && !this.GetTypesTgzFile(reference).Exists();

        public override Task PublishModule(LocalModuleReference moduleReference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description) =>
            throw new NotSupportedException("Local modules cannot be published.");

        public override async Task PublishExtension(LocalModuleReference reference, ExtensionPackage package)
        {
            var archive = await ExtensionV1Archive.Build(package);
            var fileUri = PathHelper.TryResolveFilePath(reference.ReferencingFile.Uri, reference.Path)!;
            FileResolver.Write(fileUri, archive.ToStream());
        }

        public override Task<bool> CheckArtifactExists(ArtifactType artifactType, LocalModuleReference reference)
            => artifactType switch
            {
                ArtifactType.Module => throw new NotSupportedException("Local modules cannot be published."),
                ArtifactType.Extension => Task.FromResult(false),
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

        public override Uri? TryGetExtensionBinary(LocalModuleReference reference) => GetExtensionBinaryFile(reference).Uri.ToUri();

        protected override void WriteArtifactContentToCache(LocalModuleReference reference, LocalModuleEntity entity)
        {
            if (entity.Package.LocalDeployEnabled)
            {
                if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                {
                    throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{reference}\".");
                }

                if (entity.Package.Binaries.SingleOrDefault(x => x.Architecture.Name == architecture.Name) is not { } binary)
                {
                    throw new InvalidOperationException($"The extension \"{reference}\" does not support architecture {architecture.Name}.");
                }

                var binaryFile = GetExtensionBinaryFile(reference);

                binaryFile.Write(binary.Data);
                binaryFile.MakeExecutable();
            }

            this.GetTypesTgzFile(reference).Write(entity.Package.Types);
        }

        private IDirectoryHandle? TryGetArtifactDirectory(LocalModuleReference reference)
        {
            if (TryReadContent(reference) is not { } binaryData)
            {
                return null;
            }

            // Extension packages are unpacked to '~/.bicep/local/sha256_<digest>'.
            // We must use '_' as a separator here because Windows does not allow ':' in file paths.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, binaryData, separator: '_');

            return reference.ReferencingFile.Features.CacheRootDirectory.GetDirectory($"local/{digest}");
        }

        protected override IDirectoryHandle GetArtifactDirectory(LocalModuleReference reference)
        {
            if (TryGetArtifactDirectory(reference) is not { } directory)
            {
                throw new InvalidOperationException($"Failed to resolve file path for {reference.FullyQualifiedReference}");
            }

            return directory;
        }

        private BinaryData? TryReadContent(LocalModuleReference reference)
        {
            if (FileResolver.TryResolveFilePath(reference.ReferencingFile.Uri, reference.Path) is not { } fileUri ||
                FileResolver.TryReadAsBinaryData(fileUri).TryUnwrap() is not { } binaryData)
            {
                return null;
            }

            return binaryData;
        }

        private IFileHandle GetTypesTgzFile(LocalModuleReference reference) => this.GetFile(reference, "types.tgz");

        private IFileHandle? TryGetTypesTgzFile(LocalModuleReference reference) => this.TryGetFile(reference, "types.tgz");

        private IFileHandle GetExtensionBinaryFile(LocalModuleReference reference) => this.GetFile(reference, "extension.bin");

        protected override IFileHandle GetArtifactLockFile(LocalModuleReference reference) => this.GetFile(reference, "lock");

        private IFileHandle GetFile(LocalModuleReference reference, string path) =>
            this.TryGetFile(reference, path) ??
            throw new InvalidOperationException($"Failed to resolve file for {reference.FullyQualifiedReference}.");

        private IFileHandle? TryGetFile(LocalModuleReference reference, string path) => this.TryGetArtifactDirectory(reference)?.GetFile(path);
    }
}
