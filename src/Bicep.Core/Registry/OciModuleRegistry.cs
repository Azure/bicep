// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Newtonsoft.Json;

namespace Bicep.Core.Registry
{
    public sealed class OciModuleRegistry : ExternalArtifactRegistry<OciModuleReference, OciArtifactResult>
    {
        private readonly AzureContainerRegistryManager client;

        private readonly string cachePath;

        private readonly RootConfiguration configuration;

        private readonly Uri parentModuleUri;

        private readonly IFeatureProvider features;

        private const string NewerVersionMightBeRequired = "A newer version of Bicep might be required to reference this artifact.";

        public OciModuleRegistry(
            IFileResolver FileResolver,
            IContainerRegistryClientFactory clientFactory,
            IFeatureProvider features,
            RootConfiguration configuration,
            Uri parentModuleUri)
            : base(FileResolver)
        {
            this.cachePath = Path.Combine(features.CacheRootDirectory, ModuleReferenceSchemes.Oci);
            this.client = new AzureContainerRegistryManager(clientFactory);
            this.configuration = configuration;
            this.features = features;
            this.parentModuleUri = parentModuleUri;
        }

        public override string Scheme => ModuleReferenceSchemes.Oci;

        public string CacheRootDirectory => this.features.CacheRootDirectory;

        public override RegistryCapabilities GetCapabilities(OciModuleReference reference)
        {
            // cannot publish without tag
            return reference.Tag is null ? RegistryCapabilities.Default : RegistryCapabilities.Publish;
        }

        public override ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(string? aliasName, string reference)
        {
            if (!OciModuleReference.TryParse(aliasName, reference, configuration, parentModuleUri).IsSuccess(out var @ref, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(@ref);
        }

        public override bool IsArtifactRestoreRequired(OciModuleReference reference)
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

            // TODO: Provider artifacts don't write a ModuleMain file, so this code is incorrect.
            //   That can be solved by only writing layer data files only (see code under features.PublishSourceEnabled)
            //   and not main.json directly (https://github.com/Azure/bicep/issues/11900)
            return
                !this.FileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.ModuleMain)) ||
                !this.FileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.Manifest)) ||
                !this.FileResolver.FileExists(this.GetModuleFileUri(reference, ModuleFileType.Metadata));
        }

        public override async Task<bool> CheckArtifactExists(OciModuleReference reference)
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
            catch (InvalidModuleException exception) when (
                exception.Kind == InvalidModuleExceptionKind.WrongArtifactType ||
                exception.Kind == InvalidModuleExceptionKind.WrongModuleLayerMediaType)
            {
                throw new ExternalArtifactException("An artifact with the tag already exists in the registry, but the artifact is not a Bicep file or module!", exception);
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

        private readonly ImmutableArray<string> allowedArtifactMediaTypes = ImmutableArray.Create(
            BicepMediaTypes.BicepModuleArtifactType,
            BicepMediaTypes.BicepProviderArtifactType);

        private readonly ImmutableArray<string> allowedConfigMediaTypes = ImmutableArray.Create(
            BicepMediaTypes.BicepModuleConfigV1,
            BicepMediaTypes.BicepProviderConfigV1);

        private readonly ImmutableArray<string> allowedMainLayerMediaTypes = ImmutableArray.Create(
            BicepMediaTypes.BicepModuleLayerV1Json,
            BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip);

        private (string mediaType, BinaryData data) getMainLayer(OciArtifactResult result)
        {
            if (result.Layers.Count() == 0)
            {
                throw new InvalidModuleException("Expected at least one layer in the OCI artifact.");
            }

            // Ignore layers we don't recognize for now.
            var mainLayers = result.Layers.Where(l => allowedMainLayerMediaTypes.Contains(l.MediaType, BicepMediaTypes.MediaTypeComparer)).ToArray();
            if (mainLayers.Count() == 0)
            {
                throw new InvalidModuleException($"Did not expect only layer media types {string.Join(", ", result.Layers.Select(l => l.MediaType).ToArray())}", InvalidModuleExceptionKind.WrongModuleLayerMediaType);
            }
            else if (mainLayers.Count() > 1)
            {
                throw new InvalidModuleException($"Did not expect to find multiple layer media types of {string.Join(", ", mainLayers.Select(l => l.MediaType).ToArray())}", InvalidModuleExceptionKind.WrongModuleLayerMediaType);
            }

            return mainLayers.Single();
        }

        private void ValidateModule(OciArtifactResult artifactResult)
        {
            var manifest = artifactResult.Manifest;
            var artifactType = manifest.ArtifactType;

            // (Bicep v0.20.0 and lower use null for this field, so assume valid in that case
            if (artifactType is not null &&
                !allowedArtifactMediaTypes.Contains(artifactType, BicepMediaTypes.MediaTypeComparer))
            {
                throw new InvalidModuleException(
                    $"Expected OCI manifest artifactType value of '{BicepMediaTypes.BicepModuleArtifactType}' but found '{artifactType}'. {NewerVersionMightBeRequired}",
                    InvalidModuleExceptionKind.WrongArtifactType);
            }
            var config = manifest.Config;
            var configMediaType = config.MediaType;
            if (configMediaType is not null &&
                !allowedConfigMediaTypes.Contains(configMediaType, BicepMediaTypes.MediaTypeComparer))
            {
                throw new InvalidModuleException($"Did not expect config media type \"{configMediaType}\". {NewerVersionMightBeRequired}");
            }

            // Verify nothing wrong with the layers we've been given
            _ = getMainLayer(artifactResult);

            // Note: We're not currently writing out non-zero config for modules but expect to soon (https://github.com/Azure/bicep/issues/11482).
            // So ignore the field for now and don't do any validation.
        }

        public override ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(OciModuleReference reference)
        {
            var localUri = this.GetModuleFileUri(reference, ModuleFileType.ModuleMain);
            return new(localUri);
        }

        public override string? TryGetDocumentationUri(OciModuleReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            if (ociAnnotations is null ||
                !ociAnnotations.TryGetValue(LanguageConstants.OciOpenContainerImageDocumentationAnnotation, out string? documentationUri)
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

        public override Task<string?> TryGetDescription(OciModuleReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            return Task.FromResult(DescriptionHelper.TryGetFromOciManifestAnnotations(ociAnnotations));
        }

        private OciManifest GetCachedManifest(OciModuleReference ociArtifactModuleReference)
        {
            string manifestFilePath = this.GetModuleFilePath(ociArtifactModuleReference, ModuleFileType.Manifest);

            try
            {
                string manifestFileContents = File.ReadAllText(manifestFilePath);
                OciManifest ociManifest = JsonConvert.DeserializeObject<OciManifest>(manifestFileContents)
                    ?? throw new Exception($"Deserialization of cached manifest \"{manifestFilePath}\" failed");
                return ociManifest;
            }
            catch (Exception ex)
            {
                throw new ExternalArtifactException($"Could not retrieve artifact manifest from \"{manifestFilePath}\"", ex);
            }
        }

        private ImmutableDictionary<string, string>? TryGetOciAnnotations(OciModuleReference ociArtifactModuleReference)
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

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<OciModuleReference> references)
        {
            var statuses = new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>();

            foreach (var reference in references)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference}");
                var (result, errorMessage) = await this.TryRestoreArtifactAsync(configuration, reference);

                if (result is null)
                {
                    if (errorMessage is not null)
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailedWithMessage(reference.FullyQualifiedReference, errorMessage));
                        timer.OnFail(errorMessage);
                    }
                    else
                    {
                        statuses.Add(reference, x => x.ModuleRestoreFailed(reference.FullyQualifiedReference));
                        timer.OnFail();
                    }
                }
            }

            return statuses;
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<OciModuleReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override async Task PublishArtifact(OciModuleReference moduleReference, Stream compiledArmTemplate, Stream? bicepSources, string? documentationUri, string? description)
        {
            // This needs to be valid JSON, otherwise there may be compatibility issues.
            // NOTE: Bicep v0.20 and earlier will throw on this, so it's a breaking change.
            var config = new StreamDescriptor(new MemoryStream(Encoding.UTF8.GetBytes("{}")), BicepMediaTypes.BicepModuleConfigV1);

            List<StreamDescriptor> layers = new List<StreamDescriptor>();
            layers.Add(new StreamDescriptor(compiledArmTemplate, BicepMediaTypes.BicepModuleLayerV1Json));
            if (bicepSources is { } && features.PublishSourceEnabled)
            {
                layers.Add(new StreamDescriptor(bicepSources, BicepMediaTypes.BicepSourceV1Layer));
            }

            var annotations = new OciManifestAnnotationsBuilder()
                .WithDescription(description)
                .WithDocumentationUri(documentationUri);

            try
            {
                await this.client.PushArtifactAsync(
                    configuration,
                    moduleReference,
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

        protected override void WriteArtifactContentToCache(OciModuleReference reference, OciArtifactResult result)
        {
            /*
             * this should be kept in sync with the IsModuleRestoreRequired() implementation
             * but beware that it's currently possible older versions of Bicep and newer versions of Bicep
             * may be sharing this cache on the same machine.
             */


            // write manifest
            // it's important to write the original stream here rather than serialize the manifest object
            // this way we guarantee the manifest hash will match
            var manifestFileUri = this.GetModuleFileUri(reference, ModuleFileType.Manifest);
            using var manifestStream = result.ToStream();
            this.FileResolver.Write(manifestFileUri, manifestStream);

            // write data file
            var mainLayer = getMainLayer(result);

            // NOTE(asilverman): currently the only difference in the processing is the filename written to disk
            // but this may change in the future if we chose to publish providers in multiple layers.
            // TODO: IsArtifactRestoreRequired assumes there must be a ModuleMain file, which isn't true for provider artifacts.
            //   That can be solved by only writing layer data files only (see code under features.PublishSourceEnabled)
            //   and not main.json directly (https://github.com/Azure/bicep/issues/11900)
            var moduleFileType = mainLayer.mediaType switch
            {
                BicepMediaTypes.BicepModuleLayerV1Json => ModuleFileType.ModuleMain,
                BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip => ModuleFileType.Provider,
                _ => throw new ArgumentException($"Unexpected layer mediaType \"{mainLayer.mediaType}\". This should have been caught in the {nameof(ValidateModule)} method.")
            };
            using var dataStream = mainLayer.data.ToStream();
            this.FileResolver.Write(this.GetModuleFileUri(reference, moduleFileType), dataStream);

            // write metadata
            var metadata = new ModuleMetadata(result.ManifestDigest);
            using var metadataStream = new MemoryStream();
            OciSerialization.Serialize(metadataStream, metadata);
            metadataStream.Position = 0;
            this.FileResolver.Write(this.GetModuleFileUri(reference, ModuleFileType.Metadata), metadataStream);

            // write source archive file
            // TODO: do we need to delete this file if there is no source layer?
            if (this.features.PublishSourceEnabled && result.GetSingleLayerByMediaType(BicepMediaTypes.BicepSourceV1Layer) is BinaryData sourceData)
            {
                // TODO: Write all layers as separate binary files instead of separate files for source.tar.gz and provider files.
                // We should do this rather than writing individual files we know about,
                //   (e.g. "source.tar.gz") because this way we can restore all layers even if we don't know what they're for.
                //   If an optional layer is added, we don't need to version the cache because all versions have the same complete
                //   info on disk and can handle the layer data as they want to.
                // The manifest can be used to determine what's in each layer file.
                //  (https://github.com/Azure/bicep/issues/11900)
                this.FileResolver.Write(this.GetModuleFileUri(reference, ModuleFileType.Source), sourceData.ToStream());
            }
        }

        protected override string GetArtifactDirectoryPath(OciModuleReference reference)
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

            //var packageDir = WebUtility.UrlEncode(reference.UnqualifiedReference);
            return Path.Combine(this.cachePath, registry, repository, tagOrDigest);
        }

        protected override Uri GetArtifactLockFileUri(OciModuleReference reference) => this.GetModuleFileUri(reference, ModuleFileType.Lock);

        private async Task<(OciArtifactResult?, string? errorMessage)> TryRestoreArtifactAsync(RootConfiguration configuration, OciModuleReference reference)
        {
            try
            {
                var result = await this.client.PullArtifactAsync(configuration, reference);
                // TODO(asilverman): Refactor validation to switch by mediaType
                ValidateModule(result);

                await this.WriteArtifactContentToCacheAsync(reference, result);

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

        private Uri GetModuleFileUri(OciModuleReference reference, ModuleFileType fileType)
        {
            string localFilePath = this.GetModuleFilePath(reference, fileType);
            if (Uri.TryCreate(localFilePath, UriKind.Absolute, out var uri))
            {
                return uri;
            }

            throw new NotImplementedException($"Local module file path is malformed: \"{localFilePath}\"");
        }

        private string GetModuleFilePath(OciModuleReference reference, ModuleFileType fileType)
        {
            var fileName = fileType switch
            {
                ModuleFileType.ModuleMain => "main.json",
                ModuleFileType.Lock => "lock",
                ModuleFileType.Manifest => "manifest",
                ModuleFileType.Metadata => "metadata",
                ModuleFileType.Provider => "types.tgz",
                ModuleFileType.Source => "source.tar.gz",
                _ => throw new NotImplementedException($"Unexpected module file type '{fileType}'.")
            };

            return Path.Combine(this.GetArtifactDirectoryPath(reference), fileName);
        }

        public override SourceArchive? TryGetSource(OciModuleReference reference)
        {
            var zipPath = GetModuleFilePath(reference, ModuleFileType.Source);
            if (File.Exists(zipPath))
            {
                return SourceArchive.FromStream(File.OpenRead(zipPath));
            }

            return null;
        }

        private enum ModuleFileType
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
