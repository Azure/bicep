// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Azure;
using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.RegistryClients.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using OrasProject.Oras.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Core.Artifacts.Registries
{
    public sealed class OciArtifactRegistry : RemoteArtifactRegistry<OciArtifactReference>
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

        public override string Scheme => ArtifactSchemes.Oci;

        protected override OciArtifactCacheEntry GetCacheEntry(OciArtifactReference reference) => reference.Type switch
        {
            ArtifactType.Module => new OciModuleArtifactCacheEntry(reference.AddressComponents, reference.Context.CacheRootDirectory),
            ArtifactType.Extension => new OciExtensionArtifactCacheEntry(reference.AddressComponents, reference.Context.CacheRootDirectory),
            _ => throw new InvalidOperationException($"Unexpected artifact type '{reference.Type}'."),
        };

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
            catch (ArtifactException exception) when (IsNotFoundException(exception.InnerException))
            {
                return false;
            }
            catch (InvalidArtifactException exception) when (
                exception.Kind == InvalidArtifactExceptionKind.WrongArtifactType ||
                exception.Kind == InvalidArtifactExceptionKind.UnknownLayerMediaType)
            {
                throw new ArtifactException($"The artifact referenced by {reference.FullyQualifiedReference} was not downloaded from the registry because it is invalid.", exception);
            }
            catch (InvalidArtifactException exception)
            {
                throw new ArtifactException($"The artifact referenced by {reference.FullyQualifiedReference} was not downloaded from the registry because it is invalid.", exception);
            }
            catch (RequestFailedException exception)
            {
                throw new ArtifactException(exception.Message, exception);
            }
            catch (ArtifactException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ArtifactException(exception.Message, exception);
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
                throw new ArtifactException($"Could not retrieve artifact manifest from \"{manifestFile.Uri}\"", ex);
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

        protected override async Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<OciArtifactReference> references)
        {
            var failures = new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>();

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
                var (restored, errorMessage) = await this.TryRestoreArtifactAsync(reference);

                if (!restored)
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

        public async Task PublishModule(OciArtifactReference reference, ModuleArtifactPackage package)
        {
            EnsureArtifactType(reference, ArtifactType.Module);

            await this.PushAsync(reference, OciModuleArtifactCodec.Encode(package));
        }

        public async Task PublishExtension(OciArtifactReference reference, ExtensionArtifactPackage package)
        {
            EnsureArtifactType(reference, ArtifactType.Extension);

            await this.PushAsync(reference, OciExtensionArtifactCodec.Encode(package));
        }

        private async Task PushAsync(OciArtifactReference reference, OciArtifactPayload payload)
        {
            await using var session = CreateSession(reference);

            try
            {
                await session.PushAsync(reference, payload, CancellationToken.None);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                throw new ArtifactException(exception.Message, exception);
            }
            catch (RequestFailedException exception)
            {
                throw new ArtifactException(exception.Message, exception);
            }
        }

        private async Task<(bool Restored, string? errorMessage)> TryRestoreArtifactAsync(OciArtifactReference reference)
        {
            await using var session = CreateSession(reference);

            try
            {
                var payload = await session.PullAsync(reference, CancellationToken.None);

                // Check the artifact type before writing anything, so that a wrong artifact leaves no partial cache entry.
                switch (GetDeclaredArtifactType(payload.Manifest), this.GetCacheEntry(reference))
                {
                    case (ArtifactType.Module, OciModuleArtifactCacheEntry entry):
                        await entry.WriteAsync(OciModuleArtifactCodec.Decode(payload), payload);
                        break;
                    case (ArtifactType.Extension, OciExtensionArtifactCacheEntry entry):
                        await entry.WriteAsync(OciExtensionArtifactCodec.Decode(payload), payload, reference);
                        break;
                    case (ArtifactType.Extension, _):
                        throw new InvalidArtifactException("Expected a module, but the artifact is an extension.");
                    default:
                        throw new InvalidArtifactException("Expected an extension, but the artifact is a module.");
                }

                return (true, null);
            }
            catch (InvalidArtifactException exception)
            {
                return (false, exception.Message);
            }
            catch (ArtifactException exception)
            {
                return (false, exception.Message);
            }
            catch (AggregateException exception) when (CheckAllInnerExceptionsAreRequestFailures(exception))
            {
                return (false, exception.Message);
            }
            catch (RequestFailedException exception)
            {
                return (false, exception.Message);
            }
            catch (Exception exception)
            {
                return (false, $"Unhandled exception: {exception}");
            }
        }

        // A manifest without an artifact type is a module published before extensions existed.
        private static ArtifactType GetDeclaredArtifactType(OciManifest manifest) => manifest.ArtifactType switch
        {
            BicepMediaTypes.BicepExtensionArtifactType => ArtifactType.Extension,
            BicepMediaTypes.BicepModuleArtifactType or null => ArtifactType.Module,
            _ => throw new InvalidArtifactException($"The artifact type \"{manifest.ArtifactType}\" is not supported by this version of Bicep. {OciModuleArtifactCodec.NewerVersionMightBeRequired}"),
        };

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
                throw new ArtifactException(
                    $"Cannot connect to registry \"{reference.Registry}\" because it is not in the list of trusted registries. " +
                    $"Set the {BicepEnvironmentVariables.TrustedRegistries} environment variable (comma-separated hostnames, e.g. \"contoso.example.com,*.contoso.io\") to allow it.");
            }

            return transportFactory.CreateSession(reference.Context.Cloud, reference.Registry, reference.Repository, reference.UseOrasSdk);
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
