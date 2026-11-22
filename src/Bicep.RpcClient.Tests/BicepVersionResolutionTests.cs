// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class BicepVersionResolutionTests
{
    public TestContext TestContext { get; set; } = null!;

    private string Root => FileHelper.GetTestOutputRootPath(TestContext);

    private CancellationToken Token => TestContext.CancellationTokenSource.Token;

    private BicepClientConfiguration CreateConfiguration(string constraint, string? explicitVersion = null)
    {
        Directory.CreateDirectory(Root);
        File.WriteAllText(Path.Combine(Root, "bicepconfig.json"),
            JsonSerializer.Serialize(new { bicep = new { version = constraint } }));
        return new()
        {
            SourceFilePath = Path.Combine(Root, "nested/main.bicepparam"),
            InstallBasePath = Path.Combine(Root, "bin"),
            BicepVersion = explicitVersion,
            OsPlatform = OSPlatform.Linux,
            Architecture = Architecture.X64,
        };
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(Root))
        {
            Directory.Delete(Root, true);
        }
    }

    [TestMethod]
    [DataRow("0.40.1", "0.40.1")]
    [DataRow("=0.40.1", "0.40.1")]
    [DataRow(">=0.40.1, <=0.40.1", "0.40.1")]
    [DataRow("0.40", "0.40.0")]
    public async Task Exact_pins_skip_release_queries_and_use_versioned_cache(string constraint, string expectedVersion)
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        var configuration = CreateConfiguration(constraint);
        var bytes = Guid.NewGuid().ToByteArray();
        handler.Expect(HttpMethod.Get, $"https://downloads.bicep.azure.com/v{expectedVersion}/bicep-linux-x64")
            .Respond(_ => new(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) });
        var factory = new BicepClientFactory(new(handler));

        var path = await factory.Download(configuration, Token);
        var secondPath = await factory.Download(configuration, Token);

        path.Should().Be(Path.Combine(Root, "bin", $"v{expectedVersion}", "bicep"));
        secondPath.Should().Be(path);
        (await File.ReadAllBytesAsync(path, Token)).Should().Equal(bytes);
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Factory_honors_inherited_pins_and_nearest_override(bool overrideVersion)
    {
        var configuration = CreateConfiguration("0.40.1");
        File.Move(Path.Combine(Root, "bicepconfig.json"), Path.Combine(Root, "base.json"));
        File.WriteAllText(Path.Combine(Root, "bicepconfig.json"), overrideVersion
            ? """{"extends":"base.json","bicep":{"version":"0.41.0"}}"""
            : """{"extends":"base.json"}""");
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        var factory = new BicepClientFactory(new(handler));

        (await factory.ResolveVersionTag(configuration, Token)).Should().Be(overrideVersion ? "v0.41.0" : "v0.40.1");
    }

    [TestMethod]
    public async Task Config_is_not_inspected_without_source_path()
    {
        var configuration = CreateConfiguration("invalid") with { SourceFilePath = null };
        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases/latest")
            .Respond("application/json", """{"tag_name":"v0.40.1"}""");
        var factory = new BicepClientFactory(new(handler));

        (await factory.ResolveVersionTag(configuration, Token)).Should().Be("v0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task Range_selects_highest_matching_semantic_version_not_first_or_lexical_maximum()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """
                [
                    {"tag_name":"v0.40.9"},
                    {"tag_name":"v1.0.0"},
                    {"tag_name":"v0.40.12"},
                    {"tag_name":"v0.39.0"},
                    {"tag_name":"v0.40.13-preview"},
                    {"tag_name":"v0.40.10"}
                ]
                """);
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/v0.40.12/bicep-linux-x64")
            .Respond("application/octet-stream", "binary");
        var factory = new BicepClientFactory(new(handler));

        var path = await factory.Download(CreateConfiguration(">=0.40, <1"), Token);

        path.Should().Be(Path.Combine(Root, "bin", "v0.40.12", "bicep"));
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow(">=0.40, <1", "0.40.5")]
    [DataRow("0.40.1", "0.40.1")]
    public async Task Explicit_version_satisfying_constraint_does_not_query_releases(string constraint, string version)
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        var factory = new BicepClientFactory(new(handler));

        var tag = await factory.ResolveVersionTag(CreateConfiguration(constraint, version), Token);

        tag.Should().Be($"v{version}");
    }

    [TestMethod]
    public async Task Explicit_version_conflicting_with_constraint_fails_before_http()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        var factory = new BicepClientFactory(new(handler));
        var configuration = CreateConfiguration(">=0.40, <1", "1.0.0");

        var act = () => factory.Initialize(configuration, Token);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*'1.0.0' does not satisfy constraint '>=0.40.0, <1.0.0'*bicepconfig.json*");
    }

    [TestMethod]
    [DataRow("[]")]
    [DataRow("""[{"tag_name":"v0.39.0"},{"tag_name":"v1.0.0"}]""")]
    public async Task No_matching_release_fails_without_falling_back_to_latest(string releases)
    {
        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", releases);
        var factory = new BicepClientFactory(new(handler));
        var configuration = CreateConfiguration(">=0.40, <1");

        var act = () => factory.ResolveVersionTag(configuration, Token);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No published Bicep CLI release satisfies*>=0.40.0, <1.0.0*bicepconfig.json*");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Missing_constraint_preserves_latest_version_behavior(bool hasConfig)
    {
        var configuration = new BicepClientConfiguration { SourceFilePath = Path.Combine(Root, "main.bicep") };
        if (hasConfig)
        {
            Directory.CreateDirectory(Root);
            File.WriteAllText(Path.Combine(Root, "bicepconfig.json"), "{}");
        }

        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases/latest")
            .Respond("application/json", """{"tag_name":"v0.40.1"}""");
        var factory = new BicepClientFactory(new(handler));

        (await factory.ResolveVersionTag(configuration, Token)).Should().Be("v0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task ListReleases_returns_versions_without_tag_prefix()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """
                [{"tag_name":"v0.40.1"},{"tag_name":"v0.39.0"},{"tag_name":"other"},{"tag_name":null},{},{"tag_name":42}]
                """);
        var factory = new BicepClientFactory(new(handler));

        (await factory.ListReleases(Token)).Should().Equal("0.40.1", "0.39.0");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow("""[{"tag_name":"v0.40.1"}]""")]
    [DataRow("[]")]
    public async Task ListReleases_caches_successful_results_per_factory(string releases)
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", releases);
        var factory = new BicepClientFactory(new(handler));

        var first = await factory.ListReleases(Token);
        (await factory.ListReleases(Token)).Should().Equal(first);
        handler.VerifyNoOutstandingExpectation();

        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """[{"tag_name":"v0.41.0"}]""");
        var otherFactory = new BicepClientFactory(new(handler));

        (await otherFactory.ListReleases(Token)).Should().Equal("0.41.0");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Test gating; not an issue in test code.")]
    public async Task ListReleases_concurrent_calls_share_one_fetch_and_waiters_can_cancel()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Fallback.Throw(new InvalidOperationException("Unexpected HTTP request."));
        var response = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond(_ => response.Task);
        var factory = new BicepClientFactory(new(handler));

        var first = factory.ListReleases(Token);
        var second = factory.ListReleases(Token);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
        var canceledWaiter = factory.ListReleases(cts.Token);
        await cts.CancelAsync();
        var act = () => canceledWaiter;
        await act.Should().ThrowAsync<OperationCanceledException>();

        response.SetResult(new(HttpStatusCode.OK)
        {
            Content = new StringContent("""[{"tag_name":"v0.40.1"}]"""),
        });

        var results = await Task.WhenAll(first, second).WaitAsync(Token);
        foreach (var result in results)
        {
            result.Should().Equal("0.40.1");
        }

        var canceledCacheRead = () => factory.ListReleases(cts.Token);
        await canceledCacheRead.Should().ThrowAsync<OperationCanceledException>();
        (await factory.ListReleases(Token)).Should().Equal("0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task ListReleases_canceled_fetch_is_retried()
    {
        using var handler = new MockHttpMessageHandler();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond(_ =>
            {
                cts.Cancel();
                return Task.FromCanceled<HttpResponseMessage>(cts.Token);
            });
        var factory = new BicepClientFactory(new(handler));
        var act = () => factory.ListReleases(cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();

        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """[{"tag_name":"v0.40.1"}]""");

        (await factory.ListReleases(Token)).Should().Equal("0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task Release_endpoint_http_failure_is_propagated()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond(HttpStatusCode.ServiceUnavailable);
        var factory = new BicepClientFactory(new(handler));
        var configuration = CreateConfiguration(">=0.40");

        var act = () => factory.ResolveVersionTag(configuration, Token);

        await act.Should().ThrowAsync<HttpRequestException>();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """[{"tag_name":"v0.40.1"}]""");
        (await factory.ListReleases(Token)).Should().Equal("0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task Release_endpoint_invalid_json_is_propagated()
    {
        using var handler = new MockHttpMessageHandler();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", "{");
        var factory = new BicepClientFactory(new(handler));

        var act = () => factory.ListReleases(Token);

        await act.Should().ThrowAsync<JsonException>();
        handler.Expect(HttpMethod.Get, "https://downloads.bicep.azure.com/releases")
            .Respond("application/json", """[{"tag_name":"v0.40.1"}]""");
        (await factory.ListReleases(Token)).Should().Equal("0.40.1");
        handler.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task Version_resolution_honors_cancellation()
    {
        var factory = new BicepClientFactory();
        var act = () => factory.ResolveVersionTag(new() { BicepVersion = "0.40.1" }, new(canceled: true));

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public void SourceFilePath_cannot_be_combined_with_existing_cli()
    {
        var act = () => BicepClientConfiguration.Validate(new()
        {
            ExistingCliPath = "bicep",
            SourceFilePath = "main.bicep",
        });

        act.Should().Throw<ArgumentException>()
            .WithMessage("*ExistingCliPath*cannot be used in conjunction with*SourceFilePath*");
    }
}
