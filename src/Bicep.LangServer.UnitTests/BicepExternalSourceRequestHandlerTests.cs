// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepExternalSourceRequestHandlerTests
    {
#if WINDOWS_BUILD
        private static string Root(string path) => $"c:\\{path}";
#else
        private static string Root(string path) => $"/{path}";
#endif

        private static readonly IFileSystem MockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["/foo/bar/bicepconfig.json"] = BicepTestConstants.BuiltInConfiguration.ToUtf8Json(),
        });

        private static readonly IConfigurationManager ConfigurationManager = new ConfigurationManager(MockFileSystem);

        private class TelemetryProviderMock
        {
            public Mock<ITelemetryProvider> Mock { get; }
            public BicepTelemetryEvent? Event;
            public ITelemetryProvider Object => Mock.Object;

            public TelemetryProviderMock()
            {
                Mock = new(MockBehavior.Strict);
                Mock.Setup(x => x.PostEvent(It.IsAny<BicepTelemetryEvent>()))
                    .Callback((BicepTelemetryEvent e) =>
                    {
                        Event = e;
                    });
            }
        }

        [TestMethod]
        public async Task InvalidModuleReference_ShouldFail()
        {
            const string ModuleRefStr = "hello";

            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(null as ArtifactReference, x => x.ArtifactRestoreFailed("blah")));
            var telemetryProviderMock = new TelemetryProviderMock();

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"The client specified an invalid module reference '{ModuleRefStr}'.");

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestFailure);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> { { "failureType", "TryGetArtifactReference" } });
        }

        [TestMethod]
        public async Task LocalModuleReference_ShouldFail()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();

            const string ModuleRefStr = "./hello.bicep";
            LocalModuleReference.TryParse(ArtifactType.Module, ModuleRefStr, new Uri("fake:///not/real.bicep")).IsSuccess(out var localRef).Should().BeTrue();
            localRef.Should().NotBeNull();

            ArtifactReference? outRef = localRef;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, failureBuilder));

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"The specified module reference '{ModuleRefStr}' refers to a local module which is not supported by {BicepExternalSourceRequestHandler.BicepExternalSourceLspMethodName} requests.");

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestFailure);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> { { "failureType", "localNotSupported" } });
        }

        [TestMethod]
        public async Task ExternalModuleNotInCache_ShouldReturnJsonContents()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");
            string? compiledJsonContents = "mock main.json contents";

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            var parentModuleLocalPath = "/foo/main.bicep";
            var parentModuleUri = new Uri($"file://{parentModuleLocalPath}");
            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, parentModuleUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, failureBuilder));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Root("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder().WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));
            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(compiledJsonContents);

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestSuccess);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> {
                        { "hasSource", "true"},
                        { "archiveFilesCount","1" },
                        { "fileExtension",".json" },
                        { "requestType","CompiledJson"}
                    });
        }

        [TestMethod]
        public async Task ExternalModuleFailedEntryPoint_ShouldFail()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();
            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            var parentModuleLocalPath = "/main.bicep";
            var parentModuleUri = new Uri($"file://{parentModuleLocalPath}");
            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, parentModuleUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, failureBuilder));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out failureBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(null as Uri, x => x.ArtifactRestoreFailed("blah")));

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"Unable to obtain the entry point URI for module '{ModuleRefStr}'.");

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestFailure);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> { { "failureType", "TryGetLocalArtifactEntryPointUri" } });
        }

        [TestMethod]
        public async Task FailureToReadEntryPoint_ShouldFail()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = IConfigurationManager.GetBuiltInConfiguration();
            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, compiledJsonUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create((string?)null, readFailureBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, BicepTestConstants.CreateMockTelemetryProvider().Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().BeNull();
            response.Error.Should().Be($"Unable to read file 'file:///foo/bar/main.json'. An error occurred reading file. Mock file read failure.");
        }

        [TestMethod]
        public async Task RestoredValidModule_WithNoSources_ShouldReturnJsonContents()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            var telemetryProviderMock = new TelemetryProviderMock();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = ConfigurationManager.GetConfiguration(compiledJsonUri);

            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, compiledJsonUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(compiledJsonContents);

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestSuccess);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> {
                        { "hasSource", "false"},
                        { "archiveFilesCount", "0" },
                        { "fileExtension",".json" },
                        { "requestType", "CompiledJson" }
                    });
        }

        [TestMethod]
        public async Task RestoredValidModule_WithSource_RequestingBicepFile_ShouldReturnBicepContents()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            var telemetryProviderMock = new TelemetryProviderMock();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = ConfigurationManager.GetConfiguration(compiledJsonUri);

            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, compiledJsonUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Root("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder().WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr, Path.GetFileName(bicepUri.AbsoluteUri));
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(bicepSource);
            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestSuccess);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> {
                        { "hasSource", "true"},
                        { "archiveFilesCount","1" },
                        { "fileExtension", ".bicep" },
                        { "requestType", "BicepEntrypoint"}
                    });
        }

        [TestMethod]
        public async Task RestoredValidModule_WithSource_RequestingCompiledJson_ShouldReturnMainJsonContents()
        {
            var dispatcher = StrictMock.Of<IModuleDispatcher>();
            var telemetryProviderMock = new TelemetryProviderMock();

            // needed for mocking out parameters
            DiagnosticBuilder.ErrorBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.ErrorBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var configuration = ConfigurationManager.GetConfiguration(compiledJsonUri);

            OciArtifactReference.TryParseModuleAndAlias(null, UnqualifiedModuleRefStr, configuration, compiledJsonUri).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(ArtifactType.Module, ModuleRefStr, It.IsAny<Uri>())).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Root("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder().WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object);

            // act
            var @params = new BicepExternalSourceParams(ModuleRefStr);
            var response = await handler.Handle(@params, default);

            response.Should().NotBeNull();
            response.Content.Should().Be(compiledJsonContents);

            telemetryProviderMock.Event.Should().NotBeNull();
            telemetryProviderMock.Event!.EventName.Should().Be(TelemetryConstants.EventNames.ExternalSourceRequestSuccess);
            telemetryProviderMock.Event.Properties.Should().Contain(new Dictionary<string, string> {
                        { "hasSource", "true"},
                        { "archiveFilesCount","1" },
                        { "fileExtension",".json" },
                        { "requestType","CompiledJson"}
                    });
        }

        #region GetExternalSourceLinkUri tests

        [TestMethod]
        public void GetExternalSourceLinkUri_FullLink_WithSource()
        {
            Uri result = GetExternalSourceLinkUri(new ExternalSourceLinkTestData());
            result.Should().Be("bicep-extsrc:br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1%2Fentrypoint.bicep %28module1%3Av1%29?br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1#entrypoint.bicep");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_FullLink_WithoutSource()
        {
            Uri result = GetExternalSourceLinkUri(new ExternalSourceLinkTestData(sourceEntrypoint: null));
            result.Should().Be("bicep-extsrc:br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1%2Fmain.json %28module1%3Av1%29?br%3Amyregistry.azurecr.io%2Fmyrepo%2Fbicep%2Fmodule1%3Av1");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_TitlesShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);

            // Source archive entrypoints are always relative to the source root folder, so remove paths
            var expectedEntrypointFilename = Path.GetFileName(testData.sourceEntrypoint ?? "main.json");

            DecodeExternalSourceUri(result).GetShortTitle().Should().Be($"{expectedEntrypointFilename} ({Path.GetFileName(testData.repository)}{testData.tagOrDigest})");
            DecodeExternalSourceUri(result).FullTitle.Should().Be($"br:{testData.registry}/{testData.repository}{testData.tagOrDigest}/{expectedEntrypointFilename} ({Path.GetFileName(testData.repository)}{testData.tagOrDigest})");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_WithExternalModuleFromCache_TitlesShouldBeCorrect()
        {
            var reference = OciArtifactReference.TryParse(
                ArtifactType.Module,
                null,
                "myregistry.azurecr.io/myrepo/bicep/module1:v1",
                BicepTestConstants.BuiltInConfiguration,
                new Uri("http://no-parent", UriKind.Absolute))
            .Unwrap();
            var ext = new ExternalSourceReference(reference, new SourceArchiveBuilder().Build())
                .WithRequestForSourceFile("<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json");

            ext.GetShortTitle().Should().Be("main.json (module1:v1->storage-account:1.0.1)");
            ext.FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1/main.json (module1:v1->storage-account:1.0.1)");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ModuleReferenceShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            DecodeExternalSourceUri(result).Components.ArtifactId.Should().Be($"{testData.registry}/{testData.repository}{testData.tagOrDigest}");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_RequestedFilenameShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            var expectedRequestedFile = testData.sourceEntrypoint is null ? "main.json" : Path.GetFileName(testData.sourceEntrypoint);
            DecodeExternalSourceUri(result).RequestedFile.Should().Be(expectedRequestedFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldStartWithExternalSourceScheme(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().StartWith("bicep-extsrc:");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldStartWithBrOrTs(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().MatchRegex("^bicep-extsrc:(br|ts)%3A", "external links should start with the scheme br: or ts:");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ShouldBeFormedCorrectly(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            result.ToString().Should().MatchRegex("^(?<fullTitle>[^#]+)#(?<module_ref>[^#]+)(?<optional_requested_source_file>%23[^#]+)?$", "external link should have one # and optionally an encoded # after that");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_RequestedFilenameShouldBeBicepOrJson(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            var decoded = DecodeExternalSourceUri(result);
            (decoded.RequestedFile ?? "main.json").Should().MatchRegex(".+\\.(bicep|json)$", "requested source file should end with .json or .bicep");
        }

        private Uri GetExternalSourceLinkUri(ExternalSourceLinkTestData testData)
        {
            Uri? entrypointUri = testData.sourceEntrypoint is { } ? PathHelper.FilePathToFileUrl(testData.sourceEntrypoint) : null;
            OciArtifactReference reference = new(
                ArtifactType.Module,
                testData.registry,
                testData.repository,
                testData.tagOrDigest[0] == ':' ? testData.tagOrDigest[1..] : null,
                testData.tagOrDigest[0] == '@' ? testData.tagOrDigest[1..] : null,
                new Uri("file:///parent.bicep", UriKind.Absolute));

            SourceArchive? sourceArchive = entrypointUri is { } ?
                new SourceArchiveBuilder().WithBicepFile(entrypointUri, "metadata description = 'bicep module'").Build()
                : null;

            return BicepExternalSourceRequestHandler.GetExternalSourceLinkUri(reference, sourceArchive);
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
            string? sourceEntrypoint =
#if WINDOWS_BUILD
                "c:\\entrypoint.bicep", // Use null to indicate no source code is available
#else
                        "/entrypoint.bicep", // Use null to indicate no source code is available
#endif
            string registry = "myregistry.azurecr.io",
            string repository = "myrepo/bicep/module1",
            string tagOrDigest = ":v1" // start with @ for digest
            );

        private static IEnumerable<object[]> GetExternalSourceLinkTestData()
        {
            foreach (var data in GetData())
            {
                yield return new object[] { data };
            }

            static IEnumerable<ExternalSourceLinkTestData> GetData()
            {
                // vary entrypoint (any valid file path character)
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my+main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my$main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my#main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my(main).bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("my%main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("subfolder/main.bicep"));
                yield return new ExternalSourceLinkTestData(sourceEntrypoint: Root("sub folder/my main.bicep"));

                // vary registry (can only be lower-case alphanumeric and '.', '_', '-')
                yield return new ExternalSourceLinkTestData(registry: "myregistry.azurecr.io");
                yield return new ExternalSourceLinkTestData(registry: "hello.my_registry.azurecr.io");
                yield return new ExternalSourceLinkTestData(registry: "hello.my-registry.azurecr.io");

                // vary repo (can only be lower-case alphanumeric and '.', '_', '-')
                yield return new ExternalSourceLinkTestData(registry: "myrepo");
                yield return new ExternalSourceLinkTestData(registry: "myrepo/bicep");
                yield return new ExternalSourceLinkTestData(registry: "myrepo/bicep/module1");
                yield return new ExternalSourceLinkTestData(registry: "myrepo/bicep/mod-ul-e1");
                yield return new ExternalSourceLinkTestData(registry: "my-repo/bicep/mod.ul.e1");
                yield return new ExternalSourceLinkTestData(registry: "my-repo/bicep/mod_ul_e1");

                // vary tag/digest (valid tag characters are alphanumeric, ".", "_", or "-" but the tag cannot begin with ".", "_", or "-")
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":v1");
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":v1.2");
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":1.2.3");
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":v-1");
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":v_1");
                yield return new ExternalSourceLinkTestData(tagOrDigest: ":whoa");
                yield return new ExternalSourceLinkTestData(tagOrDigest: "@sha256:02345342df02345342df02345342df02345342df02345342df02345342df1234");
            }
        }

        #endregion
    }
}
