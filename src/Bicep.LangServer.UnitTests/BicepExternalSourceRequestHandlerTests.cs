// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceLink;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Features.Language.DocumentLink;
using Bicep.Testing;
using Bicep.Testing.Dummies;
using Bicep.Testing.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepExternalSourceRequestHandlerTests
    {
#if WINDOWS_BUILD
        private static string Rooted(string path) => $"c:\\{path}";
#else
        private static string Rooted(string path) => $"/{path}";
#endif
        [TestMethod]
        public async Task InvalidModuleReference_ShouldFail()
        {
            const string ModuleRefStr = "br:hello";
            var handler = new BicepExternalSourceRequestHandler(StrictMock.Of<IRemoteArtifactRestorer>().Object, RemoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"The client specified an invalid module reference '{ModuleRefStr}'.");

        }

        [TestMethod]
        public async Task LocalModuleReference_ShouldFail()
        {
            const string ModuleRefStr = "./hello.bicep";

            var handler = new BicepExternalSourceRequestHandler(StrictMock.Of<IRemoteArtifactRestorer>().Object, RemoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"The specified module reference '{ModuleRefStr}' refers to a local module which is not supported by {BicepExternalSourceRequestHandler.BicepExternalSourceLspMethodName} requests.");

        }

        [TestMethod]
        public async Task ExternalModuleFailedEntryPoint_ShouldFail()
        {
            var restorer = StrictMock.Of<IRemoteArtifactRestorer>();
            const string ModuleRefStr = "br:example.azurecr.invalid/foo/bar:v3";

            var moduleReference = ParseModuleReference(RemoteArtifactContextFactory, ModuleRefStr);
            restorer.Setup(m => m.GetRestoreState(moduleReference)).Returns(new RemoteArtifactRestoreState.RestoreFailed(x => x.ArtifactRestoreFailed("blah")));

            var handler = new BicepExternalSourceRequestHandler(restorer.Object, RemoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"Unable to obtain the entry point URI for module '{ModuleRefStr}'.");

        }

        [TestMethod]
        public async Task FailureToReadEntryPoint_ShouldFail()
        {
            var restorer = StrictMock.Of<IRemoteArtifactRestorer>();
            const string ModuleRefStr = "br:example.azurecr.invalid/foo/bar:v3";

            var (moduleReference, remoteArtifactContextFactory) = CreateMockModuleReferenceAndRemoteArtifactContextFactory(ModuleRefStr);
            restorer.Setup(m => m.GetRestoreState(moduleReference)).Returns(new RemoteArtifactRestoreState.Restored(DummyFileHandle.Default));

            var handler = new BicepExternalSourceRequestHandler(restorer.Object, remoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"Unable to read file '/dummy/path/to/main.json'. An error occurred reading file. File not found.");
        }

        [TestMethod]
        public async Task RestoredValidModule_WithNoSources_ShouldReturnJsonContents()
        {
            string? compiledJsonContents = "mock main.json contents";
            const string ModuleRefStr = "br:example.azurecr.invalid/foo/bar:v3";
            var (moduleReference, remoteArtifactContextFactory) = CreateMockModuleReferenceAndRemoteArtifactContextFactory(ModuleRefStr, null, compiledJsonContents);
            var restorer = StrictMock.Of<IRemoteArtifactRestorer>();
            restorer.Setup(m => m.GetRestoreState(moduleReference)).Returns(new RemoteArtifactRestoreState.Restored(DummyFileHandle.Default));

            var handler = new BicepExternalSourceRequestHandler(restorer.Object, remoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(compiledJsonContents);

        }

        [TestMethod]
        public async Task RestoredValidModule_WithSource_RequestingBicepFile_ShouldReturnBicepContents()
        {
            string? compiledJsonContents = "mock main.json contents";
            const string ModuleRefStr = "br:example.azurecr.invalid/foo/bar:v3";

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var sourceArchive = DummySourceArchive.Create("main.bicep", bicepSource);

            var (moduleReference, remoteArtifactContextFactory) = CreateMockModuleReferenceAndRemoteArtifactContextFactory(ModuleRefStr, sourceArchive, compiledJsonContents);
            var restorer = StrictMock.Of<IRemoteArtifactRestorer>();
            restorer.Setup(m => m.GetRestoreState(moduleReference)).Returns(new RemoteArtifactRestoreState.Restored(DummyFileHandle.Default));

            var handler = new BicepExternalSourceRequestHandler(restorer.Object, remoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr, "main.bicep");
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(bicepSource);
        }

        [TestMethod]
        public async Task RestoredValidModule_WithSource_RequestingCompiledJson_ShouldReturnMainJsonContents()
        {
            string? compiledJsonContents = "mock main.json contents";
            const string ModuleRefStr = "br:example.azurecr.invalid/foo/bar:v3";

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var sourceArchive = DummySourceArchive.Create("main.bicep", bicepSource);

            var (moduleReference, remoteArtifactContextFactory) = CreateMockModuleReferenceAndRemoteArtifactContextFactory(ModuleRefStr, sourceArchive, compiledJsonContents);
            var restorer = StrictMock.Of<IRemoteArtifactRestorer>();
            restorer.Setup(m => m.GetRestoreState(moduleReference)).Returns(new RemoteArtifactRestoreState.Restored(DummyFileHandle.Default));

            var handler = new BicepExternalSourceRequestHandler(restorer.Object, remoteArtifactContextFactory);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(compiledJsonContents);

        }

        #region GetExternalSourceLinkUri tests

        [TestMethod]
        public void GetExternalSourceLinkUri_FullLink_WithSource()
        {
            Uri result = GetExternalSourceLinkUri(new ExternalSourceLinkTestData());
            //result.Should().Be("bicep-extsrc:br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1%2Fentrypoint.bicep %28module1%3Av1%29?br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1#entrypoint.bicep");
            DecodeExternalSourceUri(result).FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> entrypoint.bicep");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_FullLink_WithoutSource()
        {
            Uri result = GetExternalSourceLinkUri(new ExternalSourceLinkTestData(RelativeEntrypoint: null));
            //result.Should().Be("bicep-extsrc:br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1%2Fmodule1%3Av1 -> main.json?br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1");
            DecodeExternalSourceUri(result).FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> main.json");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_FullLink_WithSource_NoModuleBasePath()
        {
            Uri result = GetExternalSourceLinkUri(new ExternalSourceLinkTestData(Repository: "module1"));
            //result.Should().Be("bicep-extsrc:br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1%2Fentrypoint.bicep %28module1%3Av1%29?br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1#entrypoint.bicep");
            DecodeExternalSourceUri(result).FullTitle.Should().Be("br:myregistry.azurecr.io/module1:v1 -> entrypoint.bicep");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_TitlesShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);

            // Source archive entrypoints are always relative to the source root folder, so remove paths
            var expectedEntrypointFilename = Path.GetFileName(testData.SourceEntrypoint ?? "main.json");

            DecodeExternalSourceUri(result).GetShortTitle().Should().Be($"{Path.GetFileName(testData.Repository)}{testData.TagOrDigest} -> {expectedEntrypointFilename}");
            DecodeExternalSourceUri(result).FullTitle.Should().Be($"br:{testData.Registry}/{testData.Repository}{testData.TagOrDigest} -> {expectedEntrypointFilename}");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_WithExternalModuleFromCache_TitlesShouldBeCorrect()
        {
            var components = OciArtifactAddressComponents.TryParse("myregistry.azurecr.io/myrepo/bicep/module1:v1")
                .Unwrap();
            var ext = new ExternalSourceReference(components, null)
                .WithRequestForSourceFile("<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json");

            ext.GetShortTitle().Should().Be("module1:v1 -> storage-account:1.0.1 -> main.json");
            ext.FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> storage-account:1.0.1 -> main.json");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_WithRequestedFileInSubfolder_TitlesShouldBeCorrect()
        {
            var components = OciArtifactAddressComponents.TryParse("myregistry.azurecr.io/myrepo/bicep/module1:v1")
                .Unwrap();
            var ext = new ExternalSourceReference(components, null)
                .WithRequestForSourceFile("subfolder1/subfolder 2/my file.bicep");

            ext.GetShortTitle().Should().Be("module1:v1 -> subfolder1>subfolder 2>my file.bicep");
            ext.FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> subfolder1>subfolder 2>my file.bicep");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ModuleReferenceShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            DecodeExternalSourceUri(result).Components.ArtifactId.Should().Be($"{testData.Registry}/{testData.Repository}{testData.TagOrDigest}");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_RequestedFilenameShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            var expectedRequestedFile = testData.SourceEntrypoint is null ? "main.json" : Path.GetFileName(testData.SourceEntrypoint);
            DecodeExternalSourceUri(result).RequestedFile.Should().Be(expectedRequestedFile);
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldStartWithExternalSourceScheme(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().StartWith("bicep-extsrc:");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldStartWithBrOrTs(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().MatchRegex("^bicep-extsrc:(br|ts)%3A", "external links should start with the scheme br: or ts:");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldBeFormedCorrectly(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().MatchRegex("^(?<fullTitle>[^#]+)#(?<module_ref>[^#]+)(?<optional_requested_source_file>%23[^#]+)?$", "external link should have one # and optionally an encoded # after that");
        }

        [TestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_RequestedFilenameShouldBeBicepOrJson(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            var decoded = DecodeExternalSourceUri(result);
            (decoded.RequestedFile ?? "main.json").Should().MatchRegex(".+\\.(bicep|json)$", "requested source file should end with .json or .bicep");
        }

        [TestMethod]
        public void GetTemplateSpecSourceLinkUri_WithTemplateSpecArtifactReference_ReturnsEncodedUri()
        {
            var subscriptionId = Guid.NewGuid();
            var resourceGroupName = "myRG";
            var templateSpecName = "myTemplateSpec";
            var version = "v1";
            var referenceValue = $"{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}";

            var configurationManager = TestConfigurations.BuiltInWithAllAnalyzersDisabled.WithStaticConfiguration();
            var referencingFile = BicepTestConstants.DummyBicepFile;

            TemplateSpecArtifactReference
                .TryParse(TemplateSpecArtifactContext.Create(referencingFile.LoadConfiguration(), referencingFile.LoadFeatures()), null, referenceValue)
                .IsSuccess(out var reference, out var errorBuilder)
                .Should()
                .BeTrue();

            var result = BicepExternalSourceRequestHandler.GetTemplateSpecSourceLinkUri(reference!);

            result.Should().Be($"bicep-extsrc:ts%3A{subscriptionId}%2FmyRG%2FmyTemplateSpec%3Av1?ts%3A{subscriptionId}%2FmyRG%2FmyTemplateSpec%3Av1");
        }

        private Uri GetExternalSourceLinkUri(ExternalSourceLinkTestData testData)
        {
            Uri? entrypointUri = testData.SourceEntrypoint is { } ? PathHelper.FilePathToFileUrl(testData.SourceEntrypoint) : null;
            OciArtifactReference reference = new(
                OciArtifactContext.Create(BicepTestConstants.DummyBicepFile.LoadConfiguration(), BicepTestConstants.DummyBicepFile.LoadFeatures()),
                ArtifactType.Module,
                testData.Registry,
                testData.Repository,
                testData.TagOrDigest[0] == ':' ? testData.TagOrDigest[1..] : null,
                testData.TagOrDigest[0] == '@' ? testData.TagOrDigest[1..] : null);

            SourceArchive? sourceArchive = entrypointUri is not null
                ? DummySourceArchive.Create(Path.GetFileName(testData.RelativeEntrypoint!))
                : null;

            return BicepExternalSourceRequestHandler.GetRegistryModuleSourceLinkUri(reference, sourceArchive);
        }

        private string TrimFirstCharacter(string s)
        {
            return s.Length > 2 ? s[1..] : s;
        }

        private ExternalSourceReference DecodeExternalSourceUri(Uri uri)
        {
            // NOTE: This code should match src\vscode-bicep\src\language\bicepExternalSourceContentProvider.ts
            string title = Uri.UnescapeDataString(uri.AbsolutePath);
            string moduleReference = Uri.UnescapeDataString(TrimFirstCharacter(uri.Query)); // skip '?'
            string? requestedSourceFile = Uri.UnescapeDataString(TrimFirstCharacter(uri.Fragment)); // skip '#'

            var reference = new ExternalSourceReference(title, moduleReference, requestedSourceFile);

            reference.FullTitle.Should().Be(title);
            reference.FullTitle.Should().EndWith(reference.GetShortTitle());

            return reference;
        }

        public record ExternalSourceLinkTestData(
            string? RelativeEntrypoint = "entrypoint.bicep", // Use null to indicate no source code is available
            string Registry = "myregistry.azurecr.io",
            string Repository = "myrepo/bicep/module1",
            string TagOrDigest = ":v1" // start with @ for digest
        )
        {
            public string? SourceEntrypoint => RelativeEntrypoint is null ? null : Rooted(RelativeEntrypoint);
        }

        private static IEnumerable<object[]> GetExternalSourceLinkTestData()
        {
            foreach (var data in GetData())
            {
                yield return new object[] { data };
            }

            static IEnumerable<ExternalSourceLinkTestData> GetData()
            {
                // vary entrypoint (any valid file path character)
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my+main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my$main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my#main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my(main).bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "my%main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "subfolder/main.bicep");
                yield return new ExternalSourceLinkTestData(RelativeEntrypoint: "sub folder/my main.bicep");

                // vary registry (can only be lower-case alphanumeric and '.', '_', '-')
                yield return new ExternalSourceLinkTestData(Registry: "myregistry.azurecr.io");
                yield return new ExternalSourceLinkTestData(Registry: "hello.my_registry.azurecr.io");
                yield return new ExternalSourceLinkTestData(Registry: "hello.my-registry.azurecr.io");

                // vary repo (can only be lower-case alphanumeric and '.', '_', '-')
                yield return new ExternalSourceLinkTestData(Registry: "myrepo");
                yield return new ExternalSourceLinkTestData(Registry: "myrepo/bicep");
                yield return new ExternalSourceLinkTestData(Registry: "myrepo/bicep/module1");
                yield return new ExternalSourceLinkTestData(Registry: "myrepo/bicep/mod-ul-e1");
                yield return new ExternalSourceLinkTestData(Registry: "my-repo/bicep/mod.ul.e1");
                yield return new ExternalSourceLinkTestData(Registry: "my-repo/bicep/mod_ul_e1");

                // vary tag/digest (valid tag characters are alphanumeric, ".", "_", or "-" but the tag cannot begin with ".", "_", or "-")
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":v1");
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":v1.2");
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":1.2.3");
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":v-1");
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":v_1");
                yield return new ExternalSourceLinkTestData(TagOrDigest: ":whoa");
                yield return new ExternalSourceLinkTestData(TagOrDigest: "@sha256:02345342df02345342df02345342df02345342df02345342df02345342df1234");
            }
        }

        #endregion

        private static readonly IRemoteArtifactContextFactory RemoteArtifactContextFactory = new RemoteArtifactContextFactory(BicepTestConstants.ConfigurationManager, BicepTestConstants.FeatureProviderFactory);

        private static (OciArtifactReference, IRemoteArtifactContextFactory) CreateMockModuleReferenceAndRemoteArtifactContextFactory(string referenceValue, SourceArchive? sourceArchive = null, string? mainJson = null)
        {
            var sourceTgzFileMock = StrictMock.Of<IFileHandle>();
            sourceTgzFileMock.Setup(x => x.Exists()).Returns(sourceArchive is not null);

            if (sourceArchive is not null)
            {
                sourceTgzFileMock.Setup(x => x.OpenRead()).Returns(sourceArchive.PackIntoBinaryData().ToStream());
            }
            else
            {
                sourceTgzFileMock.Setup(x => x.OpenRead()).Throws(new FileNotFoundException("File not found."));
            }

            var mainJsonFileMock = StrictMock.Of<IFileHandle>();
            mainJsonFileMock.Setup(x => x.Exists()).Returns(mainJson is not null);
            mainJsonFileMock.Setup(x => x.Uri).Returns(new IOUri("file", "", "/dummy/path/to/main.json"));

            if (mainJson is not null)
            {
                mainJsonFileMock.Setup(x => x.OpenRead()).Returns(BinaryData.FromString(mainJson).ToStream());
            }
            else
            {
                mainJsonFileMock.Setup(x => x.OpenRead()).Throws(new FileNotFoundException("File not found."));
            }

            var cacheDirectoryMock = StrictMock.Of<IDirectoryHandle>();
            cacheDirectoryMock.Setup(x => x.GetFile("source.tgz")).Returns(sourceTgzFileMock.Object);
            cacheDirectoryMock.Setup(x => x.GetFile("main.json")).Returns(mainJsonFileMock.Object);

            var cacheRootDirectoryMock = StrictMock.Of<IDirectoryHandle>();
            cacheRootDirectoryMock.Setup(x => x.GetDirectory(It.IsAny<string>())).Returns(cacheDirectoryMock.Object);

            var featureProviderMock = StrictMock.Of<IFeatureProvider>();
            featureProviderMock.Setup(x => x.CacheRootDirectory).Returns(cacheRootDirectoryMock.Object);
            featureProviderMock.Setup(x => x.OciEnabled).Returns(false);

            var featureProviderFactoryMock = StrictMock.Of<IFeatureProviderFactory>();
            featureProviderFactoryMock.Setup(x => x.GetFeatureProvider(It.IsAny<IOUri>())).Returns(featureProviderMock.Object);

            var remoteArtifactContextFactory = new RemoteArtifactContextFactory(BicepTestConstants.ConfigurationManager, featureProviderFactoryMock.Object);

            return (ParseModuleReference(remoteArtifactContextFactory, referenceValue), remoteArtifactContextFactory);
        }

        private static OciArtifactReference ParseModuleReference(IRemoteArtifactContextFactory remoteArtifactContextFactory, string fullyQualifiedReference, string? aliasName = null)
        {
            if (!fullyQualifiedReference.StartsWith("br:"))
            {
                throw new ArgumentException("Module reference is not fully qualified.");
            }

            return OciArtifactReference.TryParse(ExternalSourceReference.GetOciArtifactContext(remoteArtifactContextFactory), ArtifactType.Module, aliasName, fullyQualifiedReference[3..]).Unwrap();
        }
    }
}
