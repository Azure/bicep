// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public record LocalModuleEntity(ExtensionPackage Package);

    public class LocalModuleRegistry : ExternalArtifactRegistry<LocalModuleReference, LocalModuleEntity>
    {
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
            var fileHandle = reference.ReferencingFile.FileHandle.TryGetRelativeFile(reference.Path).Unwrap();
            fileHandle.Write(archive);
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

        private static IDirectoryHandle? TryGetArtifactDirectory(LocalModuleReference reference)
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

        private static BinaryData? TryReadContent(LocalModuleReference reference)
        {
            var moduleFileHandle = reference.ReferencingFile.FileHandle.TryGetRelativeFile(reference.Path).TryUnwrap();

            return moduleFileHandle?.TryReadBinaryData().TryUnwrap();
        }

        private IFileHandle GetTypesTgzFile(LocalModuleReference reference) => this.GetFile(reference, "types.tgz");

        private IFileHandle GetExtensionBinaryFile(LocalModuleReference reference) => this.GetFile(reference, "extension.bin");

        protected override IFileHandle GetArtifactLockFile(LocalModuleReference reference) => this.GetFile(reference, "lock");

        private IFileHandle GetFile(LocalModuleReference reference, string path) =>
            this.TryGetFile(reference, path) ??
            throw new InvalidOperationException($"Failed to resolve file for {reference.FullyQualifiedReference}.");

        private IFileHandle? TryGetFile(LocalModuleReference reference, string path) => TryGetArtifactDirectory(reference)?.GetFile(path);
    }
}
