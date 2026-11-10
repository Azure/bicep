// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Azure;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.SourceLink;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using OrasProject.Oras.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Core.Registry
{
    public sealed class OciArtifactRegistry : ArtifactRegistry<OciArtifactReference>
    {
        private readonly RegistryConfiguration registryConfiguration;

        private readonly IOciRegistryTransportFactory transportFactory;

        private readonly ILogger<OciArtifactRegistry> logger;

        public OciArtifactRegistry(
            RegistryConfiguration registryConfiguration,
            IOciRegistryTransportFactory transportFactory,
            ILogger<OciArtifactRegistry> logger)
        {
            this.registryConfiguration = registryConfiguration;
            this.transportFactory = transportFactory;
            this.logger = logger;
        }

        public override string Scheme => ArtifactReferenceSchemes.Oci;

        protected override BicepRegistryArtifact GetCacheEntry(OciArtifactReference reference) => reference.CacheEntry;

        protected override IFileHandle? TryGetEntryPoint(OciArtifactReference reference) =>
            // Security-first: an artifact from an untrusted registry is never loaded, even if it is cached. It counts as
            // not restored, so that restore runs and reports the untrusted registry (BCP446).
            this.registryConfiguration.IsRegistryTrusted(reference.Registry) ? base.TryGetEntryPoint(reference) : null;

        public async Task<bool> CheckArtifactExists(OciArtifactReference reference)
        {
            await using var session = CreateSession(reference);

            try
            {
                await session.ResolveAsync(reference, CancellationToken.None);
                return true;
            }
            catch (NotFoundException)
            {
                return false;
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
            catch (InvalidArtifactException exception)
            {
                throw new ExternalArtifactException($"The artifact referenced by {reference.FullyQualifiedReference} was not downloaded from the registry because it is invalid.", exception);
            }
            catch (RequestFailedException exception)
            {
                throw new ExternalArtifactException(exception.Message, exception);
            }
            catch (ExternalArtifactException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException(exception.Message, exception);
            }
        }

        public string? TryGetDocumentationUri(OciArtifactReference ociArtifactModuleReference)
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

        public Task<string?> TryGetModuleDescription(ModuleSymbol module, OciArtifactReference ociArtifactModuleReference)
        {
            var ociAnnotations = TryGetOciAnnotations(ociArtifactModuleReference);
            return Task.FromResult(DescriptionHelper.TryGetFromOciManifestAnnotations(ociAnnotations));
        }

        private OciManifest GetCachedManifest(OciArtifactReference ociArtifactModuleReference)
        {
            var manifestFile = this.GetCacheEntry(ociArtifactModuleReference).ManifestFile;

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
                try
                {
                    var session = CreateSession(reference);
                    try
                    {
#pragma warning disable VSTHRD002
                        var (_, manifest) = session.ResolveAsync(reference, CancellationToken.None).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
                        return manifest.Annotations ?? [];
                    }
                    finally
                    {
#pragma warning disable VSTHRD002
                        session.DisposeAsync().AsTask().GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        protected override async Task<IDictionary<ExternalArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactReference> references)
        {
            var failures = new Dictionary<ExternalArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

            var referencesEvaluated = references.ToArray();

            // CONSIDER: Run these in parallel
            foreach (var reference in referencesEvaluated)
            {
                // Block restore if the registry is not in the trusted list (BCP446).
                // Invalid patterns in config are handled as warnings at config-load time (BCP447 via IBicepConfiguration)
                // and are simply not included in the valid TrustedRegistries list, so they won't match here.
                if (!registryConfiguration.IsRegistryTrusted(reference.Registry))
                {
                    failures[reference] = x => x.ArtifactRestoreBlockedByRegistry(reference.Registry);
                    continue;
                }

                using var timer = new ExecutionTimer($"Restore module {reference.FullyQualifiedReference} to {this.GetCacheEntry(reference).CacheDirectory.Uri.GetFilePath()}");
                var (result, errorMessage) = await this.TryRestoreArtifactAsync(reference);

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

        public async Task PublishModule(OciArtifactReference reference, ModulePackage package)
        {
            EnsureArtifactType(reference, ArtifactType.Module);

            var config = new Oci.OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1);

            var layers = new List<Oci.OciDescriptor>
            {
                new(package.Template, BicepMediaTypes.BicepModuleLayerV1Json, new OciManifestAnnotationsBuilder().WithTitle("Compiled ARM template").Build())
            };

            if (package.Sources is { } bicepSources)
            {
                layers.Add(
                    new(
                        bicepSources,
                        BicepMediaTypes.BicepSourceV1Layer,
                        new OciManifestAnnotationsBuilder().WithTitle("Source files").Build()));
            }

            var annotations = new OciManifestAnnotationsBuilder()
                .WithDescription(DescriptionHelper.TryGetFromArmTemplate(package.Template))
                .WithDocumentationUri(package.DocumentationUri)
                .WithCreatedTime(DateTime.Now);

            await using var session = CreateSession(reference);

            try
            {
                await session.PushAsync(
                    reference,
                    BicepMediaTypes.OciImageManifest,
                    BicepMediaTypes.BicepModuleArtifactType,
                    config,
                    layers,
                    annotations,
                    CancellationToken.None);
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

        public async Task PublishExtension(OciArtifactReference reference, ExtensionPackage package)
        {
            EnsureArtifactType(reference, ArtifactType.Extension);

            OciExtensionV1Config configData = package.LocalDeployEnabled && package.BinaryLayerVersion == ExtensionBinaryLayerVersion.V1 ? new(
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

            if (configData.LocalDeployEnabled == true || package.BinaryLayerVersion == ExtensionBinaryLayerVersion.V2)
            {
                foreach (var binary in package.Binaries)
                {
                    var layerName = package.BinaryLayerVersion == ExtensionBinaryLayerVersion.V2
                        ? BicepMediaTypes.GetExtensionArtifactLayerV2Binary(binary.Architecture)
                        : BicepMediaTypes.GetExtensionArtifactLayerV1Binary(binary.Architecture);
                    layers.Add(new(binary.Data, layerName, new OciManifestAnnotationsBuilder().WithTitle("extension.bin").Build()));
                }
            }

            var annotations = new OciManifestAnnotationsBuilder()
                .WithBicepSerializationFormatV1()
                .WithCreatedTime(DateTime.Now);

            await using var session = CreateSession(reference);

            try
            {
                await session.PushAsync(
                    reference,
                    BicepMediaTypes.OciImageManifest,
                    BicepMediaTypes.BicepExtensionArtifactType,
                    config,
                    layers,
                    annotations,
                    CancellationToken.None);
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

        // media types are case-insensitive (they are lowercase by convention only)
        public static readonly StringComparison MediaTypeComparison = StringComparison.OrdinalIgnoreCase;

        private async Task<(OciArtifactResult?, string? errorMessage)> TryRestoreArtifactAsync(OciArtifactReference reference)
        {
            await using var session = CreateSession(reference);

            try
            {
                var result = await session.PullAsync(reference, CancellationToken.None);

                // Check the artifact type before writing anything, so that a wrong artifact leaves no partial cache entry.
                switch (reference.Type, result)
                {
                    case (ArtifactType.Module, OciModuleArtifactResult):
                    case (ArtifactType.Extension, OciExtensionArtifactResult):
                        break;
                    case (ArtifactType.Module, OciExtensionArtifactResult):
                        throw new InvalidArtifactException($"Expected a module, but retrieved an extension.");
                    case (ArtifactType.Extension, OciModuleArtifactResult):
                        throw new InvalidArtifactException($"Expected an extension, but retrieved a module.");
                    default:
                        throw new InvalidOperationException($"Unexpected artifact type \"{result.GetType().Name}\".");
                }

                await this.GetCacheEntry(reference).WriteAsync(result, reference.ToString());

                return (result, null);
            }
            catch (InvalidArtifactException exception)
            {
                return (null, exception.Message);
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

        private static void EnsureArtifactType(OciArtifactReference reference, ArtifactType expected)
        {
            if (reference.Type != expected)
            {
                throw new ArgumentException($"Cannot publish a package of type '{expected}' to a target of type '{reference.Type}'.", nameof(reference));
            }
        }

        private IRegistrySession CreateSession(OciArtifactReference reference)
        {
            // Security: never open an authenticated session to a registry that is not trusted.
            // This is the single choke point for all session-based operations (restore, publish,
            // existence checks, hover/annotations) and covers every registry,
            // preventing credentials/tokens from being sent to an attacker-controlled registry host.
            if (!registryConfiguration.IsRegistryTrusted(reference.Registry))
            {
                throw new ExternalArtifactException(
                    $"Cannot connect to registry \"{reference.Registry}\" because it is not in the list of trusted registries. " +
                    $"Set the {BicepEnvironmentVariables.TrustedRegistries} environment variable (comma-separated hostnames, e.g. \"contoso.example.com,*.contoso.io\") to allow it.");
            }

            return transportFactory.CreateSession(reference);
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
    }
}
