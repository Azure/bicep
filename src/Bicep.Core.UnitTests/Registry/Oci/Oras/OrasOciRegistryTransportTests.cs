// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Oci.Oras;

[TestClass]
public class OrasOciRegistryTransportTests
{
    private static readonly IBicepCloudConfiguration Cloud = BicepTestConstants.BuiltInConfiguration.Cloud;
    private const string Registry = "example.azurecr.io";
    private const string Repository = "modules/test";
    private const string ManifestUrl = $"https://{Registry}/v2/{Repository}/manifests/v1";

    [TestMethod]
    [DataRow("example.azurecr.io")]
    [DataRow("mcr.microsoft.com")]
    [DataRow("mcr.azure.cn")]
    public async Task PublicRegistriesDoNotRequireAzureCredentials(string registry)
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        http.When($"https://{registry}/v2/{Repository}/manifests/v1").Respond(_ => ManifestResponse(CreateManifest(layer)));
        http.When($"https://{registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(_ => BlobResponse(layer.Data!));
        var transport = CreateTransport(http);

        var result = await transport.PullArtifactAsync(Cloud, new OciArtifactAddressComponents(registry, Repository, "v1", null));

        result.Should().BeOfType<OciModuleArtifactResult>();
        result.GetMainLayer().Data.ToString().Should().Be("{}");
    }

    [TestMethod]
    public async Task PrivateAcrUsesConfiguredAzureCredentialChainAndTokenExchange()
    {
        using var http = new MockHttpMessageHandler();
        var manifest = CreateManifest();
        http.When(HttpMethod.Get, ManifestUrl).Respond(request =>
            request.Headers.Authorization?.Parameter == "registry-access-token" ? ManifestResponse(manifest) : Challenge());
        http.When(HttpMethod.Get, $"https://{Registry}/oauth2/token*").Respond(HttpStatusCode.Unauthorized);
        var exchange = http.When(HttpMethod.Post, $"https://{Registry}/oauth2/exchange")
            .WithFormData("access_token", "entra-token")
            .Respond("application/json", """{"refresh_token":"registry-refresh-token"}""");
        http.When(HttpMethod.Post, $"https://{Registry}/oauth2/token")
            .WithFormData("refresh_token", "registry-refresh-token")
            .Respond("application/json", """{"access_token":"registry-access-token","expires_in":3600}""");
        var credential = StrictMock.Of<TokenCredential>();
        credential.Setup(x => x.GetTokenAsync(
            It.Is<TokenRequestContext>(context => context.Scopes.SequenceEqual(new[] { $"{Cloud.ResourceManagerAudience}/.default" })),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessToken("entra-token", DateTimeOffset.UtcNow.AddHours(1)));
        var factory = StrictMock.Of<ITokenCredentialFactory>();
        factory.Setup(x => x.CreateChain(Cloud.CredentialPrecedence, Cloud.CredentialOptions, Cloud.ActiveDirectoryAuthorityUri))
            .Returns(credential.Object);
        var transport = CreateTransport(http, factory.Object);

        await using var session = transport.CreateSession(Cloud, Registry, Repository);
        var reference = OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null);
        var result = await session.ResolveAsync(reference, default);

        result.Digest.Should().Be(Digest(manifest));
        http.GetMatchCount(exchange).Should().Be(1);
        factory.VerifyAll();
    }

    [TestMethod]
    public async Task ListsRepositoriesAndTagsWithPaginationAndLimits()
    {
        using var http = new MockHttpMessageHandler();
        http.Expect($"https://{Registry}/v2/_catalog*").Respond(_ =>
        {
            var response = JsonResponse("""{"repositories":["modules/a","modules/b"]}""");
            response.Headers.Add("Link", "</v2/_catalog?last=modules%2Fb&n=2>; rel=\"next\"");
            return response;
        });
        http.Expect($"https://{Registry}/v2/_catalog?last=modules%2Fb&n=2")
            .Respond("application/json", """{"repositories":["modules/c","modules/d"]}""");
        http.Expect($"https://{Registry}/v2/{Repository}/tags/list*").Respond(_ =>
        {
            var response = JsonResponse("""{"name":"modules/test","tags":["v2","v1"]}""");
            response.Headers.Add("Link", $"</v2/{Repository}/tags/list?last=v1&n=2>; rel=\"next\"");
            return response;
        });
        http.Expect($"https://{Registry}/v2/{Repository}/tags/list?last=v1&n=2")
            .Respond("application/json", """{"name":"modules/test","tags":["v3","v10"]}""");
        var transport = CreateTransport(http);

        var repositories = await transport.GetRepositoryNamesAsync(Cloud, Registry, 3);
        var tags = await transport.GetRepositoryTagsAsync(Cloud, Registry, Repository);

        repositories.Should().Equal("modules/a", "modules/b", "modules/c");
        tags.Should().Equal("v1", "v10", "v2", "v3");
        (await transport.GetRepositoryNamesAsync(Cloud, Registry, 0)).Should().BeEmpty();
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task RepositoryLimitDoesNotFetchUnneededPages()
    {
        using var http = new MockHttpMessageHandler();
        http.Expect($"https://{Registry}/v2/_catalog*").Respond(_ =>
        {
            var response = JsonResponse("""{"repositories":["modules/a","modules/b"]}""");
            response.Headers.Add("Link", "</v2/_catalog?last=modules%2Fb>; rel=\"next\"");
            return response;
        });

        var repositories = await CreateTransport(http).GetRepositoryNamesAsync(Cloud, Registry, 1);

        repositories.Should().Equal("modules/a");
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CatalogDetailsUseLastOrdinalTagRatherThanHighestSemanticVersion()
    {
        using var http = new MockHttpMessageHandler();
        http.When($"https://{Registry}/v2/_catalog*").Respond("application/json", """{"repositories":["modules/test"]}""");
        http.When($"https://{Registry}/v2/{Repository}/tags/list*")
            .Respond("application/json", """{"name":"modules/test","tags":["v2","v10","v1"]}""");
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        foreach (var tag in new[] { "v1", "v2", "v10" })
        {
            var manifest = BinaryData.FromObjectAsJson(new OciManifest(
                2, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepModuleArtifactType,
                new OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1), [layer],
                new OciManifestAnnotationsBuilder().WithDescription($"Description for {tag}").Build()),
                OciManifestSerializationContext.Default.Options);
            http.When($"https://{Registry}/v2/{Repository}/manifests/{tag}").Respond(_ => ManifestResponse(manifest));
        }
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(_ => BlobResponse(layer.Data!));
        var factory = StrictMock.Of<IOciRegistryTransportFactory>();
        factory.Setup(x => x.GetTransport(Registry, true)).Returns(CreateTransport(http));
        var provider = new PrivateAcrModuleMetadataProvider(Cloud, Registry, factory.Object, useOrasSdk: true);

        var module = await provider.TryGetModuleAsync(Repository);
        module.Should().NotBeNull();
        var versions = await module!.TryGetVersionsAsync();
        var details = await module.TryGetDetailsAsync();

        versions.Select(version => version.Version).Should().Equal("v1", "v10", "v2");
        details.Description.Should().Be("Description for v2");
    }

    [TestMethod]
    public async Task ResolveDoesNotDownloadLayers()
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        http.Expect(HttpMethod.Get, ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(layer)));
        var transport = CreateTransport(http);
        await using var session = transport.CreateSession(Cloud, Registry, Repository);

        var result = await session.ResolveAsync(OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null), default);

        result.Manifest.Layers.Should().ContainSingle();
        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    [DataRow("different content")]
    [DataRow("{")]
    [DataRow("{} extra")]
    [DataRow("[]")]
    public async Task PullValidatesBlobsAgainstManifestDescriptors(string corruptContent)
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        http.When(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(layer)));
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}")
            .Respond(_ => BlobResponse(BinaryData.FromString(corruptContent)));
        var transport = CreateTransport(http);

        var action = () => transport.PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        await action.Should().ThrowAsync<InvalidArtifactException>();
    }

    [TestMethod]
    [DataRow(-1L, false)]
    [DataRow((long)int.MaxValue, false)]
    [DataRow(long.MaxValue, false)]
    [DataRow(-1L, true)]
    [DataRow((long)int.MaxValue, true)]
    [DataRow(long.MaxValue, true)]
    public async Task PullRejectsMismatchedDescriptorSizesWithoutPreallocating(long size, bool isConfig)
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", isConfig ? BicepMediaTypes.BicepModuleConfigV1 : BicepMediaTypes.BicepModuleLayerV1Json);
        var descriptor = new OciDescriptor(layer.MediaType, layer.Digest, size, null);
        var manifest = BinaryData.FromObjectAsJson(new OciManifest(
            2, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepModuleArtifactType,
            isConfig ? descriptor : new OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1),
            isConfig ? [] : [descriptor], []), OciManifestSerializationContext.Default.Options);
        http.When(ManifestUrl).Respond(_ => ManifestResponse(manifest));
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(_ => BlobResponse(layer.Data!));

        var action = () => CreateTransport(http).PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        await action.Should().ThrowAsync<InvalidArtifactException>().WithMessage($"*size of {size} bytes but received 2 bytes*");
    }

    [TestMethod]
    public async Task PullDoesNotPreallocateDeclaredLayerSize()
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        var descriptor = new OciDescriptor(layer.MediaType, layer.Digest, Array.MaxLength, null);
        http.When(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(descriptor)));
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(_ => BlobResponse(layer.Data!));

        var action = () => CreateTransport(http).PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        await action.Should().ThrowAsync<InvalidArtifactException>().WithMessage($"*size of {Array.MaxLength} bytes but received 2 bytes*");
    }

    [TestMethod]
    public async Task PullValidatesContentAcrossBufferBoundaries()
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor(new string('a', 200000), BicepMediaTypes.BicepModuleLayerV1Json);
        http.When(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(layer)));
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(_ => BlobResponse(layer.Data!));

        var result = await CreateTransport(http).PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        result.GetMainLayer().Data.ToMemory().Span.SequenceEqual(layer.Data!.ToMemory().Span).Should().BeTrue();
    }

    [TestMethod]
    public async Task PullReportsMissingBlobsAsInvalidArtifact()
    {
        using var http = new MockHttpMessageHandler();
        var layer = new OciDescriptor("{}", BicepMediaTypes.BicepModuleLayerV1Json);
        http.When(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(layer)));
        http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(HttpStatusCode.NotFound);
        var transport = CreateTransport(http);

        var action = () => transport.PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        await action.Should().ThrowAsync<InvalidArtifactException>().WithMessage("*non-existent blob*");
    }

    [TestMethod]
    public async Task ResolveRejectsCorruptManifestContent()
    {
        using var http = new MockHttpMessageHandler();
        http.When(ManifestUrl).Respond(_ =>
        {
            var response = ManifestResponse(CreateManifest());
            response.Headers.Remove("Docker-Content-Digest");
            response.Headers.Add("Docker-Content-Digest", $"sha256:{new string('0', 64)}");
            return response;
        });
        var transport = CreateTransport(http);
        await using var session = transport.CreateSession(Cloud, Registry, Repository);

        var action = () => session.ResolveAsync(OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null), default);

        await action.Should().ThrowAsync<InvalidArtifactException>();
    }

    [TestMethod]
    public async Task NotFoundIsTranslatedForExistenceChecks()
    {
        using var http = new MockHttpMessageHandler();
        http.When(ManifestUrl).Respond(HttpStatusCode.NotFound);
        var transport = CreateTransport(http);
        await using var session = transport.CreateSession(Cloud, Registry, Repository);

        var action = () => session.ResolveAsync(OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null), default);
        var exception = (await action.Should().ThrowAsync<ExternalArtifactException>()).Which;

        exception.InnerException.Should().BeOfType<OrasProject.Oras.Exceptions.NotFoundException>();
    }

    [TestMethod]
    public async Task UntrustedCatalogDoesNotMakeRequests()
    {
        using var http = new MockHttpMessageHandler();
        var transport = CreateTransport(http);

        var action = () => transport.GetRepositoryNamesAsync(Cloud, "untrusted.example", 10);

        await action.Should().ThrowAsync<ExternalArtifactException>().WithMessage("*not in the list of trusted registries*");
    }

    [TestMethod]
    public async Task TransientFailuresAreRetried()
    {
        using var http = new MockHttpMessageHandler();
        http.Expect(ManifestUrl).Respond(HttpStatusCode.ServiceUnavailable);
        http.Expect(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest()));
        var transport = CreateTransport(http);
        await using var session = transport.CreateSession(Cloud, Registry, Repository);

        await session.ResolveAsync(OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null), default);

        http.VerifyNoOutstandingExpectation();
    }

    [TestMethod]
    public async Task CancellationStopsRetries()
    {
        using var http = new MockHttpMessageHandler();
        using var cancellation = new CancellationTokenSource();
        http.When(ManifestUrl).Respond(async _ =>
        {
            await cancellation.CancelAsync();
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        });
        var transport = CreateTransport(http);
        await using var session = transport.CreateSession(Cloud, Registry, Repository);

        var action = () => session.ResolveAsync(OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null), cancellation.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task PullBoundsConcurrentLayerDownloadsAtFive()
    {
        using var http = new MockHttpMessageHandler();
        var layers = Enumerable.Range(0, 11).Select(i =>
            new OciDescriptor($"layer-{i}", i == 0 ? BicepMediaTypes.BicepModuleLayerV1Json : "application/octet-stream")).ToArray();
        http.When(ManifestUrl).Respond(_ => ManifestResponse(CreateManifest(layers)));
        var active = 0;
        var maximum = 0;
        var count = 0;
        var sync = new object();
        foreach (var layer in layers)
        {
            http.When($"https://{Registry}/v2/{Repository}/blobs/{layer.Digest}").Respond(async _ =>
            {
                lock (sync)
                {
                    active++;
                    maximum = Math.Max(active, maximum);
                    count++;
                }

                await Task.Delay(30);
                lock (sync)
                {
                    active--;
                }

                return BlobResponse(layer.Data!);
            });
        }

        var result = await CreateTransport(http).PullArtifactAsync(Cloud, new OciArtifactAddressComponents(Registry, Repository, "v1", null));

        maximum.Should().Be(5);
        count.Should().Be(11);
        result.Layers.Select(layer => layer.Digest).Should().Equal(layers.Select(layer => layer.Digest));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task NonAzureDockerAuthenticationRespectsFeatureGate(bool enabled)
    {
        using var http = new MockHttpMessageHandler();
        http.When($"https://ghcr.io/v2/{Repository}/manifests/v1").Respond(request =>
        {
            if (request.Headers.Authorization?.Parameter == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("user:password")))
            {
                return ManifestResponse(CreateManifest());
            }

            var challenge = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            challenge.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=\"registry\""));
            return challenge;
        });
        var docker = new DockerCredentialProvider(TestEnvironment.Default.WithVariables(
            ("DOCKER_REGISTRY", "ghcr.io"), ("DOCKER_USERNAME", "user"), ("DOCKER_PASSWORD", "password")), new InMemoryFileExplorer());
        await using var session = CreateTransport(http, docker: docker).CreateSession(Cloud, "ghcr.io", Repository, useDockerCredentials: enabled);
        var reference = OciRegistryHelper.CreateModuleReference("ghcr.io", Repository, "v1", null);
        var action = () => session.ResolveAsync(reference, default);

        if (enabled)
        {
            (await action()).Digest.Should().Be(Digest(CreateManifest()));
        }
        else
        {
            await action.Should().ThrowAsync<ExternalArtifactException>().WithMessage("*Missing username or password*");
        }
    }

    [TestMethod]
    [DataRow(BicepMediaTypes.BicepModuleArtifactType, BicepMediaTypes.BicepModuleConfigV1, BicepMediaTypes.BicepModuleLayerV1Json)]
    [DataRow(BicepMediaTypes.BicepExtensionArtifactType, BicepMediaTypes.BicepExtensionConfigV1, BicepMediaTypes.BicepExtensionArtifactLayerV1TarGzip)]
    public async Task PushPreservesManifestAndBlobContents(string artifactType, string configType, string layerType)
    {
        using var http = new MockHttpMessageHandler();
        http.When(HttpMethod.Post, $"https://{Registry}/v2/{Repository}/blobs/uploads/").Respond(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri($"/v2/{Repository}/blobs/uploads/upload", UriKind.Relative);
            return response;
        });
        var uploaded = new Dictionary<string, BinaryData>();
        http.When(HttpMethod.Put, $"https://{Registry}/v2/{Repository}/blobs/uploads/upload*").Respond(async request =>
        {
            var data = await BinaryData.FromStreamAsync(await request.Content!.ReadAsStreamAsync());
            uploaded.Add(Digest(data), data);
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Add("Docker-Content-Digest", Digest(data));
            return response;
        });
        OciManifest? uploadedManifest = null;
        http.When(HttpMethod.Put, ManifestUrl).Respond(async request =>
        {
            request.Content!.Headers.ContentType!.MediaType.Should().Be(BicepMediaTypes.OciImageManifest);
            var data = await BinaryData.FromStreamAsync(await request.Content.ReadAsStreamAsync());
            uploadedManifest = OciManifest.FromBinaryData(data);
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Add("Docker-Content-Digest", Digest(data));
            return response;
        });
        var config = new OciDescriptor("{}", configType);
        var layer = new OciDescriptor("payload", layerType);
        var transport = CreateTransport(http);
        var reference = OciRegistryHelper.CreateModuleReference(Registry, Repository, "v1", null);

        await transport.PushArtifactAsync(Cloud, reference, BicepMediaTypes.OciImageManifest, artifactType, config, [layer],
            new OciManifestAnnotationsBuilder().WithDescription("description"));

        uploaded.Keys.Should().BeEquivalentTo(config.Digest, layer.Digest);
        uploaded[layer.Digest].ToString().Should().Be("payload");
        uploadedManifest.Should().NotBeNull();
        uploadedManifest!.ArtifactType.Should().Be(artifactType);
        uploadedManifest.Config.MediaType.Should().Be(configType);
        uploadedManifest.Layers.Should().ContainSingle().Which.MediaType.Should().Be(layerType);
        uploadedManifest.Annotations[OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation].Should().Be("description");
    }

    private static OrasOciRegistryTransport CreateTransport(MockHttpMessageHandler handler, ITokenCredentialFactory? credentials = null, DockerCredentialProvider? docker = null)
    {
        var clients = StrictMock.Of<IHttpClientFactory>();
        clients.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(handler, disposeHandler: false));
        var defaultCredentials = StrictMock.Of<ITokenCredentialFactory>();
        defaultCredentials.Setup(x => x.CreateChain(Cloud.CredentialPrecedence, Cloud.CredentialOptions, Cloud.ActiveDirectoryAuthorityUri))
            .Returns(StrictMock.Of<TokenCredential>().Object);
        return new OrasOciRegistryTransport(
            new RegistryConfiguration(PermitUntrustedRegistries: false),
            docker ?? new DockerCredentialProvider(TestEnvironment.Default, new InMemoryFileExplorer()),
            credentials ?? defaultCredentials.Object,
            clients.Object);
    }

    private static BinaryData CreateManifest(params OciDescriptor[] layers) =>
        BinaryData.FromObjectAsJson(new OciManifest(
            2, BicepMediaTypes.OciImageManifest, BicepMediaTypes.BicepModuleArtifactType,
            new OciDescriptor("{}", BicepMediaTypes.BicepModuleConfigV1), [.. layers], []),
            OciManifestSerializationContext.Default.Options);

    private static string Digest(BinaryData data) => OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, data);

    private static HttpResponseMessage BlobResponse(BinaryData data) => new(HttpStatusCode.OK)
    {
        Content = new ByteArrayContent(data.ToArray()),
    };

    private static HttpResponseMessage ManifestResponse(BinaryData data)
    {
        var response = BlobResponse(data);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(BicepMediaTypes.OciImageManifest);
        response.Headers.Add("Docker-Content-Digest", Digest(data));
        return response;
    }

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
    };

    private static HttpResponseMessage Challenge()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer",
            $"realm=\"https://{Registry}/oauth2/token\",service=\"{Registry}\",scope=\"repository:{Repository}:pull\""));
        return response;
    }
}
