// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Abstractions;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Core.Registry
{
    public sealed class OciArtifactRegistry : ExternalArtifactRegistry<OciArtifactReference, OciArtifactResult>
    {
        private readonly AzureContainerRegistryManager client;

        private readonly string cachePath;

        private readonly RootConfiguration configuration;

        private readonly Uri parentModuleUri;

        private readonly IFeatureProvider features;

        public OciArtifactRegistry(
            IFileResolver FileResolver,
            IFileSystem fileSystem,
            IContainerRegistryClientFactory clientFactory,
            IFeatureProvider features,
            RootConfiguration configuration,
            Uri parentModuleUri)
            : base(FileResolver, fileSystem)
        {
            this.cachePath = FileSystem.Path.Combine(features.CacheRootDirectory, ArtifactReferenceSchemes.Oci);
            this.client = new AzureContainerRegistryManager(clientFactory);
            this.configuration = configuration;
            this.features = features;
            this.parentModuleUri = parentModuleUri;
        }

        public override string Scheme => ArtifactReferenceSchemes.Oci;

        public string CacheRootDirectory => this.features.CacheRootDirectory;

        public override RegistryCapabilities GetCapabilities(OciArtifactReference reference)
        {
            // cannot publish without tag
            return reference.Tag is null ? RegistryCapabilities.Default : RegistryCapabilities.Publish;
        }

        public override ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType artifactType, string? aliasName, string reference)
        {
            if (!OciArtifactReference.TryParse(artifactType, aliasName, reference, configuration, parentModuleUri).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }
            return new(@ref);
        }


        public override bool IsArtifactRestoreRequired(OciArtifactReference reference)
        {
            /*
             * this should be kept in sync with the WriteModuleContent() implementation
             * but beware that it's possible older versions of Bicep and newer versions of Bicep
             * may be sharing this cache on the same machine.
             *
             * when we write content to the module cache, we attempt to get a lock so that no other writes happen in the directory
             * the code here appears to implement a lock-free read by checking existence of several files that are expected in a fully restored module
             *
             * this relies on the assumption that modules are never updated in-place in the cache
             * when we need to invalidate the cache, the module directory (or even a single file) should be deleted from the cache
             */

            var artifactFilesNotFound = reference.Type switch
            {
                ArtifactType.Module => !this.FileResolver.FileExists(this.GetArtifactFileUri(reference, ArtifactFileType.ModuleMain)),
                ArtifactType.Provider => !this.FileResolver.FileExists(this.GetArtifactFileUri(reference, ArtifactFileType.Provider)),
                _ => throw new UnreachableException()
            };

            return artifactFilesNotFound ||
                !this.FileResolver.FileExists(this.GetArtifactFileUri(reference, ArtifactFileType.Manifest)) ||
                !this.FileResolver.FileExists(this.GetArtifactFileUri(reference, ArtifactFileType.Metadata));
        }

        public override async Task<bool> CheckArtifactExists(OciArtifactReference reference)
        {
            try
            {
                // Get module
                await this.client.PullArtifactAsync(configuration, reference);
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                // Found module but tag doesn't exist
                return false;
            }
            catch (ExternalArtifactException exception) when
                (exception.InnerException is RequestFailedException &&
                ((RequestFailedException)exception.InnerException).Status == 404)
            {
                // Found no module at all
                return false;
            }
            catch (InvalidArtifactException exception) when (
                exception.Kind == InvalidArtifactExceptionKind.WrongArtifactType ||
                exception.Kind == InvalidArtifactExceptionKind.UnknownLayerMediaType)
            {
                throw new ExternalArtifactException($"The artifact referenced by {reference.FullyQualifiedReference} was not downloaded from the registry because it is invalid.", exception);
            }
            catch (RequestFailedException exception)
            {
                throw new ExternalArtifactException(exception.Message, exception);
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException(exception.Message, exception);
            }

            return true;
        }

        public override ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(OciArtifactReference reference)
        {
            var artifactFileType = reference.Type switch
            {
                ArtifactType.Module => ArtifactFileType.ModuleMain,
                ArtifactType.Provider => ArtifactFileType.Provider,
                _ => throw new UnreachableException()
            };

            var localUri = this.GetArtifactFileUri(reference, artifactFileType);
            return new(localUri);
        }

        public override string? TryGetDocumentationUri(OciArtifactReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            if (ociAnnotations is null ||
                !ociAnnotations.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, out string? documentationUri)
                || string.IsNullOrWhiteSpace(documentationUri))
            {
                // Automatically generate a help URI for public MCR modules
                if (ociArtifactModuleReference.Registry == LanguageConstants.BicepPublicMcrRegistry && ociArtifactModuleReference.Repository.StartsWith(LanguageConstants.McrRepositoryPrefix, StringComparison.Ordinal))
                {
                    var moduleName = ociArtifactModuleReference.Repository.Substring(LanguageConstants.McrRepositoryPrefix.Length);
                    return ociArtifactModuleReference.Tag is null ? null : GetPublicBicepModuleDocumentationUri(moduleName, ociArtifactModuleReference.Tag);
                }

                return null;
            }

            return documentationUri;
        }

        /// <summary>
        /// Automatically generate a help URI for public MCR modules
        /// </summary>
        /// <param name="publicModuleName">e.g. app/dapr-containerapp</param>
        /// <param name="tag">e.g. 1.0.1</param>
        public static string GetPublicBicepModuleDocumentationUri(string publicModuleName, string tag)
        {
            // Example: https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapp/1.0.2/modules/app/dapr-containerapp/README.md
            return $"https://github.com/Azure/bicep-registry-modules/tree/{publicModuleName}/{tag}/modules/{publicModuleName}/README.md";
        }

        public override Task<string?> TryGetModuleDescription(ModuleSymbol module, OciArtifactReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            return Task.FromResult(DescriptionHelper.TryGetFromOciManifestAnnotations(ociAnnotations));
        }

        private OciManifest GetCachedManifest(OciArtifactReference ociArtifactModuleReference)
        {
            string manifestFilePath = this.GetArtifactFilePath(ociArtifactModuleReference, ArtifactFileType.Manifest);

            try
            {
                string manifestFileContents = FileSystem.File.ReadAllText(manifestFilePath);
                var manifest = JsonSerializer.Deserialize(manifestFileContents, OciManifestSerializationContext.Default.OciManifest);
                return manifest ?? throw new Exception($"Deserialization of cached manifest \"{manifestFilePath}\" failed");
            }
            catch (Exception ex)
            {
                throw new ExternalArtifactException($"Could not retrieve artifact manifest from \"{manifestFilePath}\"", ex);
            }
        }

        private ImmutableDictionary<string, string>? TryGetOciAnnotations(OciArtifactReference ociArtifactModuleReference)
        {
            try
            {
                return GetCachedManifest(ociArtifactModuleReference).Annotations;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactReference> references)
        {
            var failures = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {GetArtifactDirectoryPath(reference)}");
                var (result, errorMessage) = await this.TryRestoreArtifactAsync(configuration, reference);

                if (result is null)
                {
                    if (errorMessage is not null)
                    {
                        failures.Add(reference, x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, errorMessage));
                        timer.OnFail(errorMessage);
                    }
                    else
                    {
                        failures.Add(reference, x => x.ArtifactRestoreFailed(reference.FullyQualifiedReference));
                        timer.OnFail();
                    }
                }
            }

            return failures;
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<OciArtifactReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override async Task PublishModule(OciArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description)
        {
            // This needs to be valid JSON, otherwise there may be compatibility issues.
            // NOTE: Bicep v0.20 and earlier will throw on this, so it's a breaking change.
            var config = new Oci.OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1);

            List<Oci.OciDescriptor> layers = new()
            {
                new(compiledArmTemplate, BicepMediaTypes.BicepModuleLayerV1Json, new OciManifestAnnotationsBuilder().WithTitle("Compiled ARM template").Build())
            };

            if (bicepSources is { } && features.PublishSourceEnabled)
            {
                layers.Add(
                    new(
                        bicepSources,
                        BicepMediaTypes.BicepSourceV1Layer,
                        new OciManifestAnnotationsBuilder().WithTitle("Source files").Build()));
            }

            var annotations = new OciManifestAnnotationsBuilder()
                .WithDescription(description)
                .WithDocumentationUri(documentationUri)
                .WithCreatedTime(DateTime.Now);

            try
            {
                await this.client.PushArtifactAsync(
                    configuration,
                    reference,
                    // Technically null should be fine for mediaType, but ACR guys recommend OciImageManifest for safer compatibility
                    ManifestMediaType.OciImageManifest.ToString(),
                    BicepMediaTypes.BicepModuleArtifactType,
                    config,
                    layers,
                    annotations);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                // will include several retry messages, but likely the best we can do
                throw new ExternalArtifactException(exception.Message, exception);
            }
            catch (RequestFailedException exception)
            {
                // can only happen if client retries are disabled
                throw new ExternalArtifactException(exception.Message, exception);
            }
        }

        public override async Task PublishProvider(OciArtifactReference reference, BinaryData typesTgz)
        {
            // This needs to be valid JSON, otherwise there may be compatibility issues.
            var config = new Oci.OciDescriptor("{}", BicepMediaTypes.BicepProviderConfigV1);

            List<Oci.OciDescriptor> layers = new()
            {
                new(typesTgz, BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip, new OciManifestAnnotationsBuilder().WithTitle("types.tgz").Build())
            };

            var annotations = new OciManifestAnnotationsBuilder()
                .WithBicepSerializationFormatV1()
                .WithCreatedTime(DateTime.Now);

            try
            {
                await this.client.PushArtifactAsync(
                    configuration,
                    reference,
                    // Technically null should be fine for mediaType, but ACR guys recommend OciImageManifest for safer compatibility
                    ManifestMediaType.OciImageManifest.ToString(),
                    BicepMediaTypes.BicepProviderArtifactType,
                    config,
                    layers,
                    annotations);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                // will include several retry messages, but likely the best we can do
                throw new ExternalArtifactException(exception.Message, exception);
            }
            catch (RequestFailedException exception)
            {
                // can only happen if client retries are disabled
                throw new ExternalArtifactException(exception.Message, exception);
            }
        }

        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

        protected override void WriteArtifactContentToCache(OciArtifactReference reference, OciArtifactResult result)
        {
            /*
                * this should be kept in sync with the IsModuleRestoreRequired() implementation
                * but beware that it's currently possible older versions of Bicep and newer versions of Bicep
                * may be sharing this cache on the same machine.
                */

            // write manifest
            // it's important to write the original stream here rather than serialize the manifest object
            // this way we guarantee the manifest hash will match
            var manifestFileUri = this.GetArtifactFileUri(reference, ArtifactFileType.Manifest);
            using var manifestStream = result.ToStream();
            this.FileResolver.Write(manifestFileUri, manifestStream);

            // write data file
            var mainLayer = result.GetMainLayer();

            // NOTE(asilverman): currently the only difference in the processing is the filename written to disk
            // but this may change in the future if we chose to publish providers in multiple layers.
            // TODO: IsArtifactRestoreRequired assumes there must be a ModuleMain file, which isn't true for provider artifacts
            // NOTE(stephenWeatherford): That can be solved by only writing layer data files only (see below CONSIDER)
            //   and not main.json directly (https://github.com/Azure/bicep/issues/11900)
            var moduleFileType = (reference.Type, result) switch
            {
                (ArtifactType.Module, OciModuleArtifactResult) => ArtifactFileType.ModuleMain,
                (ArtifactType.Module, OciProviderArtifactResult) => throw new InvalidArtifactException($"Expected a module, but retrieved a provider."),
                (ArtifactType.Provider, OciProviderArtifactResult) => ArtifactFileType.Provider,
                (ArtifactType.Provider, OciModuleArtifactResult) => throw new InvalidArtifactException($"Expected a provider, but retrieved a module."),
                _ => throw new InvalidOperationException($"Unexpected artifact type \"{result.GetType().Name}\"."),
            };

            using var dataStream = mainLayer.Data.ToStream();
            this.FileResolver.Write(this.GetArtifactFileUri(reference, moduleFileType), dataStream);

            if (result is OciModuleArtifactResult moduleArtifact)
            {
                // write source archive file
                if (result.TryGetSingleLayerByMediaType(BicepMediaTypes.BicepSourceV1Layer) is BinaryData sourceData)
                {
                    // CONSIDER: Write all layers as separate binary files instead of separate files for source.tgz and provider files.
                    // We should do this rather than writing individual files we know about,
                    //   (e.g. "source.tgz") because this way we can restore all layers even if we don't know what they're for.
                    //   If an optional layer is added, we don't need to version the cache because all versions have the same complete
                    //   info on disk and can handle the layer data as they want to.
                    // The manifest can be used to determine what's in each layer file.
                    //  (https://github.com/Azure/bicep/issues/11900)
                    this.FileResolver.Write(this.GetArtifactFileUri(reference, ArtifactFileType.Source), sourceData.ToStream());
                }
            }

            // write metadata
            var metadata = new ModuleMetadata(result.ManifestDigest);
            using var metadataStream = new MemoryStream();
            OciSerialization.Serialize(metadataStream, metadata);
            metadataStream.Position = 0;
            this.FileResolver.Write(this.GetArtifactFileUri(reference, ArtifactFileType.Metadata), metadataStream);
        }

        protected override string GetArtifactDirectoryPath(OciArtifactReference reference)
        {
            // cachePath is already set to %userprofile%\.bicep\br or ~/.bicep/br by default depending on OS
            // we need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac

            // TODO: Need to deal with IDNs (internationalized domain names)
            // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
            // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
            // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
            // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
            // we will also normalize casing for registries since they are case-insensitive
            var registry = reference.Registry.Replace(':', '$').ToLowerInvariant();

            // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
            // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
            // the replacement char must one that is not valid in a repository string but is valid in common type systems
            var repository = reference.Repository.Replace('/', '$');

            string? tagOrDigest;
            if (reference.Tag is not null)
            {
                // tags are case-sensitive with length up to 128
                tagOrDigest = TagEncoder.Encode(reference.Tag);
            }
            else if (reference.Digest is not null)
            {
                // digests are strings like "sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
                // and are already guaranteed to be lowercase
                // the only problematic character is the : which we will replace with a #
                // however the encoding we choose must not be ambiguous with the tag encoding
                tagOrDigest = reference.Digest.Replace(':', '#');
            }
            else
            {
                throw new InvalidOperationException("Module reference is missing both tag and digest.");
            }

            return FileSystem.Path.Combine(this.cachePath, registry, repository, tagOrDigest);
        }

        protected override Uri GetArtifactLockFileUri(OciArtifactReference reference) => this.GetArtifactFileUri(reference, ArtifactFileType.Lock);

        private async Task<(OciArtifactResult?, string? errorMessage)> TryRestoreArtifactAsync(RootConfiguration configuration, OciArtifactReference reference)
        {
            try
            {
                var result = await client.PullArtifactAsync(configuration, reference);

                await WriteArtifactContentToCacheAsync(reference, result);

                return (result, null);
            }
            catch (ExternalArtifactException exception)
            {
                // we can trust the message in this exception
                return (null, exception.Message);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                // the message on this one is not great because it includes all the retry attempts
                // however, we don't really have a good way to classify them in a cross-platform way
                return (null, exception.Message);
            }
            catch (RequestFailedException exception)
            {
                // this can only happen if we disable retry on the client and a registry request failed
                return (null, exception.Message);
            }
            catch (Exception exception)
            {
                return (null, $"Unhandled exception: {exception}");
            }
        }

        private static bool CheckAllInnerExceptionsAreRequestFailures(AggregateException exception) =>
            exception.InnerExceptions.All(inner => inner is RequestFailedException);

        private Uri GetArtifactFileUri(OciArtifactReference reference, ArtifactFileType fileType)
        {
            string localFilePath = this.GetArtifactFilePath(reference, fileType);
            if (Uri.TryCreate(localFilePath, UriKind.Absolute, out var uri))
            {
                return uri;
            }

            throw new NotImplementedException($"Local artifact file path is malformed: \"{localFilePath}\"");
        }

        private string GetArtifactFilePath(OciArtifactReference reference, ArtifactFileType fileType)
        {
            var fileName = fileType switch
            {
                ArtifactFileType.ModuleMain => "main.json",
                ArtifactFileType.Lock => "lock",
                ArtifactFileType.Manifest => "manifest",
                ArtifactFileType.Metadata => "metadata",
                ArtifactFileType.Provider => "types.tgz",
                ArtifactFileType.Source => "source.tgz",
                _ => throw new NotImplementedException($"Unexpected artifact file type '{fileType}'.")
            };

            return FileSystem.Path.Combine(this.GetArtifactDirectoryPath(reference), fileName);
        }

        public override ResultWithException<SourceArchive> TryGetSource(OciArtifactReference reference)
        {
            var zipPath = GetArtifactFilePath(reference, ArtifactFileType.Source);
            if (FileSystem.File.Exists(zipPath))
            {
                return SourceArchive.UnpackFromStream(FileSystem.File.OpenRead(zipPath));
            }

            // No sources available (presumably they weren't published)
            return new(new SourceNotAvailableException());
        }

        private enum ArtifactFileType
        {
            ModuleMain,
            Manifest,
            Lock,
            Metadata,
            Provider,
            Source,
        };
    }
}
