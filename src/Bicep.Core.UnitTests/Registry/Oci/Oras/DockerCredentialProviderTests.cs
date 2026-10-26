// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrasProject.Oras.Registry.Remote.Auth;

namespace Bicep.Core.UnitTests.Registry.Oci.Oras;

[TestClass]
public class DockerCredentialProviderTests
{
    [TestMethod]
    [DataRow("DOCKER_CONFIG", "/docker-config", "/docker-config/config.json")]
    [DataRow("DOCKER_CONFIG", "docker-config", "/docker-config/config.json")]
    [DataRow("HOME", "/home/user", "/home/user/.docker/config.json")]
    [DataRow("USERPROFILE", "/home/user", "/home/user/.docker/config.json")]
    public async Task ResolveCredentialAsync_ConfigLocationFromEnvironment_ReadsConfigFile(string variable, string directory, string configPath)
    {
        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile(configPath, """{"auths":{"example.com":{"username":"user","password":"pass"}}}""");
        var variableValue = directory.StartsWith('/') ? fileSet.GetUri(directory).GetFilePath() : directory;
        var environment = (TestEnvironment.Default with { CurrentDirectory = fileSet.GetUri("").GetFilePath() })
            .WithVariables((variable, variableValue));
        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer);

        var credential = await provider.ResolveCredentialAsync("example.com", default);

        credential.Username.Should().Be("user");
        credential.Password.Should().Be("pass");
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_InvalidConfigPath_ReturnsEmptyCredential()
    {
        var environment = TestEnvironment.Default.WithVariables(("DOCKER_CONFIG", "\\\\?\\invalid"));
        var provider = new DockerCredentialProvider(environment, new MockFileSystemTestFileSet().FileExplorer);

        var credential = await provider.ResolveCredentialAsync("example.com", default);

        credential.Username.Should().BeNull();
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldReadCredentialsFromDockerConfig()
    {
        const string registry = "localhost:5000";
        var dockerConfigRoot = "/tmp/docker";
        var configPath = $"{dockerConfigRoot}/config.json";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile(configPath,
                """
{
  "auths": {
    "localhost:5000": {
      "auth": "dXNlcjpwYXNz"
    }
  }
}
""");

        var environment = TestEnvironment.Default.WithVariables(("DOCKER_CONFIG", fileSet.GetUri(dockerConfigRoot).GetFilePath()));
        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer);

        Credential credential = await provider.ResolveCredentialAsync(registry, default);

        credential.Username.Should().Be("user");
        credential.Password.Should().Be("pass");
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldUseCredentialHelperForMatchingRegistry()
    {
        const string registry = "ghcr.io";
        var dockerConfigRoot = "/tmp/docker";
        var configPath = $"{dockerConfigRoot}/config.json";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile(configPath,
                """
{
  "auths": {},
  "credHelpers": {
    "ghcr.io": "gh"
  }
}
""");

        var environment = TestEnvironment.Default.WithVariables(("DOCKER_CONFIG", fileSet.GetUri(dockerConfigRoot).GetFilePath()));
        var helperInvoker = new TestDockerCredentialHelperInvoker((helper, targetRegistry) =>
        {
            helper.Should().Be("gh");
            targetRegistry.Should().Be("ghcr.io");
            return new Credential("helperUser", "helperPass");
        });

        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer, helperInvoker);

        var credential = await provider.ResolveCredentialAsync(registry, CancellationToken.None);

        credential.Username.Should().Be("helperUser");
        credential.Password.Should().Be("helperPass");
        helperInvoker.Invocations.Should().ContainSingle().Which.Should().Be(("gh", "ghcr.io"));
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldUseCredsStoreWhenNoSpecificHelperMatches()
    {
        const string registry = "registry.example.com";
        var dockerConfigRoot = "/tmp/docker";
        var configPath = $"{dockerConfigRoot}/config.json";

        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile(configPath,
                """
{
  "auths": {},
  "credsStore": "desktop"
}
""");

        var environment = TestEnvironment.Default.WithVariables(("DOCKER_CONFIG", fileSet.GetUri(dockerConfigRoot).GetFilePath()));
        var helperInvoker = new TestDockerCredentialHelperInvoker((helper, targetRegistry) =>
        {
            helper.Should().Be("desktop");
            targetRegistry.Should().Be(registry);
            return new Credential("storeUser", "storePass");
        });

        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer, helperInvoker);

        var credential = await provider.ResolveCredentialAsync(registry, CancellationToken.None);

        credential.Username.Should().Be("storeUser");
        credential.Password.Should().Be("storePass");
        helperInvoker.Invocations.Should().ContainSingle().Which.Should().Be(("desktop", registry));
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldUseEnvironmentVariables_WhenScopedToMatchingRegistry()
    {
        var fileSet = new MockFileSystemTestFileSet();
        var environment = TestEnvironment.Default.WithVariables(
            ("DOCKER_REGISTRY", "example.com"),
            ("DOCKER_USERNAME", "envUser"),
            ("DOCKER_PASSWORD", "envPass"));
        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer);

        Credential credential = await provider.ResolveCredentialAsync("example.com", default);

        credential.Username.Should().Be("envUser");
        credential.Password.Should().Be("envPass");
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldNotLeakEnvironmentVariables_WhenDockerRegistryUnset()
    {
        // Security: without DOCKER_REGISTRY scoping, ambient DOCKER_USERNAME/PASSWORD must NOT be sent
        // to an arbitrary (possibly attacker-controlled) host.
        var fileSet = new MockFileSystemTestFileSet();
        var environment = TestEnvironment.Default.WithVariables(("DOCKER_USERNAME", "envUser"), ("DOCKER_PASSWORD", "envPass"));
        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer);

        Credential credential = await provider.ResolveCredentialAsync("attacker.example", default);

        credential.Username.Should().BeNull();
        credential.Password.Should().BeNull();
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldNotLeakEnvironmentVariables_ToNonMatchingRegistry()
    {
        // Security: env credentials scoped to one registry must never be sent to a different host.
        var fileSet = new MockFileSystemTestFileSet();
        var environment = TestEnvironment.Default.WithVariables(
            ("DOCKER_REGISTRY", "trusted.example.com"),
            ("DOCKER_USERNAME", "envUser"),
            ("DOCKER_PASSWORD", "envPass"));
        var provider = new DockerCredentialProvider(environment, fileSet.FileExplorer);

        Credential credential = await provider.ResolveCredentialAsync("attacker.example", default);

        credential.Username.Should().BeNull();
        credential.Password.Should().BeNull();
    }

    [TestMethod]
    public async Task ResolveCredentialAsync_ShouldReturnEmptyCredential_WhenNoSourcesPresent()
    {
        var provider = new DockerCredentialProvider(TestEnvironment.Default, new MockFileSystemTestFileSet().FileExplorer);

        Credential credential = await provider.ResolveCredentialAsync("example.com", default);

        credential.Username.Should().BeNull();
        credential.Password.Should().BeNull();
        credential.RefreshToken.Should().BeNull();
        credential.AccessToken.Should().BeNull();
    }

    private sealed class TestDockerCredentialHelperInvoker : IDockerCredentialHelperInvoker
    {
        private readonly Func<string, string, Credential?> resolver;

        public TestDockerCredentialHelperInvoker(Func<string, string, Credential?> resolver)
        {
            this.resolver = resolver;
        }

        public List<(string Helper, string Registry)> Invocations { get; } = new();

        public Task<Credential?> InvokeAsync(string helperName, string registry, CancellationToken cancellationToken)
        {
            Invocations.Add((helperName, registry));
            return Task.FromResult(resolver(helperName, registry));
        }
    }
}
