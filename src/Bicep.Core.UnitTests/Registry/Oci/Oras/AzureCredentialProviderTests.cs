// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using Azure.Core;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Oci.Oras;

[TestClass]
public class AzureCredentialProviderTests
{
    [TestMethod]
    [DataRow("https://management.azure.com")]
    [DataRow("https://management.chinacloudapi.cn")]
    [DataRow("https://management.usgovcloudapi.net/")]
    public async Task ExchangesConfiguredAudienceAndCachesCredential(string audience)
    {
        using var http = new MockHttpMessageHandler();
        http.Expect(HttpMethod.Post, "https://example.azurecr.io/oauth2/exchange")
            .WithFormData("grant_type", "access_token")
            .WithFormData("service", "example.azurecr.io")
            .WithFormData("access_token", "entra-token")
            .Respond("application/json", """{"refresh_token":"acr-refresh-token"}""");
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(
            It.Is<TokenRequestContext>(context => context.Scopes.SequenceEqual(new[] { $"{audience.TrimEnd('/')}/.default" })),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, audience, client);

        var results = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => provider.ResolveCredentialAsync("example.azurecr.io", default)));

        results.Should().OnlyContain(result => result.RefreshToken == "acr-refresh-token");
        credential.Verify(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()), Times.Once);
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow(-1, 2)]
    [DataRow(1, 1)]
    public async Task CachesCredentialUntilEntraTokenExpires(int expiresInMinutes, int expectedExchanges)
    {
        using var http = new MockHttpMessageHandler();
        var exchange = http.When(HttpMethod.Post, "https://example.azurecr.io/oauth2/exchange")
            .Respond("application/json", """{"refresh_token":"refresh"}""");
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, "https://management.azure.com", client);

        await provider.ResolveCredentialAsync("example.azurecr.io", default);
        await provider.ResolveCredentialAsync("example.azurecr.io", default);

        http.GetMatchCount(exchange).Should().Be(expectedExchanges);
    }

    [TestMethod]
    public async Task RejectsCredentialsForAnotherHostBeforeAcquiringToken()
    {
        using var client = new HttpClient(new MockHttpMessageHandler());
        var provider = new AzureCredentialProvider("example.azurecr.io", StrictMock.Of<TokenCredential>().Object, "https://management.azure.com", client);
        var action = () => provider.ResolveCredentialAsync("attacker.example", default);
        await action.Should().ThrowAsync<InvalidOperationException>();
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("""{"refresh_token":null}""")]
    [DataRow("""{"refresh_token":""}""")]
    [DataRow("""{"refresh_token":" "}""")]
    [DataRow("""{"refresh_token":42}""")]
    [DataRow("[]")]
    [DataRow("null")]
    [DataRow("not json")]
    public async Task RejectsInvalidExchangeResponses(string body)
    {
        using var http = new MockHttpMessageHandler();
        http.When("*").Respond("application/json", body);
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, "https://management.azure.com", client);

        var action = () => provider.ResolveCredentialAsync("example.azurecr.io", default);
        await action.Should().ThrowAsync<ExternalArtifactException>().WithMessage("*invalid Azure token exchange response*");
    }

    [TestMethod]
    [DataRow(HttpStatusCode.Unauthorized)]
    [DataRow(HttpStatusCode.Forbidden)]
    [DataRow(HttpStatusCode.TooManyRequests)]
    [DataRow(HttpStatusCode.InternalServerError)]
    public async Task FailedExchangeDoesNotExposeResponseBodyOrToken(HttpStatusCode statusCode)
    {
        using var http = new MockHttpMessageHandler();
        var exchange = http.When("*").Respond(statusCode, "text/plain", "secret-response-body");
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("secret-entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, "https://management.azure.com", client);

        var action = () => provider.ResolveCredentialAsync("example.azurecr.io", default);
        var exception = (await action.Should().ThrowAsync<HttpRequestException>()).Which;
        exception.StatusCode.Should().Be(statusCode);
        exception.ToString().Should().NotContain("secret-response-body").And.NotContain("secret-entra-token");
        http.GetMatchCount(exchange).Should().Be(1);
    }

    [TestMethod]
    public async Task FailedExchangeDoesNotCacheCredential()
    {
        using var http = new MockHttpMessageHandler();
        http.Expect(HttpMethod.Post, "https://example.azurecr.io/oauth2/exchange")
            .Respond(HttpStatusCode.InternalServerError);
        http.Expect(HttpMethod.Post, "https://example.azurecr.io/oauth2/exchange")
            .Respond("application/json", """{"refresh_token":"acr-refresh-token"}""");
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, "https://management.azure.com", client);

        var action = () => provider.ResolveCredentialAsync("example.azurecr.io", default);
        await action.Should().ThrowAsync<HttpRequestException>();
        var result = await provider.ResolveCredentialAsync("example.azurecr.io", default);

        result.RefreshToken.Should().Be("acr-refresh-token");
        credential.Verify(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task ExchangeDoesNotFollowRedirects()
    {
        using var http = new MockHttpMessageHandler();
        http.Expect(HttpMethod.Post, "https://example.azurecr.io/oauth2/exchange")
            .Respond(_ => new HttpResponseMessage(HttpStatusCode.TemporaryRedirect)
            {
                Headers = { Location = new Uri("https://attacker.example/oauth2/exchange") },
            });
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        using var client = http.ToHttpClient();
        var provider = new AzureCredentialProvider("example.azurecr.io", credential.Object, "https://management.azure.com", client);

        var action = () => provider.ResolveCredentialAsync("example.azurecr.io", default);

        var exception = (await action.Should().ThrowAsync<HttpRequestException>()).Which;
        exception.StatusCode.Should().Be(HttpStatusCode.TemporaryRedirect);
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CancellationDoesNotAcquireCredentials()
    {
        using var client = new HttpClient(new MockHttpMessageHandler());
        var provider = new AzureCredentialProvider("example.azurecr.io", StrictMock.Of<TokenCredential>().Object, "https://management.azure.com", client);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var action = () => provider.ResolveCredentialAsync("example.azurecr.io", cancellation.Token);
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}
