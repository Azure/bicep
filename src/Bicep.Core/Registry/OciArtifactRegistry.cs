// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;
using OrasProject.Oras.Exceptions;
using RegistryOciDescriptor = Bicep.Core.Registry.Oci.OciDescriptor;
using SessionRegistryArtifact = Bicep.Core.Registry.Sessions.RegistryArtifact;

namespace Bicep.Core.Registry
{
    public sealed class OciArtifactRegistry : ExternalArtifactRegistry<OciArtifactReference, OciArtifactResult>
    {
        private readonly IOciRegistryTransportFactory transportFactory;

        private readonly IPublicModuleMetadataProvider publicModuleMetadataProvider;

        private readonly ICredentialChain credentialChain;

        private readonly ILogger<OciArtifactRegistry> logger;

        public OciArtifactRegistry(
            IOciRegistryTransportFactory transportFactory,
            IPublicModuleMetadataProvider publicModuleMetadataProvider,
            ICredentialChain credentialChain,
            ILogger<OciArtifactRegistry> logger)
        {
            this.transportFactory = transportFactory;
            this.publicModuleMetadataProvider = publicModuleMetadataProvider;
            this.credentialChain = credentialChain;
            this.logger = logger;
        }

        public override string Scheme => ArtifactReferenceSchemes.Oci;

        public override RegistryCapabilities GetCapabilities(ArtifactType artifactType, OciArtifactReference reference)
        {
            // cannot publish without tag
            return reference.Tag is null ? RegistryCapabilities.Default : RegistryCapabilities.Publish;
        }

        public override ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType artifactType, string? aliasName, string reference)
        {
            if (!OciArtifactReference.TryParse(referencingFile, artifactType, aliasName, reference).IsSuccess(out var @ref, out var failureBuilder))
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
                ArtifactType.Module => !this.GetArtifactFile(reference, ArtifactFileType.ModuleMain).Exists(),
                ArtifactType.Extension => !this.GetArtifactFile(reference, ArtifactFileType.Extension).Exists(),
                _ => throw new UnreachableException()
            };

            return artifactFilesNotFound ||
                !this.GetArtifactFile(reference, ArtifactFileType.Manifest).Exists() ||
                !this.GetArtifactFile(reference, ArtifactFileType.Metadata).Exists();
        }

        public override async Task<bool> CheckArtifactExists(ArtifactType artifactType, OciArtifactReference reference)
        {
            if (reference.ReferencingFile.Features.OciEnabled)
            {
                var session = TryCreateSession(reference) ?? throw new ExternalArtifactException($"Registry provider for '{reference.Registry}' does not support OCI sessions.");

                await using var disposableSession = session;
                try
                {
                    await disposableSession.ResolveAsync(CreateRegistryRef(reference), CancellationToken.None).ConfigureAwait(false);
                    return true;
                }
                catch (NotFoundException)
                {
                    return false;
                }
                catch (Exception exception) when (exception is ExternalArtifactException or RequestFailedException)
                {
                    return false;
                }
                catch (InvalidArtifactException exception)
                {
                    throw new ExternalArtifactException($"The artifact referenced by {reference.FullyQualifiedReference} was not downloaded from the registry because it is invalid.", exception);
                }
            }

            var transport = transportFactory.GetTransport(reference);

            try
            {
                await transport.PullArtifactAsync(reference.ReferencingFile.Configuration.Cloud, reference);
                return true;
            }
            catch (RequestFailedException exception) when (exception.Status == 404)
            {
                return false;
            }
            catch (ExternalArtifactException exception) when (IsNotFoundException(exception.InnerException))
            {
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
        }

        public override string? TryGetDocumentationUri(OciArtifactReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            if (ociAnnotations is null ||
                !ociAnnotations.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, out string? documentationUri)
                || string.IsNullOrWhiteSpace(documentationUri))
            {
                // Automatically generate a help URI for public MCR modules
                if (ociArtifactModuleReference.Registry == LanguageConstants.BicepPublicMcrRegistry && ociArtifactModuleReference.Repository.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.Ordinal))
                {
                    var moduleName = ociArtifactModuleReference.Repository.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length);
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
            var manifestFile = this.GetArtifactFile(ociArtifactModuleReference, ArtifactFileType.Manifest);

            try
            {
                using var manifestFileStream = manifestFile.OpenRead();
                var manifest = JsonSerializer.Deserialize(manifestFileStream, OciManifestSerializationContext.Default.OciManifest);

                return manifest ?? throw new Exception($"Deserialization of cached manifest \"{manifestFile.Uri}\" failed");
            }
            catch (Exception ex)
            {
                throw new ExternalArtifactException($"Could not retrieve artifact manifest from \"{manifestFile.Uri}\"", ex);
            }
        }

    private ImmutableDictionary<string, string>? TryGetOciAnnotations(OciArtifactReference reference)
    {
        try
        {
            return GetCachedManifest(reference).Annotations;
        }
        catch (Exception)
        {
            if (!reference.ReferencingFile.Features.OciEnabled)
            {
                return null;
            }

            var session = TryCreateSession(reference);
            if (session is null)
            {
                throw new ExternalArtifactException($"Registry provider for '{reference.Registry}' does not support OCI sessions.");
            }

            try
            {
#pragma warning disable VSTHRD002
                var info = session.ResolveAsync(CreateRegistryRef(reference), CancellationToken.None).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
                return info.Annotations?.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase) ?? ImmutableDictionary<string, string>.Empty;
            }
            finally
            {
#pragma warning disable VSTHRD002
                session.DisposeAsync().AsTask().GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
            }
        }
    }

        public override async Task OnRestoreArtifacts(bool forceRestore)
        {
            // We don't want linter tests to download anything during analysis.  So we are downloading
            //   metadata here to avoid downloading during analysis, and tests can use cached data if it
            //   exists (e.g. IRegistryModuleMetadataProvider.GetCached* methods).
            // If --no-restore has been specified on the command ine, we don't want to download anything at all.
            // Therefore we do the cache download here so that lint rules can have access to the cached metadata.
            // CONSIDER: Revisit if it's okay to download metadata during analysis?  This will be more of a problem
            //   when we extend the linter rules to include private registry modules.

            await publicModuleMetadataProvider.TryAwaitCache(forceRestore);
        }

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactReference> references)
        {
            var failures = new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            var referencesEvaluated = references.ToArray();

            // CONSIDER: Run these in parallel
            foreach (var reference in referencesEvaluated)
            {
                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {GetArtifactDirectory(reference).Uri.GetFilePath()}");
                var (result, errorMessage) = await this.TryRestoreArtifactAsync(reference.ReferencingFile.Configuration, reference);

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

        public override async Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<OciArtifactReference> references)
        {
            return await base.InvalidateArtifactsCacheInternal(references);
        }

        public override async Task PublishModule(OciArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri, string? description)
        {
            var config = new Oci.OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1);

            var layers = new List<Oci.OciDescriptor>
            {
                new(compiledArmTemplate, BicepMediaTypes.BicepModuleLayerV1Json, new OciManifestAnnotationsBuilder().WithTitle("Compiled ARM template").Build())
            };

            if (bicepSources is { })
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

            if (reference.ReferencingFile.Features.OciEnabled)
            {
                var session = TryCreateSession(reference) ?? throw new ExternalArtifactException($"Registry provider for '{reference.Registry}' does not support OCI sessions.");
                var artifact = new SessionRegistryArtifact(
                    ManifestMediaType.OciImageManifest.ToString(),
                    BicepMediaTypes.BicepModuleArtifactType,
                    config,
                    layers.ToImmutableArray(),
                    annotations.Build());

                await using var disposableSession = session;
                try
                {
                    await disposableSession.PushAsync(CreateRegistryRef(reference), artifact, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new ExternalArtifactException($"Failed to publish module via OCI session: {ex.Message}", ex);
                }
            }
            else
            {
                var transport = transportFactory.GetTransport(reference);

                try
                {
                    await transport.PushArtifactAsync(
                        reference.ReferencingFile.Configuration.Cloud,
                        reference,
                        ManifestMediaType.OciImageManifest.ToString(),
                        BicepMediaTypes.BicepModuleArtifactType,
                        config,
                        layers,
                        annotations);
                }
                catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
                {
                    throw new ExternalArtifactException(exception.Message, exception);
                }
                catch (RequestFailedException exception)
                {
                    throw new ExternalArtifactException(exception.Message, exception);
                }
            }
        }

        public override async Task PublishExtension(OciArtifactReference reference, ExtensionPackage package)
        {
            OciExtensionV1Config configData = package.LocalDeployEnabled ? new(
                localDeployEnabled: true,
                supportedArchitectures: package.Binaries.Select(x => x.Architecture.Name).ToImmutableArray()) :
                new(null, null);

            var config = new Oci.OciDescriptor(
                JsonSerializer.Serialize(configData, OciExtensionV1ConfigSerializationContext.Default.OciExtensionV1Config),
                BicepMediaTypes.BicepExtensionConfigV1);

            var layers = new List<Oci.OciDescriptor>
            {
                new(package.Types, BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip, new OciManifestAnnotationsBuilder().WithTitle("types.tgz").Build())
            };

            if (configData.LocalDeployEnabled == true)
            {
                foreach (var binary in package.Binaries)
                {
                    var layerName = BicepMediaTypes.GetExtensionArtifactLayerV1Binary(binary.Architecture);
                    layers.Add(new(binary.Data, layerName, new OciManifestAnnotationsBuilder().WithTitle("extension.bin").Build()));
                }
            }

            var annotations = new OciManifestAnnotationsBuilder()
                .WithBicepSerializationFormatV1()
                .WithCreatedTime(DateTime.Now);

            if (reference.ReferencingFile.Features.OciEnabled)
            {
                var session = TryCreateSession(reference) ?? throw new ExternalArtifactException($"Registry provider for '{reference.Registry}' does not support OCI sessions.");

                var artifact = new SessionRegistryArtifact(
                    ManifestMediaType.OciImageManifest.ToString(),
                    BicepMediaTypes.BicepExtensionArtifactType,
                    config,
                    layers.ToImmutableArray(),
                    annotations.Build());

                await using var disposableSession = session;
                try
                {
                    await disposableSession.PushAsync(CreateRegistryRef(reference), artifact, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new ExternalArtifactException($"Failed to publish extension via OCI session: {ex.Message}", ex);
                }
            }
            else
            {
                var transport = transportFactory.GetTransport(reference);

                try
                {
                    await transport.PushArtifactAsync(
                        reference.ReferencingFile.Configuration.Cloud,
                        reference,
                        ManifestMediaType.OciImageManifest.ToString(),
                        BicepMediaTypes.BicepExtensionArtifactType,
                        config,
                        layers,
                        annotations);
                }
                catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
                {
                    throw new ExternalArtifactException(exception.Message, exception);
                }
                catch (RequestFailedException exception)
                {
                    throw new ExternalArtifactException(exception.Message, exception);
                }
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
            this.GetArtifactFile(reference, ArtifactFileType.Manifest).Write(result.ManifestData);

            // write data file
            var mainLayer = result.GetMainLayer();

            // TODO: IsArtifactRestoreRequired assumes there must be a ModuleMain file, which isn't true for extension artifacts
            // NOTE(stephenWeatherford): That can be solved by only writing layer data files only (see below CONSIDER)
            //   and not main.json directly (https://github.com/Azure/bicep/issues/11900)
            var moduleFileType = (reference.Type, result) switch
            {
                (ArtifactType.Module, OciModuleArtifactResult) => ArtifactFileType.ModuleMain,
                (ArtifactType.Module, OciExtensionArtifactResult) => throw new InvalidArtifactException($"Expected a module, but retrieved an extension."),
                (ArtifactType.Extension, OciExtensionArtifactResult) => ArtifactFileType.Extension,
                (ArtifactType.Extension, OciModuleArtifactResult) => throw new InvalidArtifactException($"Expected an extension, but retrieved a module."),
                _ => throw new InvalidOperationException($"Unexpected artifact type \"{result.GetType().Name}\"."),
            };

            this.GetArtifactFile(reference, moduleFileType).Write(mainLayer.Data);

            if (result is OciModuleArtifactResult)
            {
                // write source archive file
                if (result.TryGetSingleLayerByMediaType(BicepMediaTypes.BicepSourceV1Layer) is BinaryData sourceData)
                {
                    // CONSIDER: Write all layers as separate binary files instead of separate files for source.tgz and extension files.
                    // We should do this rather than writing individual files we know about,
                    //   (e.g. "source.tgz") because this way we can restore all layers even if we don't know what they're for.
                    //   If an optional layer is added, we don't need to version the cache because all versions have the same complete
                    //   info on disk and can handle the layer data as they want to.
                    // The manifest can be used to determine what's in each layer file.
                    //  (https://github.com/Azure/bicep/issues/11900)
                    this.GetArtifactFile(reference, ArtifactFileType.Source).Write(sourceData);
                }
            }

            if (result is OciExtensionArtifactResult extension)
            {
                var config = extension.Config is { } ?
                    JsonSerializer.Deserialize(extension.Config.Data, OciExtensionV1ConfigSerializationContext.Default.OciExtensionV1Config) :
                    null;

                // if the artifact supports local deployment, fetch the extension binary
                if (config?.LocalDeployEnabled == true &&
                    config?.SupportedArchitectures is { } binaryArchitectures)
                {
                    if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                    {
                        throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{reference}\".");
                    }

                    if (!binaryArchitectures.Contains(architecture.Name) ||
                        result.TryGetSingleLayerByMediaType(BicepMediaTypes.GetExtensionArtifactLayerV1Binary(architecture)) is not { } sourceData)
                    {
                        throw new InvalidOperationException($"The extension \"{reference}\" does not support architecture {architecture.Name}.");
                    }

                    var file = this.GetArtifactFile(reference, ArtifactFileType.ExtensionBinary);
                    file.Write(sourceData);
                    file.MakeExecutable();
                }
            }

            // write metadata
            var metadata = new ModuleMetadata(result.ManifestDigest);
            using var metadataStream = new MemoryStream();
            OciSerialization.Serialize(metadataStream, metadata);
            metadataStream.Position = 0;

            this.GetArtifactFile(reference, ArtifactFileType.Metadata).Write(metadataStream);
        }

        protected override IDirectoryHandle GetArtifactDirectory(OciArtifactReference reference)
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

            return reference.ReferencingFile.Features.CacheRootDirectory.GetDirectory($"{ArtifactReferenceSchemes.Oci}/{registry}/{repository}/{tagOrDigest}");
        }

        protected override IFileHandle GetArtifactLockFile(OciArtifactReference reference) => this.GetArtifactFile(reference, ArtifactFileType.Lock);

        private async Task<(OciArtifactResult?, string? errorMessage)> TryRestoreArtifactAsync(RootConfiguration configuration, OciArtifactReference reference)
        {
            if (reference.ReferencingFile.Features.OciEnabled)
            {
                var session = TryCreateSession(reference) ?? throw new ExternalArtifactException($"Registry provider for '{reference.Registry}' does not support OCI sessions.");

                await using var disposableSession = session;
                try
                {
                    var artifact = await disposableSession.PullAsync(CreateRegistryRef(reference), CancellationToken.None).ConfigureAwait(false);
                    var result = ConvertToOciArtifactResult(artifact);

                    await WriteArtifactContentToCacheAsync(reference, result).ConfigureAwait(false);

                    return (result, null);
                }
                catch (Exception exception) when (exception is ExternalArtifactException or RequestFailedException or InvalidArtifactException)
                {
                    return (null, exception.Message);
                }
                catch (Exception exception)
                {
                    throw new ExternalArtifactException($"Failed to restore artifact via OCI session: {exception.Message}", exception);
                }
            }

            var transport = transportFactory.GetTransport(reference);

            try
            {
                var result = await transport.PullArtifactAsync(configuration.Cloud, reference).ConfigureAwait(false);

                await WriteArtifactContentToCacheAsync(reference, result).ConfigureAwait(false);

                return (result, null);
            }
            catch (ExternalArtifactException exception)
            {
                return (null, exception.Message);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                return (null, exception.Message);
            }
            catch (RequestFailedException exception)
            {
                return (null, exception.Message);
            }
            catch (Exception exception)
            {
                return (null, $"Unhandled exception: {exception}");
            }
        }

        private RegistryRef CreateRegistryRef(OciArtifactReference reference) =>
            new(reference.Registry, reference.Repository, reference.Tag, reference.Digest);

        private IRegistrySession? TryCreateSession(OciArtifactReference reference, CancellationToken cancellationToken = default)
        {
            try
            {
                var context = new RegistryProviderContext
                {
                    Logger = logger,
                    CredentialChain = credentialChain,
                    Cloud = reference.ReferencingFile.Configuration.Cloud,
                    CancellationToken = cancellationToken,
                };

                return transportFactory.CreateSession(CreateRegistryRef(reference), context);
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static OciArtifactResult ConvertToOciArtifactResult(SessionRegistryArtifact artifact)
        {
            var manifest = new OciManifest(
                schemaVersion: 2,
                mediaType: artifact.MediaType,
                artifactType: artifact.ArtifactType,
                config: artifact.Config,
                layers: artifact.Layers,
                annotations: artifact.Annotations);

#pragma warning disable IL2026
            var manifestBinaryData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026
            var manifestDigest = RegistryOciDescriptor.ComputeDigest(RegistryOciDescriptor.AlgorithmIdentifierSha256, manifestBinaryData);

            var layers = artifact.Layers.Select(descriptor =>
            {
                if (descriptor.Data is null)
                {
                    throw new InvalidOperationException("Registry session returned a layer without content.");
                }

                return new OciArtifactLayer(descriptor.Digest, descriptor.MediaType, descriptor.Data);
            }).ToImmutableArray();

            OciArtifactLayer? configLayer = null;
            if (!artifact.Config.IsEmpty() && artifact.Config.Data is BinaryData configData)
            {
                configLayer = new OciArtifactLayer(artifact.Config.Digest, artifact.Config.MediaType, configData);
            }

            return artifact.ArtifactType switch
            {
                BicepMediaTypes.BicepExtensionArtifactType => new OciExtensionArtifactResult(manifestBinaryData, manifestDigest, layers, configLayer),
                BicepMediaTypes.BicepModuleArtifactType or null => new OciModuleArtifactResult(manifestBinaryData, manifestDigest, layers),
                _ => throw new InvalidArtifactException($"artifacts of type: '{artifact.ArtifactType}' are not supported by this Bicep version."),
            };
        }

        private static bool IsNotFoundException(Exception? exception) =>
            exception switch
            {
                null => false,
                NotFoundException => true,
                RequestFailedException { Status: 404 } => true,
                _ => false,
            };

        private static bool CheckAllInnerExceptionsAreRequestFailures(AggregateException exception) =>
            exception.InnerExceptions.All(inner => inner is RequestFailedException);

        private IFileHandle GetArtifactFile(OciArtifactReference reference, ArtifactFileType fileType)
        {
            var fileName = fileType switch
            {
                ArtifactFileType.ModuleMain => "main.json",
                ArtifactFileType.Lock => "lock",
                ArtifactFileType.Manifest => "manifest",
                ArtifactFileType.Metadata => "metadata",
                ArtifactFileType.Extension => "types.tgz",
                ArtifactFileType.Source => "source.tgz",
                ArtifactFileType.ExtensionBinary => "extension.bin",
                _ => throw new NotImplementedException($"Unexpected artifact file type '{fileType}'.")
            };

            return this.GetArtifactDirectory(reference).GetFile(fileName);
        }

        private enum ArtifactFileType
        {
            ModuleMain,
            Manifest,
            Lock,
            Metadata,
            Extension,
            Source,
            ExtensionBinary,
        };
    }
}
