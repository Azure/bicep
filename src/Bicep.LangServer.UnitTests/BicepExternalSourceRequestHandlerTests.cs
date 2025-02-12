// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Diagnostics.LinterRuleTests.UseRecentApiVersionRuleTests.GetAcceptableApiVersionsInvariantsTests;

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

        private static readonly IFileExplorer FileExplorer = new FileSystemFileExplorer(new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["/foo/bar/bicepconfig.json"] = BicepTestConstants.BuiltInConfiguration.ToUtf8Json(),
        }));

        private static readonly ConfigurationManager ConfigurationManager = new(FileExplorer);

        private static readonly SourceFileFactory SourceFileFactory = new(ConfigurationManager, new FeatureProviderFactory(ConfigurationManager, FileExplorer));

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
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(null as ArtifactReference, x => x.ArtifactRestoreFailed("blah")));
            var telemetryProviderMock = new TelemetryProviderMock();

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();

            const string ModuleRefStr = "./hello.bicep";
            LocalModuleReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, ModuleRefStr).IsSuccess(out var localRef).Should().BeTrue();
            localRef.Should().NotBeNull();

            ArtifactReference? outRef = localRef;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, failureBuilder));

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");
            string? compiledJsonContents = "mock main.json contents";

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            TryParseOciArtifactReference(BicepTestConstants.DummyBicepFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, failureBuilder));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Rooted("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));
            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder = null;
            var telemetryProviderMock = new TelemetryProviderMock();
            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            TryParseOciArtifactReference(BicepTestConstants.DummyBicepFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, failureBuilder));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out failureBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(null as Uri, x => x.ArtifactRestoreFailed("blah")));

            var resolver = StrictMock.Of<IFileResolver>();

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.DiagnosticBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            TryParseOciArtifactReference(BicepTestConstants.DummyBicepFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create((string?)null, readFailureBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, BicepTestConstants.CreateMockTelemetryProvider().Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.DiagnosticBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var bicepFileUri = new Uri("file:///foo/bar/main.bicep");
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var referencingFile = SourceFileFactory.CreateBicepFile(bicepFileUri, "");

            TryParseOciArtifactReference(referencingFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(new SourceNotAvailableException()));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.DiagnosticBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var bicepFileUri = new Uri("file:///foo/bar/main.bicep");
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var referencingFile = SourceFileFactory.CreateBicepFile(bicepFileUri, "");

            TryParseOciArtifactReference(referencingFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Rooted("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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
            DiagnosticBuilder.DiagnosticBuilderDelegate? nullBuilder = null;
            DiagnosticBuilder.DiagnosticBuilderDelegate? readFailureBuilder = x => x.ErrorOccurredReadingFile("Mock file read failure.");
            string? compiledJsonContents = "mock main.json contents";
            var bicepFileUri = new Uri("file:///foo/bar/main.bicep");
            var compiledJsonUri = new Uri("file:///foo/bar/main.json");

            const string UnqualifiedModuleRefStr = "example.azurecr.invalid/foo/bar:v3";
            const string ModuleRefStr = "br:" + UnqualifiedModuleRefStr;

            var referencingFile = SourceFileFactory.CreateBicepFile(bicepFileUri, "");

            TryParseOciArtifactReference(referencingFile, UnqualifiedModuleRefStr).IsSuccess(out var moduleReference).Should().BeTrue();
            moduleReference.Should().NotBeNull();

            ArtifactReference? outRef = moduleReference;
            dispatcher.Setup(m => m.TryGetArtifactReference(It.IsAny<BicepSourceFile>(), ArtifactType.Module, ModuleRefStr)).Returns(ResultHelper.Create(outRef, null));
            dispatcher.Setup(m => m.GetArtifactRestoreStatus(moduleReference!, out nullBuilder)).Returns(ArtifactRestoreStatus.Succeeded);
            dispatcher.Setup(m => m.TryGetLocalArtifactEntryPointUri(moduleReference!)).Returns(ResultHelper.Create(compiledJsonUri, null));

            var bicepSource = "metadata hi = 'This is the bicep source file'";
            var bicepUri = PathHelper.FilePathToFileUrl(Rooted("foo/bar/entrypoint.bicep"));
            var sourceArchive = new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).WithBicepFile(bicepUri, bicepSource).Build();
            dispatcher.Setup(m => m.TryGetModuleSources(moduleReference!)).Returns(new ResultWithException<SourceArchive>(sourceArchive));

            var resolver = StrictMock.Of<IFileResolver>();
            resolver.Setup(m => m.TryRead(compiledJsonUri)).Returns(ResultHelper.Create(compiledJsonContents, nullBuilder));

            var handler = new BicepExternalSourceRequestHandler(dispatcher.Object, resolver.Object, telemetryProviderMock.Object, BicepTestConstants.SourceFileFactory);

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

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
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
            var ext = new ExternalSourceReference(components, new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).Build())
                .WithRequestForSourceFile("<cache>/br/mcr.microsoft.com/bicep$storage$storage-account/1.0.1$/main.json");

            ext.GetShortTitle().Should().Be("module1:v1 -> storage-account:1.0.1 -> main.json");
            ext.FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> storage-account:1.0.1 -> main.json");
        }

        [TestMethod]
        public void GetExternalSourceLinkUri_WithRequestedFileInSubfolder_TitlesShouldBeCorrect()
        {
            var components = OciArtifactAddressComponents.TryParse("myregistry.azurecr.io/myrepo/bicep/module1:v1")
                .Unwrap();
            var ext = new ExternalSourceReference(components, new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).Build())
                .WithRequestForSourceFile("subfolder1/subfolder 2/my file.bicep");

            ext.GetShortTitle().Should().Be("module1:v1 -> subfolder1>subfolder 2>my file.bicep");
            ext.FullTitle.Should().Be("br:myregistry.azurecr.io/myrepo/bicep/module1:v1 -> subfolder1>subfolder 2>my file.bicep");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_ModuleReferenceShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            DecodeExternalSourceUri(result).Components.ArtifactId.Should().Be($"{testData.Registry}/{testData.Repository}{testData.TagOrDigest}");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExternalSourceLinkTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExternalSourceLinkTestData))]
        public void GetExternalSourceLinkUri_RequestedFilenameShouldBeCorrect(ExternalSourceLinkTestData testData)
        {
            Uri result = GetExternalSourceLinkUri(testData);
            var expectedRequestedFile = testData.SourceEntrypoint is null ? "main.json" : Path.GetFileName(testData.SourceEntrypoint);
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

        [TestMethod]
        public void GetTemplateSpecSourceLinkUri_WithTemplateSpecModuleReference_ReturnsEncodedUri()
        {
            var subscriptionId = Guid.NewGuid();
            var resourceGroupName = "myRG";
            var templateSpecName = "myTemplateSpec";
            var version = "v1";
            var referenceValue = $"{subscriptionId}/{resourceGroupName}/{templateSpecName}:{version}";

            var configurationManager = IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
            var referencingFile = new BicepFile(new Uri("file:///dummy.bicep"), [], SyntaxFactory.EmptyProgram, configurationManager, BicepTestConstants.FeatureProviderFactory, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);

            TemplateSpecModuleReference
                .TryParse(referencingFile, null, referenceValue)
                .IsSuccess(out var reference, out var errorBuilder)
                .Should()
                .BeTrue();

            var result = BicepExternalSourceRequestHandler.GetTemplateSpeckSourceLinkUri(reference!);

            result.Should().Be($"bicep-extsrc:ts%3A{subscriptionId}%2FmyRG%2FmyTemplateSpec%3Av1?ts%3A{subscriptionId}%2FmyRG%2FmyTemplateSpec%3Av1");
        }

        private Uri GetExternalSourceLinkUri(ExternalSourceLinkTestData testData)
        {
            Uri? entrypointUri = testData.SourceEntrypoint is { } ? PathHelper.FilePathToFileUrl(testData.SourceEntrypoint) : null;
            OciArtifactReference reference = new(
                BicepTestConstants.DummyBicepFile,
                ArtifactType.Module,
                testData.Registry,
                testData.Repository,
                testData.TagOrDigest[0] == ':' ? testData.TagOrDigest[1..] : null,
                testData.TagOrDigest[0] == '@' ? testData.TagOrDigest[1..] : null);

            SourceArchive? sourceArchive = entrypointUri is { } ?
                new SourceArchiveBuilder(BicepTestConstants.SourceFileFactory).WithBicepFile(entrypointUri, "metadata description = 'bicep module'").Build()
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

        private static ResultWithDiagnosticBuilder<OciArtifactReference> TryParseOciArtifactReference(BicepFile referencingFile, string value, string? aliasName = null) =>
            OciArtifactReference.TryParse(referencingFile, ArtifactType.Module, aliasName, value);

    }
}
