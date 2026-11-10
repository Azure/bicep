// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Cryptography;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Samples;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

/// <summary>
/// Pins the on-disk layout of the artifact cache. Older Bicep versions on the same machine read and write the same
/// cache, so these paths must not change.
/// </summary>
[TestClass]
public class ArtifactCacheLayoutTests
{
    private const string TemplateSpecId = "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/Test-RG/providers/Microsoft.Resources/templateSpecs/Test-TS/versions/V1.0";

    private const string TemplateSpecContent = """
        {
          "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/Test-RG/providers/Microsoft.Resources/templateSpecs/Test-TS/versions/V1.0",
          "properties": {
            "mainTemplate": {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "resources": []
            }
          }
        }
        """;

    private readonly TestExternalArtifactManager artifactManager = new();

    [TestMethod]
    public async Task Restored_artifacts_are_cached_at_the_expected_paths()
    {
        var compiler = this.CreateCompiler();
        var remoteExtension = MockExtensionFactory.CreateMockExtWithNoConfigType("foo");
        var localExtensionPackage = await ExtensionV1Archive.Build(new ExtensionPackage(MockExtensionFactory.CreateMockExtWithNoConfigType("bar").TypesTgzData, LocalDeployEnabled: false, []));

        await this.artifactManager.PublishRegistryModules(
            new("br:mockregistry.io/test/module1:v1", "output o string = 'v1'", WithSource: true),
            new("br:mockregistry.io/test/module1:Mixed_Tag", "output o string = 'mixed'"),
            new("br:MockRegistry.IO/test/casing:v1", "output o string = 'casing'"),
            new("br:localhost:5000/test/app:v1", "output o string = 'port'"),
            new("br:mockregistry.io/test/nested/deep/mod:v1", "output o string = 'nested'"));
        await this.artifactManager.PublishExtension(remoteExtension);
        this.artifactManager.UpsertTemplateSpec(TemplateSpecId, TemplateSpecContent);

        var result = await compiler.Compile(
            ("main.bicep", $$"""
                extension '{{remoteExtension.ExtensionRepoReference}}'
                extension './local.tgz'

                module tagged 'br:mockregistry.io/test/module1:v1' = {}
                module mixedTag 'br:mockregistry.io/test/module1:Mixed_Tag' = {}
                module mixedCaseRegistry 'br:MockRegistry.IO/test/casing:v1' = {}
                module port 'br:localhost:5000/test/app:v1' = {}
                module nested 'br:mockregistry.io/test/nested/deep/mod:v1' = {}
                module spec 'ts:00000000-0000-0000-0000-000000000000/Test-RG/Test-TS:V1.0' = {}
                """),
            ("local.tgz", localExtensionPackage));

        result.Diagnostics.Where(x => x.Level == DiagnosticLevel.Error).Should().BeEmpty();

        // A local extension package is unpacked under the SHA-256 of the package file.
        var localDigest = $"sha256_{Convert.ToHexString(SHA256.HashData(localExtensionPackage.ToArray())).ToLowerInvariant()}";
        GetCachedFiles(compiler).Should().BeEquivalentTo(
            "br/mockregistry.io/test$module1/v1$/main.json",
            "br/mockregistry.io/test$module1/v1$/manifest",
            "br/mockregistry.io/test$module1/v1$/metadata",
            "br/mockregistry.io/test$module1/v1$/source.tgz",
            // Upper-case letters of a tag are encoded as a bit mask after "$": M (0) and T (6) => 0x41.
            "br/mockregistry.io/test$module1/Mixed_Tag$41/main.json",
            "br/mockregistry.io/test$module1/Mixed_Tag$41/manifest",
            "br/mockregistry.io/test$module1/Mixed_Tag$41/metadata",
            "br/mockregistry.io/test$casing/v1$/main.json",
            "br/mockregistry.io/test$casing/v1$/manifest",
            "br/mockregistry.io/test$casing/v1$/metadata",
            "br/localhost$5000/test$app/v1$/main.json",
            "br/localhost$5000/test$app/v1$/manifest",
            "br/localhost$5000/test$app/v1$/metadata",
            "br/mockregistry.io/test$nested$deep$mod/v1$/main.json",
            "br/mockregistry.io/test$nested$deep$mod/v1$/manifest",
            "br/mockregistry.io/test$nested$deep$mod/v1$/metadata",
            "br/mcr.microsoft.com/bicep$extensions$foo$v1/1.2.3$/types.tgz",
            "br/mcr.microsoft.com/bicep$extensions$foo$v1/1.2.3$/manifest",
            "br/mcr.microsoft.com/bicep$extensions$foo$v1/1.2.3$/metadata",
            "ts/00000000-0000-0000-0000-000000000000/test-rg/test-ts/v1.0/main.json",
            $"local/{localDigest}/types.tgz");
    }

    [TestMethod]
    public async Task Artifacts_restored_by_digest_are_cached_under_the_digest()
    {
        var compiler = this.CreateCompiler();
        await this.artifactManager.PublishRegistryModule("br:mockregistry.io/test/module1:v1", "output o string = 'v1'");

        // The manifest digest is the SHA-256 of the manifest content, which restoring the tag writes to the cache.
        await compiler.Compile("module tagged 'br:mockregistry.io/test/module1:v1' = {}");
        var manifest = GetCacheRoot(compiler).GetFile("br/mockregistry.io/test$module1/v1$/manifest").TryReadBinaryData().Unwrap().ToArray();
        var digestHex = Convert.ToHexString(SHA256.HashData(manifest)).ToLowerInvariant();

        var result = await compiler.Compile($"module pinned 'br:mockregistry.io/test/module1@sha256:{digestHex}' = {{}}");

        result.Diagnostics.Where(x => x.Level == DiagnosticLevel.Error).Should().BeEmpty();
        GetCachedFiles(compiler).Where(path => path.Contains('#')).Should().BeEquivalentTo(
            $"br/mockregistry.io/test$module1/sha256#{digestHex}/main.json",
            $"br/mockregistry.io/test$module1/sha256#{digestHex}/manifest",
            $"br/mockregistry.io/test$module1/sha256#{digestHex}/metadata");
    }

    private TestCompiler CreateCompiler() => TestCompiler
        .ForMockFileSystemCompilation()
        .ConfigureServices(services => services.AddExternalArtifactManager(this.artifactManager));

    private static IDirectoryHandle GetCacheRoot(TestCompiler compiler) =>
        compiler.GetService<IFeatureProviderFactory>().GetFeatureProvider(compiler.FileSet.GetUri("main.bicep")).CacheRootDirectory;

    // The "lock" files are left out: whether they remain after the lock is released depends on the file system.
    private static IEnumerable<string> GetCachedFiles(TestCompiler compiler)
    {
        var root = GetCacheRoot(compiler);
        var rootPath = root.Uri.Path.TrimEnd('/') + "/";

        return EnumerateFilesRecursively(root)
            .Select(file => file.Uri.Path[rootPath.Length..])
            .Where(path => !path.EndsWith("/lock", StringComparison.Ordinal));
    }

    private static IEnumerable<IFileHandle> EnumerateFilesRecursively(IDirectoryHandle directory) =>
        directory.EnumerateFiles().Concat(directory.EnumerateDirectories().SelectMany(EnumerateFilesRecursively));
}
