// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http.Headers;
using System.Text.Json;
using Bicep.Core.Utils;

namespace Bicep.Cli.Helpers;

public record BicepGitHubReleaseVersion(
    Version Version,
    string TagName,
    Uri? ReleaseUri);

public interface IGitHubLatestReleaseClient
{
    Task<BicepGitHubReleaseVersion?> GetLatestReleaseVersionAsync(CancellationToken cancellationToken);
}

public class GitHubLatestReleaseClient(HttpClient httpClient, IEnvironment environment) : IGitHubLatestReleaseClient
{
    private const string LatestReleaseEndpoint = "https://api.github.com/repos/Azure/bicep/releases/latest";

    public async Task<BicepGitHubReleaseVersion?> GetLatestReleaseVersionAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, LatestReleaseEndpoint);
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Bicep.Cli", environment.CurrentVersion.Version));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("tag_name", out var tagNameElement) ||
            tagNameElement.GetString() is not { } tagName ||
            VersionChecker.TryParseVersion(tagName) is not { } releaseVersion)
        {
            return null;
        }

        Uri? releaseUri = null;
        if (document.RootElement.TryGetProperty("html_url", out var htmlUrlElement) &&
            htmlUrlElement.GetString() is { } htmlUrl &&
            Uri.TryCreate(htmlUrl, UriKind.Absolute, out var parsedReleaseUri))
        {
            releaseUri = parsedReleaseUri;
        }

        return new(releaseVersion, tagName, releaseUri);
    }
}
