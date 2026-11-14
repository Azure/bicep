// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts;

[TestClass]
public class LocalArtifactLoaderTests
{
    private static readonly BinaryData Types = BinaryData.FromString("types");

    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void A_local_module_is_loaded_from_its_file()
    {
        var reference = new LocalArtifactReference(ArtifactType.Module, this.GetFile("main.bicep"), "./main.bicep");

        Load(reference, this.GetCacheRoot()).Uri.Should().Be(reference.File.Uri);
    }

    [TestMethod]
    public async Task A_local_extension_is_extracted_into_the_cache_on_first_use()
    {
        var cacheRoot = this.GetCacheRoot();
        var reference = await this.CreateExtensionReference("ext.tgz", new(Types, []));

        var entryPoint = Load(reference, cacheRoot);

        entryPoint.Uri.Path.Should().MatchRegex("/local/sha256_[0-9a-f]{64}/types\\.tgz$");
        entryPoint.TryReadBinaryData().Unwrap().ToString().Should().Be("types");
        new ExtensionArtifactCacheLayout(entryPoint.GetParent()).Open().Binary.Should().BeNull();

        // The cache is content-addressed, so another file with the same content reuses the extracted package.
        var copy = new LocalArtifactReference(ArtifactType.Extension, this.GetFile("copy.tgz"), "./copy.tgz");
        copy.File.Write(reference.File.TryReadBinaryData().Unwrap());
        Load(copy, cacheRoot).Uri.Should().Be(entryPoint.Uri);
    }

    [TestMethod]
    public async Task A_local_extension_is_extracted_with_the_binary_for_this_machine()
    {
        var current = GetCurrentArchitecture();
        var other = SupportedArchitectures.All.First(x => x.Name != current.Name);
        var cacheRoot = this.GetCacheRoot();
        var reference = await this.CreateExtensionReference("ext.tgz", new(Types, [new(other, BinaryData.FromString("other")), new(current, BinaryData.FromString("this"))]));

        var entryPoint = Load(reference, cacheRoot);

        var artifact = new ExtensionArtifactCacheLayout(entryPoint.GetParent()).Open();
        artifact.Types.Uri.Should().Be(entryPoint.Uri);
        artifact.Binary!.TryReadBinaryData().Unwrap().ToString().Should().Be("this");
    }

    [TestMethod]
    public async Task A_local_extension_without_a_binary_for_this_machine_is_not_extracted()
    {
        var current = GetCurrentArchitecture();
        var other = SupportedArchitectures.All.First(x => x.Name != current.Name);
        var cacheRoot = this.GetCacheRoot();
        var reference = await this.CreateExtensionReference("ext.tgz", new(Types, [new(other, BinaryData.FromString("other"))]));

        var diagnostic = LoadError(reference, cacheRoot);

        diagnostic.Code.Should().Be("BCP192");
        diagnostic.Message.Should().Be($"Unable to restore the artifact with reference \"./ext.tgz\": The extension \"./ext.tgz\" does not support architecture {current.Name}.");

        // Nothing is written, so the next load reports the same error rather than loading the extension without a binary.
        var cacheEntry = GetCacheEntry(reference, cacheRoot);
        cacheEntry.TypesTgzFile.Exists().Should().BeFalse();
        cacheEntry.BinaryFile.Exists().Should().BeFalse();
    }

    [TestMethod]
    public async Task Concurrent_loads_of_a_local_extension_all_see_the_complete_extraction()
    {
        var cacheRoot = this.GetCacheRoot();
        var binary = BinaryData.FromBytes(new byte[1024 * 1024]);
        ExtensionBinary[] binaries = SupportedArchitectures.TryGetCurrent() is { } current ? [new(current, binary)] : [];
        var reference = await this.CreateExtensionReference("ext.tgz", new(Types, [.. binaries]));

        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() => LocalArtifactLoader.TryGetEntryPoint(reference, cacheRoot))));

        var entryPoints = results.Select(result => result.Unwrap()).ToArray();
        entryPoints.Select(x => x.Uri).Distinct().Should().ContainSingle();
        entryPoints[0].TryReadBinaryData().Unwrap().ToString().Should().Be("types");
        if (binaries.Length > 0)
        {
            GetCacheEntry(reference, cacheRoot).BinaryFile.TryReadBinaryData().Unwrap().ToArray().Should().Equal(binary.ToArray());
        }
    }

    [TestMethod]
    public void An_invalid_local_extension_reports_an_error()
    {
        var reference = new LocalArtifactReference(ArtifactType.Extension, this.GetFile("ext.tgz"), "./ext.tgz");
        reference.File.Write("not an archive");

        var diagnostic = LoadError(reference, this.GetCacheRoot());

        diagnostic.Code.Should().Be("BCP192");
        diagnostic.Message.Should().StartWith("Unable to restore the artifact with reference \"./ext.tgz\": The file is not a valid extension package.");
    }

    [TestMethod]
    public void A_missing_local_extension_reports_an_error()
    {
        var reference = new LocalArtifactReference(ArtifactType.Extension, this.GetFile("missing.tgz"), "./missing.tgz");

        LocalArtifactLoader.TryGetEntryPoint(reference, this.GetCacheRoot()).IsSuccess().Should().BeFalse();
    }

    private static SupportedArchitecture GetCurrentArchitecture() =>
        SupportedArchitectures.TryGetCurrent() ?? throw new AssertInconclusiveException("Extension binaries are not supported on this machine's OS or architecture.");

    private static IFileHandle Load(LocalArtifactReference reference, IDirectoryHandle cacheRoot)
    {
        LocalArtifactLoader.TryGetEntryPoint(reference, cacheRoot).IsSuccess(out var entryPoint, out var errorBuilder)
            .Should().BeTrue(errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart()).Message);

        return entryPoint!;
    }

    private static Diagnostic LoadError(LocalArtifactReference reference, IDirectoryHandle cacheRoot)
    {
        LocalArtifactLoader.TryGetEntryPoint(reference, cacheRoot).IsSuccess(out _, out var errorBuilder).Should().BeFalse();

        return errorBuilder!(DiagnosticBuilder.ForDocumentStart());
    }

    private static LocalExtensionArtifactCacheEntry GetCacheEntry(LocalArtifactReference reference, IDirectoryHandle cacheRoot) =>
        new(reference.File.TryReadBinaryData().Unwrap(), cacheRoot);

    private async Task<LocalArtifactReference> CreateExtensionReference(string fileName, ExtensionArtifactPackage package)
    {
        var reference = new LocalArtifactReference(ArtifactType.Extension, this.GetFile(fileName), $"./{fileName}");
        reference.File.Write(await LocalExtensionArtifactCodec.Encode(package));

        return reference;
    }

    private IDirectoryHandle GetCacheRoot() => FileHelper.GetCacheRootDirectory(this.TestContext);

    private IFileHandle GetFile(string fileName) =>
        BicepTestConstants.FileExplorer.GetFile(IOUri.FromFilePath(Path.Combine(FileHelper.GetUniqueTestOutputPath(this.TestContext), fileName)));
}
