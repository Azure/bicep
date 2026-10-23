// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using System.Text.Json;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Core;
using Azure.Core.Pipeline;
using Bicep.Core.Tracing;
using OrasProject.Oras.Registry.Remote.Auth;

namespace Bicep.Core.Registry.Oci.Oras;

internal sealed class AzureCredentialProvider(
    string registry,
    TokenCredential credential,
    string audience,
    HttpClient httpClient) : ICredentialProvider
{
    private readonly ContainerRegistryClient exchangeClient = CreateExchangeClient(registry, httpClient);
    private readonly SemaphoreSlim refreshLock = new(1);
    private Credential cachedCredential;
    private DateTimeOffset refreshAfter;

    public async Task<Credential> ResolveCredentialAsync(string hostname, CancellationToken cancellationToken)
    {
        // Never exchange an Azure token at a host supplied by an authentication challenge.
        if (!string.Equals(hostname, registry, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot use Azure credentials for registry \"{hostname}\".");
        }

        await refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (DateTimeOffset.UtcNow < refreshAfter)
            {
                return cachedCredential;
            }

            var token = await credential.GetTokenAsync(new TokenRequestContext([$"{audience.TrimEnd('/')}/.default"]), cancellationToken);
            Response<AcrRefreshToken> response;
            try
            {
                response = await exchangeClient.ExchangeAadAccessTokenForAcrRefreshTokenAsync(
                    registry, accessToken: token.Token, cancellationToken: cancellationToken);
            }
            catch (RequestFailedException exception)
            {
                // SDK exceptions include the response body. Preserve status-based retries without exposing credentials.
                throw new HttpRequestException($"Azure token exchange for registry \"{registry}\" failed with HTTP {exception.Status}.",
                    inner: null, exception.Status == 0 ? null : (HttpStatusCode)exception.Status);
            }
            catch (Exception exception) when (exception is JsonException or InvalidOperationException)
            {
                throw new ExternalArtifactException($"Registry \"{registry}\" returned an invalid Azure token exchange response.", exception);
            }

            var refreshToken = response.Value?.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ExternalArtifactException($"Registry \"{registry}\" returned an invalid Azure token exchange response.");
            }

            cachedCredential = new Credential(RefreshToken: refreshToken);
            // Re-exchange before the Entra token expires rather than depending on ACR's opaque token format.
            refreshAfter = token.ExpiresOn - TimeSpan.FromMinutes(5);
            return cachedCredential;
        }
        finally
        {
            refreshLock.Release();
        }
    }

    private static ContainerRegistryClient CreateExchangeClient(string registry, HttpClient httpClient)
    {
        var options = new ContainerRegistryClientOptions
        {
            Transport = new HttpClientTransport(httpClient),
        };
        options.Diagnostics.ApplySharedContainerRegistrySettings();
        // The ORAS transport owns retries and supplies an HTTP client that does not follow authentication redirects.
        options.Retry.MaxRetries = 0;
        return new ContainerRegistryClient(new Uri($"https://{registry}"), options);
    }
}
