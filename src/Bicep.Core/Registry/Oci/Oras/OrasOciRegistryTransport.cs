// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Azure.Identity;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Microsoft.Extensions.Caching.Memory;
using OrasProject.Oras.Registry.Remote;
using OrasProject.Oras.Registry.Remote.Auth;
using OrasProject.Oras.Exceptions;
using OrasReference = OrasProject.Oras.Registry.Reference;
using OrasRegistryClient = OrasProject.Oras.Registry.Remote.Registry;
using OrasRepositoryClient = OrasProject.Oras.Registry.Remote.Repository;

namespace Bicep.Core.Registry.Oci.Oras;

public class OrasOciRegistryTransport(
    RegistryConfiguration registryConfiguration,
    DockerCredentialProvider dockerCredentialProvider,
    ITokenCredentialFactory tokenCredentialFactory,
    IHttpClientFactory httpClientFactory) : IOciRegistryTransport
{
    internal const string RegistryHttpClient = "OciRegistry";
    internal const string AuthenticationHttpClient = "OciRegistryAuthentication";

    internal OrasRegistrySession CreateSession(IBicepCloudConfiguration cloud, string registry, string repository, bool useDockerCredentials = true) =>
        new(this, cloud, registry, repository, useDockerCredentials);

    public Task<string[]> GetRepositoryNamesAsync(IBicepCloudConfiguration cloud, string registry, int maxResults, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxResults);
        return ExecuteAsync(cloud, registry, async client =>
        {
            Trace.WriteLine($"Listing repositories for registry {registry} using ORAS SDK.");
            var registryClient = new OrasRegistryClient(new RepositoryOptions
            {
                Client = client,
                Reference = new OrasReference(registry),
                PlainHttp = UsePlainHttp(registry),
            });
            var results = new List<string>();
            if (maxResults > 0)
            {
                await foreach (var name in registryClient.ListRepositoriesAsync(cancellationToken: cancellationToken))
                {
                    results.Add(name);
                    if (results.Count >= maxResults)
                    {
                        break;
                    }
                }
            }

            Trace.WriteLine($"Successfully listed {results.Count} repositories for registry {registry} using ORAS SDK.");
            return results.ToArray();
        }, cancellationToken, anonymousFirst: false, allowAnonymousFallback: true);
    }

    public Task<IReadOnlyList<string>> GetRepositoryTagsAsync(IBicepCloudConfiguration cloud, string registry, string repository, CancellationToken cancellationToken = default) =>
        ExecuteAsync<IReadOnlyList<string>>(cloud, registry, async client =>
        {
            Trace.WriteLine($"Listing tags for repository {registry}/{repository} using ORAS SDK.");
            var tags = new List<string>();
            await foreach (var tag in CreateRepository(client, registry, repository).ListTagsAsync(cancellationToken: cancellationToken))
            {
                tags.Add(tag);
            }

            // A stable ordering is needed by catalog metadata selection.
            tags.Sort(StringComparer.Ordinal);
            Trace.WriteLine($"Successfully listed {tags.Count} tags for repository {registry}/{repository} using ORAS SDK.");
            return tags;
        }, cancellationToken, anonymousFirst: false, allowAnonymousFallback: true);

    public async Task<OciArtifactResult> PullArtifactAsync(IBicepCloudConfiguration cloud, IOciArtifactAddressComponents artifactReference, CancellationToken cancellationToken = default)
    {
        Trace.WriteLine($"Pulling artifact {artifactReference.ArtifactId} using ORAS SDK.");
        var result = await CreateSession(cloud, artifactReference.Registry, artifactReference.Repository).PullArtifactAsync(artifactReference, cancellationToken);
        Trace.WriteLine($"Pulled artifact {artifactReference.ArtifactId} using ORAS SDK.");
 
        return result;
    }

    public async Task PushArtifactAsync(
        IBicepCloudConfiguration cloud,
        IOciArtifactReference artifactReference,
        string? mediaType,
        string? artifactType,
        OciDescriptor config,
        IEnumerable<OciDescriptor> layers,
        OciManifestAnnotationsBuilder annotations,
        CancellationToken cancellationToken = default)
    {
        Trace.WriteLine($"Publishing artifact {artifactReference.ArtifactId} using ORAS SDK.");
        await CreateSession(cloud, artifactReference.Registry, artifactReference.Repository)
            .PushAsync(artifactReference, mediaType, artifactType, config, layers, annotations, cancellationToken);
        Trace.WriteLine($"Published artifact {artifactReference.ArtifactId} using ORAS SDK.");
    }

    internal async Task<T> ExecuteAsync<T>(
        IBicepCloudConfiguration cloud,
        string registry,
        Func<Client, Task<T>> operation,
        CancellationToken cancellationToken,
        bool anonymousFirst = true,
        bool useDockerCredentials = true,
        bool allowAnonymousFallback = false)
    {
        if (!registryConfiguration.IsRegistryTrusted(registry))
        {
            throw new ExternalArtifactException($"Cannot connect to registry \"{registry}\" because it is not in the list of trusted registries.");
        }

        var isAzure = OciRegistryTransportFactory.IsAzureRegistry(registry);
        var isMicrosoftPublicRegistry = registry.Equals("mcr.microsoft.com", StringComparison.OrdinalIgnoreCase) ||
            registry.Equals("mcr.azure.cn", StringComparison.OrdinalIgnoreCase);
        using var httpClient = httpClientFactory.CreateClient(RegistryHttpClient);
        using var authenticationClient = httpClientFactory.CreateClient(AuthenticationHttpClient);
        // ORAS otherwise shares tokens process-wide, including between different cloud credential configurations.
        using var anonymousCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 });
        using var authenticatedCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 });
        ICredentialProvider? credentials = null;

        Client CreateClient(bool authenticated)
        {
            if (authenticated && credentials is null)
            {
                if (isAzure)
                {
                    var credential = tokenCredentialFactory.CreateChain(cloud.CredentialPrecedence, cloud.CredentialOptions, cloud.ActiveDirectoryAuthorityUri);
                    credentials = new AzureCredentialProvider(registry, credential, cloud.ResourceManagerAudience, authenticationClient);
                }
                else if (useDockerCredentials && !isMicrosoftPublicRegistry)
                {
                    credentials = dockerCredentialProvider;
                }
            }

            var client = new Client(httpClient, authenticationClient, authenticated ? credentials : null,
                new Cache(authenticated ? authenticatedCache : anonymousCache))
            {
                RealmValidator = new DefaultRealmValidator
                {
                    AllowInsecureHttp = UsePlainHttp(registry),
                    TrustedRealmHosts = IsDockerHub(registry) ? new HashSet<string> { "auth.docker.io" } : new HashSet<string>(),
                },
            };
            client.SetUserAgent($"bicep/{ThisAssembly.AssemblyFileVersion}");
            return client;
        }

        try
        {
            // Public Azure registries must remain usable without acquiring Azure credentials.
            if (anonymousFirst && isAzure)
            {
                try
                {
                    return await RetryAsync(() => operation(CreateClient(false)), cancellationToken);
                }
                catch (HttpRequestException exception) when (exception.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                {
                    Trace.WriteLine($"Anonymous access to registry {registry} failed. Retrying with Azure credentials.");
                }
            }

            try
            {
                return await RetryAsync(() => operation(CreateClient(true)), cancellationToken);
            }
            catch (Exception exception) when (allowAnonymousFallback &&
                (exception is CredentialUnavailableException ||
                    exception is HttpRequestException { StatusCode: HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden }))
            {
                Trace.WriteLine($"Authenticated catalog access to registry {registry} failed. Retrying anonymously.");
                return await RetryAsync(() => operation(CreateClient(false)), cancellationToken);
            }
        }
        catch (NotFoundException exception)
        {
            throw new ExternalArtifactException("The artifact does not exist in the registry.", exception);
        }
        catch (Exception exception) when (exception is HttpRequestException or IOException or AuthenticationFailedException or AuthenticationException)
        {
            throw new ExternalArtifactException(exception.Message, exception);
        }
    }

    internal static OrasRepositoryClient CreateRepository(Client client, string registry, string repository) =>
        new(new RepositoryOptions
        {
            Client = client,
            Reference = OrasReference.Parse($"{registry}/{repository}"),
            PlainHttp = UsePlainHttp(registry),
        });

    private static async Task<T> RetryAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken)
    {
        for (var attempt = 0; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await operation();
            }
            catch (HttpRequestException exception) when (attempt < 3 &&
                exception.StatusCode is null or HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests or
                    HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)
            {
                Trace.WriteLine($"Registry request failed. Retrying: {exception.Message}");
                await Task.Delay(TimeSpan.FromMilliseconds(800 * (1 << attempt)), cancellationToken);
            }
        }
    }

    private static bool IsDockerHub(string registry) =>
        registry.Equals("docker.io", StringComparison.OrdinalIgnoreCase) ||
        registry.Equals("registry-1.docker.io", StringComparison.OrdinalIgnoreCase) ||
        registry.Equals("index.docker.io", StringComparison.OrdinalIgnoreCase);

    private static bool UsePlainHttp(string registry) =>
        Uri.TryCreate($"http://{registry}", UriKind.Absolute, out var uri) && uri.IsLoopback;
}
