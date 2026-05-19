// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Bicep.Cli.Helpers;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests.Helpers;

[TestClass]
public class GitHubLatestReleaseClientTests
{
    [TestMethod]
    public async Task GetLatestReleaseVersionAsync_ShouldReturnVersionFromGitHubReleasePayload()
    {
        string? userAgent = null;
        Uri? requestUri = null;
        var httpClient = new HttpClient(new TestHttpMessageHandler(request =>
        {
            userAgent = request.Headers.UserAgent.ToString();
            requestUri = request.RequestUri;

            return new(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                    {
                      "tag_name": "v0.43.8",
                      "html_url": "https://github.com/Azure/bicep/releases/tag/v0.43.8"
                    }
                    """),
            };
        }));

        var client = new GitHubLatestReleaseClient(httpClient, TestEnvironment.Default);

        var result = await client.GetLatestReleaseVersionAsync(CancellationToken.None);

        result.Should().Be(new BicepGitHubReleaseVersion(
            new Version("0.43.8"),
            "v0.43.8",
            new Uri("https://github.com/Azure/bicep/releases/tag/v0.43.8")));
        requestUri.Should().Be(new Uri("https://api.github.com/repos/Azure/bicep/releases/latest"));
        userAgent.Should().Contain("Bicep.Cli");
    }

    [TestMethod]
    public async Task GetLatestReleaseVersionAsync_ShouldReturnNullForInvalidReleaseVersion()
    {
        var httpClient = new HttpClient(new TestHttpMessageHandler(_ => new(HttpStatusCode.OK)
        {
            Content = new StringContent("""
                {
                  "tag_name": "not-a-version",
                  "html_url": "https://github.com/Azure/bicep/releases/tag/not-a-version"
                }
                """),
        }));

        var client = new GitHubLatestReleaseClient(httpClient, TestEnvironment.Default);

        var result = await client.GetLatestReleaseVersionAsync(CancellationToken.None);

        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetLatestReleaseVersionAsync_ShouldThrowForUnsuccessfulStatusCode()
    {
        var httpClient = new HttpClient(new TestHttpMessageHandler(_ => new(HttpStatusCode.Forbidden)));
        var client = new GitHubLatestReleaseClient(httpClient, TestEnvironment.Default);

        var action = async () => await client.GetLatestReleaseVersionAsync(CancellationToken.None);

        await action.Should().ThrowAsync<HttpRequestException>();
    }

    private sealed class TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handleRequest) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(handleRequest(request));
    }
}
