// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Utils;
using OrasProject.Oras.Content;
using OrasProject.Oras.Oci;
using OrasProject.Oras.Registry.Remote;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasReference = OrasProject.Oras.Registry.Reference;
using OrasRegistryClient = OrasProject.Oras.Registry.Remote.Registry;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;

namespace Bicep.Core.Registry.Oci.Oras;

public class OrasOciRegistryTransport : IOciRegistryTransport
{
    private const string UserAgentPrefix = "bicep";

    private readonly ICredentialProvider credentialProvider;

    public OrasOciRegistryTransport(ICredentialChain credentialChain)
    {
        ArgumentNullException.ThrowIfNull(credentialChain);
        this.credentialProvider = new CredentialChainCredentialProvider(credentialChain);
    }

    public async Task<string[]> GetRepositoryNamesAsync(CloudConfiguration cloud, string registry, int maxResults, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var registryClient = new OrasRegistryClient(new RepositoryOptions
        {
            Client = client,
            Reference = new OrasReference(registry),
            PlainHttp = UsePlainHttp(registry)
        });

        var results = new List<string>();
        try
        {
            await foreach (var name in registryClient.ListRepositoriesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                results.Add(name);
                if (results.Count >= maxResults)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            throw new ExternalArtifactException(exception.Message, exception);
        }

        return [.. results];
    }

    public async Task<IReadOnlyList<string>> GetRepositoryTagsAsync(CloudConfiguration cloud, string registry, string repository, CancellationToken cancellationToken = default)
    {
        var repositoryClient = CreateRepository(registry, repository);
        var tags = new List<string>();

        try
        {
            await foreach (var tag in repositoryClient.ListTagsAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                tags.Add(tag);
            }
        }
        catch (Exception exception)
        {
            throw new ExternalArtifactException(exception.Message, exception);
        }

        return tags;
    }

    public async Task<OciArtifactResult> PullArtifactAsync(CloudConfiguration cloud, IOciArtifactAddressComponents artifactReference, CancellationToken cancellationToken = default)
    {
        var repository = CreateRepository(artifactReference.Registry, artifactReference.Repository);
        var reference = artifactReference.Tag ?? artifactReference.Digest ?? throw new InvalidOperationException("Artifact reference must contain a tag or digest.");

        try
        {
            var (descriptor, manifestStream) = await repository.FetchAsync(reference, cancellationToken).ConfigureAwait(false);
            await using var manifestContent = manifestStream;
            var manifestBytes = await manifestContent.ReadAllAsync(descriptor, cancellationToken).ConfigureAwait(false);
            var manifestBinaryData = BinaryData.FromBytes(manifestBytes);

            var manifest = OciManifest.FromBinaryData(manifestBinaryData)
                ?? throw new InvalidArtifactException("Unable to deserialize OCI manifest");

            var layerTasks = manifest.Layers.Select(layer => FetchLayerAsync(repository, layer, cancellationToken));
            var layerResults = await Task.WhenAll(layerTasks).ConfigureAwait(false);

            OciArtifactLayer? configLayer = null;
            if (!manifest.Config.IsEmpty())
            {
                var config = await FetchLayerAsync(repository, manifest.Config, cancellationToken).ConfigureAwait(false);
                configLayer = config;
            }

            return manifest.ArtifactType switch
            {
                BicepMediaTypes.BicepExtensionArtifactType => new OciExtensionArtifactResult(manifestBinaryData, descriptor.Digest, layerResults, configLayer),
                BicepMediaTypes.BicepModuleArtifactType or null => new OciModuleArtifactResult(manifestBinaryData, descriptor.Digest, layerResults),
                _ => throw new InvalidArtifactException($"artifacts of type: '{manifest.ArtifactType}' are not supported by this Bicep version. {OciModuleArtifactResult.NewerVersionMightBeRequired}")
            };
        }
        catch (InvalidArtifactException)
        {
            throw;
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

    public async Task PushArtifactAsync(
        CloudConfiguration cloud,
        IOciArtifactReference artifactReference,
        string? mediaType,
        string? artifactType,
        OciDescriptor config,
        IEnumerable<OciDescriptor> layers,
        OciManifestAnnotationsBuilder annotations,
        CancellationToken cancellationToken = default)
    {
        var repository = CreateRepository(artifactReference.Registry, artifactReference.Repository);

        try
        {
            await PushDescriptorAsync(repository, config, cancellationToken).ConfigureAwait(false);

            var layerDescriptors = layers.ToImmutableArray();
            foreach (var layer in layerDescriptors)
            {
                await PushDescriptorAsync(repository, layer, cancellationToken).ConfigureAwait(false);
            }

            var manifest = new OciManifest(
                schemaVersion: 2,
                mediaType: mediaType,
                artifactType: artifactType,
                config: config,
                layers: layerDescriptors,
                annotations: annotations.Build());

#pragma warning disable IL2026
            var manifestBinaryData = BinaryData.FromObjectAsJson(manifest, OciManifestSerializationContext.Default.Options);
#pragma warning restore IL2026

            var manifestBytes = manifestBinaryData.ToArray();
            var manifestDigest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, manifestBinaryData);
            var manifestDescriptor = new Descriptor
            {
                Digest = manifestDigest,
                MediaType = ManifestMediaType.OciImageManifest.ToString(),
                Size = manifestBytes.Length,
                Annotations = manifest.Annotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            await using var manifestStreamContent = manifestBinaryData.ToStream();
            await repository.PushAsync(manifestDescriptor, manifestStreamContent, artifactReference.Tag ?? artifactReference.Digest!, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            throw new ExternalArtifactException(exception.Message, exception);
        }
    }

    private OrasRepositoryClient CreateRepository(string registry, string repository)
    {
        var client = CreateClient();
        var reference = OrasReference.Parse($"{registry}/{repository}");
        var options = new RepositoryOptions
        {
            Client = client,
            Reference = reference,
            PlainHttp = UsePlainHttp(registry)
        };

        return new OrasRepositoryClient(options);
    }

    private Client CreateClient()
    {
        var client = new Client(httpClient: null, credentialProvider: credentialProvider);
        client.SetUserAgent($"{UserAgentPrefix}/{ThisAssembly.AssemblyFileVersion}");
        return client;
    }

    private static bool UsePlainHttp(string registry)
    {
        if (Uri.TryCreate($"https://{registry}", UriKind.Absolute, out var uri) &&
            IPAddress.TryParse(uri.Host, out var address))
        {
            return IPAddress.IsLoopback(address);
        }

        return registry.Contains("localhost", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<OciArtifactLayer> FetchLayerAsync(OrasRepositoryClient repository, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        var (fetchedDescriptor, stream) = await repository.Blobs.FetchAsync(descriptor.Digest, cancellationToken).ConfigureAwait(false);
        await using var content = stream;
        var bytes = await content.ReadAllAsync(fetchedDescriptor, cancellationToken).ConfigureAwait(false);
        return new OciArtifactLayer(descriptor.Digest, descriptor.MediaType, BinaryData.FromBytes(bytes));
    }

    private static async Task PushDescriptorAsync(OrasRepositoryClient repository, OciDescriptor descriptor, CancellationToken cancellationToken)
    {
        if (descriptor.Data is null)
        {
            return;
        }

        var orasDescriptor = new Descriptor
        {
            Digest = descriptor.Digest,
            MediaType = descriptor.MediaType,
            Size = descriptor.Size,
            Annotations = descriptor.Annotations.Any()
                ? descriptor.Annotations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : null
        };

        await using var stream = descriptor.Data.ToStream();
        await repository.Blobs.PushAsync(orasDescriptor, stream, cancellationToken).ConfigureAwait(false);
    }
}
