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
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Utils;

namespace Bicep.Core.Registry
{
    public record LocalModuleEntity(ExtensionPackage Package);

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
                ArtifactType.Extension => RegistryCapabilities.Publish,
                _ => throw new UnreachableException(),
            };

        public override ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? alias, string reference)
        {
            if (artifactType != ArtifactType.Module && artifactType != ArtifactType.Extension)
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
            var localUri = FileResolver.TryResolveFilePath(reference.ParentModuleUri, reference.Path);
            if (localUri is null)
            {
                return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ParentModuleUri.LocalPath));
            }

            if (reference.ArtifactType == ArtifactType.Extension)
            {
                if (TryGetTypesTgzUri(reference) is null)
                {
                    return new(x => x.FilePathCouldNotBeResolved(reference.Path, reference.ParentModuleUri.LocalPath));
                }

                return new(GetTypesTgzUri(reference));
            }

            return new(localUri);
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<LocalModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

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

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<LocalModuleReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override bool IsArtifactRestoreRequired(LocalModuleReference reference)
        {
            if (reference.ArtifactType != ArtifactType.Extension)
            {
                return false;
            }

            return !this.FileResolver.FileExists(this.GetTypesTgzUri(reference));
        }

        public override Task PublishModule(LocalModuleReference moduleReference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description)
            => throw new NotSupportedException("Local modules cannot be published.");

        public override async Task PublishExtension(LocalModuleReference reference, ExtensionPackage package)
        {
            var archive = await ExtensionV1Archive.Build(package);

            var fileUri = PathHelper.TryResolveFilePath(reference.ParentModuleUri, reference.Path)!;
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

        public override Uri? TryGetExtensionBinary(LocalModuleReference reference)
            => GetExtensionBinaryUri(reference);

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

                var binaryUri = GetExtensionBinaryUri(reference);
                this.FileResolver.Write(binaryUri, binary.Data.ToStream());
                if (!OperatingSystem.IsWindows())
                {
                    this.FileSystem.File.SetUnixFileMode(binaryUri.LocalPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }
            }

            var typesUri = this.GetTypesTgzUri(reference);
            this.FileResolver.Write(typesUri, entity.Package.Types.ToStream());
        }

        private string? TryGetArtifactDirectoryPath(LocalModuleReference reference)
        {
            if (TryReadContent(reference) is not { } binaryData)
            {
                return null;
            }

            // Extension packages are unpacked to '~/.bicep/local/sha256_<digest>'.
            // We must use '_' as a separator here because Windows does not allow ':' in file paths.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, binaryData, separator: '_');

            return FileSystem.Path.Combine(
                this.featureProvider.CacheRootDirectory,
                "local",
                digest);
        }

        protected override string GetArtifactDirectoryPath(LocalModuleReference reference)
        {
            if (TryGetArtifactDirectoryPath(reference) is not { } path)
            {
                throw new InvalidOperationException($"Failed to resolve file path for {reference.FullyQualifiedReference}");
            }
            
            return path;
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

        private Uri? TryGetTypesTgzUri(LocalModuleReference reference) => TryGetFileUri(reference, "types.tgz");

        private Uri GetExtensionBinaryUri(LocalModuleReference reference) => GetFileUri(reference, "extension.bin");

        protected override Uri GetArtifactLockFileUri(LocalModuleReference reference) => GetFileUri(reference, "lock");

        private Uri GetFileUri(LocalModuleReference reference, string path)
            => TryGetFileUri(reference, path) ?? throw new InvalidOperationException($"Failed to resolve file path for {reference.FullyQualifiedReference}");

        private Uri? TryGetFileUri(LocalModuleReference reference, string path)
            => TryGetArtifactDirectoryPath(reference) is {} directoryPath ? 
            new(FileSystem.Path.Combine(directoryPath, path), UriKind.Absolute) :
            null;
    }
}
